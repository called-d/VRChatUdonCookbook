---
title: "U# でよく使うプロキシスクリプトについて"
emoji: "📔"
type: "tech" # tech: 技術記事 / idea: アイデア
topics: ["Unity", "VRChat", "UdonSharp"]
published: false
---

VRChat でワールドギミックを作っていると、プレイヤーのインタラクトを別の Udon のスクリプトに転送したい事があります。稀によくある。

中身はシンプルですが、これを知っていると回避できるトラブルもあるし、他のスクリプトの前提として出てくることがあり、第 0 回の題材として使います。

## ソースコード
```cs:InteractProxy.cs
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class InteractProxy : UdonSharpBehaviour
{
    [SerializeField] private UdonBehaviour _target;
    [SerializeField] private string _eventName;


    public override void Interact()
    {
        if (!Utilities.IsValid(_target))
        {
            Debug.LogError("_target is not valid.", this);
            return;
        }
        if (string.IsNullOrEmpty(_eventName))
        {
            Debug.LogError("_eventName is null or \"\".", this);
            return;
        }

        _target.SendCustomEvent(_eventName);
    }
}
```

:::details 受ける側
とりあえず分かりやすいように適当にランダムに向きが変わります。

```cs:SampleTargetObject.cs
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class SampleTargetObject : UdonSharpBehaviour
{
    private readonly string TAG = "[<color=#ff88ff>SampleTargetObject</color>]";

    public void _EventA()
    {
        Debug.Log($"{TAG}: Event A を受け取りました", this);
        transform.rotation = Random.rotation;
    }
}
```
:::

:::details Pickup 用
長いので折りたたみますが、同じ要領で Pickup 用も作れます。
こっちは全部のイベントを常に投げたいわけじゃないので文字が空のときエラー出さずに return する方がいいかな。

```cs:PickupProxy.cs
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class PickupProxy : UdonSharpBehaviour
{
    [SerializeField] private UdonBehaviour _target;
    [SerializeField] private string _dropEventName;
    [SerializeField] private string _pickupEventName;
    [SerializeField] private string _pickupUseDownEventName;
    [SerializeField] private string _pickupUseUpEventName;


    public override void OnPickup()
    {
        if (!Utilities.IsValid(_target))
        {
            Debug.LogError("_target is not valid.", this);
            return;
        }
        if (string.IsNullOrEmpty(_pickupEventName)) return;
        _target.SendCustomEvent(_pickupEventName);
    }

    public override void OnDrop()
    {
        if (!Utilities.IsValid(_target))
        {
            Debug.LogError("_target is not valid.", this);
            return;
        }
        if (string.IsNullOrEmpty(_dropEventName)) return;
        _target.SendCustomEvent(_dropEventName);
    }

    public override void OnPickupUseDown()
    {
        if (!Utilities.IsValid(_target))
        {
            Debug.LogError("_target is not valid.", this);
            return;
        }
        if (string.IsNullOrEmpty(_pickupUseDownEventName)) return;
        _target.SendCustomEvent(_pickupUseDownEventName);
    }

    public override void OnPickupUseUp()
    {
        if (!Utilities.IsValid(_target))
        {
            Debug.LogError("_target is not valid.", this);
            return;
        }
        if (string.IsNullOrEmpty(_pickupUseUpEventName)) return;
        _target.SendCustomEvent(_pickupUseUpEventName);
    }
}
```
:::

:::details ついでに SetActive に反応するやつ
オブジェクトの初期状態が 非Active だと同期まわりが怪しかったりするので、外の Udon に逃がせると便利だったり

```cs:ActiveProxy.cs
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class ActiveProxy : UdonSharpBehaviour
{
    [SerializeField] private UdonBehaviour[] _targets;
    [SerializeField] private string[] _activeEventNames;
    [SerializeField] private string[] _inactiveEventNames;

    void OnEnable()
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            if (!Utilities.IsValid(_targets[i])) continue;
            if (i >= _activeEventNames.Length || string.IsNullOrEmpty(_activeEventNames[i])) continue;
            _targets[i].SendCustomEvent(_activeEventNames[i]);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            if (!Utilities.IsValid(_targets[i])) continue;
            if (i >= _inactiveEventNames.Length || string.IsNullOrEmpty(_inactiveEventNames[i])) continue;
            _targets[i].SendCustomEvent(_inactiveEventNames[i]);
        }
    }
}

```
この手の操作は Active Relay[^activerelay] がおすすめです。
:::

