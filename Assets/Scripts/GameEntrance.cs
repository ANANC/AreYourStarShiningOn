using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntrance : MonoBehaviour
{
    private Stone_RunTime m_StoneRunTime;

    [Header("使用AssetBundle")]
    public bool m_UseAssetBundle;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(this);

        m_StoneRunTime = new Stone_RunTime();
        Stone_RunTime.Current = m_StoneRunTime;

        Entrance();
    }

    // Update is called once per frame
    void Update()
    {
        m_StoneRunTime.Update();
    }

    void OnDestroy()
    {
        m_StoneRunTime.Destroy();
    }

    // 启动
    void Entrance()
    {
        Stone_ResourceManagerLifeControl resourceManagerLifeControl = new Stone_ResourceManagerLifeControl();
        resourceManagerLifeControl.UseAssetBundle = m_UseAssetBundle;

        //通用
        Stone_RunTime.AddManager(new Stone_UserConfigManager(new Stone_UserConfigManagerLifeControl()));
        Stone_RunTime.AddManager(new Stone_ResourceManager(resourceManagerLifeControl));
        Stone_RunTime.AddManager(new Stone_StateManager(new Stone_StateManagerLifeControl()));
        Stone_RunTime.AddManager(new Stone_EventManager());
        Stone_RunTime.AddManager(new Stone_TimerManager(),true);
        Stone_RunTime.AddManager(new Stone_UIManager(new Stone_UIManagerLifeControl()));
        Stone_RunTime.AddManager(new Stone_SoundManager(new Stone_SoundManagerLifeControl()));

        //游戏-通用
        //Stone_RunTime.AddManager(new ModelManager());

        Stone_UserConfigManager userConfigManager = Stone_RunTime.GetManager<Stone_UserConfigManager>(Stone_UserConfigManager.Name);
        Stone_StateManager stateManager = Stone_RunTime.GetManager<Stone_StateManager>(Stone_StateManager.Name);

        GameEntranceConfig entranceConfig = userConfigManager.GetConfig<GameEntranceConfig>();
        stateManager.EnterState(entranceConfig.FirstEnterStateName);
    }


}
