using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePlayerManager : Stone_Manager
{
    public const string Name = "GamePlayerManager";
    public override string GetName()
    {
        return GamePlayerManager.Name;
    }

    private Dictionary<int, Type> GamePlayType2ControllerDict;

    public class UserGamePlayInfo : Stone_BaseUserConfigData
    {
        public int GamePlayType;    //类型
        public string ExpandConfigName; //扩展信息配置名
    }

    private UserGamePlayInfo m_GamePlayInfo;
    private IGamePlayController m_GamePlay;

    public GamePlayerManager(Stone_IManagerLifeControl stone_ManagerLifeControl) : base(stone_ManagerLifeControl)
    {
    }

    public override void Init()
    {
        GamePlayType2ControllerDict = new Dictionary<int, Type>();
    }

    public override void UnInit()
    {
        m_GamePlay?.UnInit();
    }

    public void AddGamePlayTagAndType(int gamePlayTag,Type gameplayType)
    {
        GamePlayType2ControllerDict.Add(gamePlayTag, gameplayType);
    }

    public void CreateGamePlay(string gamePlayConfigName)
    {
        Stone_UserConfigManager userConfigManager = Stone_RunTime.GetManager<Stone_UserConfigManager>(Stone_UserConfigManager.Name);
        m_GamePlayInfo = userConfigManager.GetConfig<UserGamePlayInfo>(gamePlayConfigName);

        Type gamePlayTypeClass = GamePlayType2ControllerDict[m_GamePlayInfo.GamePlayType];

        System.Object gamePlayerObject = Activator.CreateInstance(gamePlayTypeClass, null);

        m_GamePlay = gamePlayerObject as IGamePlayController;
        m_GamePlay.Init(m_GamePlayInfo.ExpandConfigName);
    }

    public T GetGamePlay<T>() where T: IGamePlayController
    {
        if(m_GamePlay == null)
        {
            return default(T);
        }

        return (T)m_GamePlay;
    }
}