この例だと、シーンに配置して Udon をアタッチして、変数 _target に GameObject `Target (Cube)` を、変数 _eventName に文字列 `_EventA` を設定しました。
![シーンに配置して、インスペクタに target を設定する](/images/00_proxies_scene.png)

## 中身の解説
一応中身を解説すると

### `public override void Interact()`
プレイヤーがワールド設置物を使うときに呼ばれるイベントです。コントローラーのトリガーなり、マウスのクリックなりで操作するたびに何回でも呼び出されます。
ざっくり言うと「ボタン押したとき呼ばれるよ」ぐらいのアレ。

Pickup の場合「手に持って使う」は別のイベントなので、`Interact()` ではなく、`OnPickupUseDown()` を使います。

### `_target.SendCustomEvent(_eventName);`
ターゲットである別のプログラム（Udon） `_target` に `_eventName` 変数に入れた名前のイベントを送信します。

:::details 中身の解説・細かいやつ

### `Debug.Log() / .LogError()`
ログを出します。Unity エディタの Console タブにも出ますし、Unityエディタからなら `%LOCALAPPDATA%\Unity\Editor\Editor.log`、VRChat のログなら `%USERPROFILE%\AppData\LocalLow\VRChat\VRChat\output_log_<日時>.txt` にも書かれます。

※`%USERPROFILE%` は `C:\Users\<ユーザー名>`、`%LOCALAPPDATA%` ってのは `C:\Users\<ユーザー名>\AppData\Local` です。~~わざわざ解説書くなら気取ってこう書く意味ないが~~

### `[SerializeField]`
これがあると Unity のインスペクタから値やシーン上のオブジェクトの参照を設定できます。

### `UdonBehaviour _target;`
VRChat ワールド内の個々の Udon のプログラムは `UdonBehaviour` という型を持っています。これは UdonSharp でも Udon Graph でも同じです。

ここでは、インタラクトされた `InteractProxy` 自身ではなく、別のプログラムの処理を呼び出したいわけなので、その相手（`_target`）を変数に持つわけです。

### `Utilities.IsValid(_target)`
普通の .cs プログラムだと `_target != null` とかでチェックするものです。VRChat のワールドでは、ログアウトした瞬間のプレイヤーとかで「null でもないのに無効なオブジェクト」が居たりするので、オブジェクト相手にはこちらを使うのが無難です。

### `public void _EventA()`
SendCustomEvent で呼ばれてイベントとして受け取るためには可視性が `public` でないといけません。
また、今回はネットワーク同期が関係ないので、ネットワーク側から呼ばれないように[^legacysecurity]先頭にアンダースコアを付けます。

:::

## なんでこれが必要になるのか
公式[^performance] も Udon Behaviour を跨いだ呼び出しはローカルメソッド呼び出しより遅くなるから避けてって言ってるので、意味もなくこの SendCustomEvent パターンを使うのは避けたいところではあります。

ところが VRChat の同期システムは一個の GameObject に付いた全ての Udon Behaviour の同期モード（manual /continuous）を統一しないといけない都合で、とくに Pickup と同期を含んだスクリプトの相性が悪かったりします。
また、Interact 等のイベントは実際に押される・持たれるオブジェクトに付いた Udon にしか送られないので、ボタンや銃などのユーザーインターフェースと、メインのロジック処理を分けたい時にはこのパターンが必要になってくるかと思います。

[^performance]: Random Tips & Performance Pointers | UdonSharp https://udonsharp.docs.vrchat.com/random-tips-&-performance-pointers/#sendcustomevent--method-calls-across-behaviours

[^activerelay]: https://github.com/mimyquality/FukuroUdon/wiki/Active-Relay

[^legacysecurity]: https://creators.vrchat.com/worlds/udon/networking/events/#legacy-events-and-security
