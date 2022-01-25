using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : Stone_IState
{
    public const string Name = "BattleState";
    public string GetName()
    {
        return GameState.Name;
    }
    public void Init()
    {
    }

    public void UnInit()
    {

    }

    public void Enter()
    {
        Stone_RunTime.AddManager(new GamePlayerManager(new GamePlayerManagerLifeControl()));
        Stone_RunTime.AddManager(new ActionControlManager(),true);

        Stone_UIManager uiManager = Stone_RunTime.GetManager<Stone_UIManager>(Stone_UIManager.Name);
        uiManager.OpenUI(MainPlayPanel.Name);

        GamePlayerManager gamePlayerManager = Stone_RunTime.GetManager<GamePlayerManager>(GamePlayerManager.Name);
        gamePlayerManager.CreateGamePlay("200001_GamePlay");
    }

    public void Exist()
    {
        Stone_UIManager uiManager = Stone_RunTime.GetManager<Stone_UIManager>(Stone_UIManager.Name);
        uiManager.DestroyUI(WinPanel.Name);

        Stone_RunTime.DeleteManager<ActionControlManager>(ActionControlManager.Name);
        Stone_RunTime.DeleteManager<GamePlayerManager>(GamePlayerManager.Name);
    }

}
