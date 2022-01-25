using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameEventDefine;

public class MainPlayPanel : Stone_UIObject
{
    public const string Name = "MainPlayPanel";
    public static Stone_UIObject CreateUI()
    {
        return new MainPlayPanel();
    }

    private Text m_PlayTipText;
    private Text m_FixedTipText;
    private Text m_LockTipText;
    private Transform m_WinTransform;

    private Sequence m_WinSequence;

    private Stone_EventManager EventManager;

    public override void Start()
    {
        Transform root = GetTransform();
        m_PlayTipText = root.Find("Bottom/PlayTip").GetComponent<Text>();
        m_FixedTipText = root.Find("Bottom/FixedTip").GetComponent<Text>();
        m_LockTipText = root.Find("Top/Lock").GetComponent<Text>();
        m_WinTransform = root.Find("Win");

        EventManager = Stone_RunTime.GetManager<Stone_EventManager>(Stone_EventManager.Name);
        EventManager.AddListener<UpdateGamePlayTipEventInfo>(UpdateGamePlayTipEvent, this, UpdatePlayTip);
        EventManager.AddListener<UpdateFixedTipEventInfo>(UpdateFixedTipEvent, this, UpdateFixedTip);
        EventManager.AddListener<UpdateLockTipEventInfo>(UpdateLockTipEvent, this, UpdateLockTip);
        EventManager.AddListener(GamePlayWinEvent, this, GamePlayWin);

    }

    public override void Destroy()
    {
        EventManager.DeleteTargetAllListener(this);

        if(m_WinSequence!=null)
        {
            m_WinSequence.Kill();
        }

        m_PlayTipText = null;
        m_FixedTipText = null;
        m_LockTipText = null;
        m_WinTransform = null;

        EventManager = null;
    }

    public override void Open()
    {

    }

    public override void Close()
    {

    }

    public void UpdatePlayTip(UpdateGamePlayTipEventInfo info)
    {
        m_PlayTipText.text = info.tip;
    }

    public void UpdateFixedTip(UpdateFixedTipEventInfo info)
    {
        m_FixedTipText.text = info.tip;
    }

    public void UpdateLockTip(UpdateLockTipEventInfo info)
    {
        m_LockTipText.text = info.tip;
    }

    public void GamePlayWin()
    {
        m_WinSequence = DOTween.Sequence();

        float time = 1.1f;
        int count = m_WinTransform.childCount;
        for (int index = count - 1; index >= 0; --index)
        {
            Image image = m_WinTransform.GetChild(index).GetComponent<Image>();
            m_WinSequence.Append(image.DOColor(Color.white, time * 2));
        }
    }
}
