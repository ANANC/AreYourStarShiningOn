using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartState : Stone_IState
{
    public const string Name = "StartState";
    public string GetName()
    {
        return StartState.Name;
    }

    public void Init()
    {
    }

    public void UnInit()
    {

    }

    public void Enter()
    {
        Stone_SoundManager soundManager = Stone_RunTime.GetManager<Stone_SoundManager>(Stone_SoundManager.Name);
        soundManager.PlayBGM("Game.mp3");

        Stone_UIManager uiManager = Stone_RunTime.GetManager<Stone_UIManager>(Stone_UIManager.Name);
        uiManager.OpenUI(StartPanel.Name);
    }

    public void Exist()
    {
        Stone_UIManager uiManager = Stone_RunTime.GetManager<Stone_UIManager>(Stone_UIManager.Name);
        uiManager.DestroyUI(StartPanel.Name);
    }

}
