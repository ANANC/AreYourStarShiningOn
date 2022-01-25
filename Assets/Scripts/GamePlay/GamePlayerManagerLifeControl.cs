using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManagerLifeControl : Stone_IManagerLifeControl
{
    public enum GamePlayType
    {
        CreateMaxStar = 1,  //创建最大星星
    }


    public void InitAfter(Stone_Manager manager)
    {
        GamePlayerManager gamePlayerManager = (GamePlayerManager)manager;

        gamePlayerManager.AddGamePlayTagAndType((int)GamePlayType.CreateMaxStar, typeof(GamePlay_CreateMaxStar));
    }
}
