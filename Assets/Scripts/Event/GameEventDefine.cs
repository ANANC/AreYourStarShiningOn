using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventDefine 
{
    // 事件名命名规范：XXXEvent
    // 事件信息命名规范：XXXEventInfo

    //游戏胜利
    public const string GamePlayWinEvent = "GamePlayWinEvent";


    //更新玩法提示
    public const string UpdateGamePlayTipEvent = "UpdateGamePlayTipEvent";
    public class UpdateGamePlayTipEventInfo : Stone_EventObject.EventCallbackInfo
    {
        public string tip;  //提示
    }

    //更新固定提示
    public const string UpdateFixedTipEvent = "UpdateFixedTipEvent";
    public class UpdateFixedTipEventInfo : Stone_EventObject.EventCallbackInfo
    {
        public string tip;  //提示
    }

    //更新锁定提示
    public const string UpdateLockTipEvent = "UpdateLockTipEvent";
    public class UpdateLockTipEventInfo : Stone_EventObject.EventCallbackInfo
    {
        public string tip;  //提示
    }


    //玩家点击块事件
    public const string PlayerClickPieceEvent = "PlayerClickPieceEvent";
    public class PlayerClickPieceEventInfo : Stone_EventObject.EventCallbackInfo
    {
        public Vector3 logicPos;  //逻辑位置
    }

    //玩家点击选择方向事件
    public const string PlayerClickSeleteEvent = "PlayerClickSeleteEvent";
    public class PlayerClickSeleteEventInfo : Stone_EventObject.EventCallbackInfo
    {
        public Vector3 direction;  //方向
    }
}
