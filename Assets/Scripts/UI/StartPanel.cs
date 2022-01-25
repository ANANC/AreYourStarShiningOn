using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : Stone_UIObject
{
    public const string Name = "StartPanel";
    public static Stone_UIObject CreateUI()
    {
        return new StartPanel();
    }

    private Button m_GameButton;
    private Button m_ExplainButton;
    private GameObject m_ExplainGameObject;

    private GameObject m_StarBackground;

    private Stone_ResourceManager ResourceManager;

    public override void Start()
    {
        Transform root = GetTransform();
        m_GameButton = root.Find("GameButton").GetComponent<Button>();
        m_ExplainButton = root.Find("ExplainButton").GetComponent<Button>();
        m_ExplainGameObject = root.Find("Explain").gameObject;

        m_GameButton.onClick.AddListener(GameButtonOnClick);
        m_ExplainButton.onClick.AddListener(ExplainButtonOnClick);

        ResourceManager = Stone_RunTime.GetManager<Stone_ResourceManager>(Stone_ResourceManager.Name);
        m_StarBackground = ResourceManager.Instance("100005_StarBackground.prefab", "Model");
    }

    public override void Destroy()
    {
        ResourceManager.DestroyGameObject(m_StarBackground);

        m_GameButton.onClick.RemoveAllListeners();
        m_ExplainButton.onClick.RemoveAllListeners();

        m_GameButton = null;
        m_ExplainButton = null;
        m_ExplainGameObject = null;

        ResourceManager = null;
    }

    public override void Open()
    {

    }

    public override void Close()
    {

    }

    private void GameButtonOnClick()
    {
        Stone_StateManager stateManager = Stone_RunTime.GetManager<Stone_StateManager>(Stone_StateManager.Name);
        stateManager.EnterState(GameState.Name, true);
    }

    private void ExplainButtonOnClick()
    {
        m_ExplainGameObject.SetActive(!m_ExplainGameObject.activeSelf);
    }
}
