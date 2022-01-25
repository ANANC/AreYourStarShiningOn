using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : Stone_UIObject
{
    public const string Name = "WinPanel";
    public static Stone_UIObject CreateUI()
    {
        return new WinPanel();
    }

    private Button m_ReturnButton;
    private Image m_CircleImage;
    private Transform m_CircleTransform;
    private Image m_StarImage;
    private Text m_TextText;

    private string m_ShowText;

    public override void Start()
    {
        Transform root = GetTransform();
        m_ReturnButton = root.Find("Button").GetComponent<Button>();
        m_CircleTransform = root.Find("GameObject/Circle");
        m_CircleImage = m_CircleTransform.GetComponent<Image>();
        m_StarImage = root.Find("GameObject/Star").GetComponent<Image>();
        m_TextText = root.Find("Text").GetComponent<Text>();

        m_ReturnButton.onClick.AddListener(ReturnButtonOnClick);


        m_ShowText =
            "不远处，千万的星屑飞行离去...\n" +            "闪烁星光在漫无边际的黑暗无声燃烧...\n" +            "守护的蓝色星球此刻正静静地运行在它的轨道中。\n" +            "宇宙的第一束光线，它去了哪里？\n" +            "如同已经无法探究的星球,\n" +            "我们也会消逝在时光里。\n" +            "但是今天,\n" +            "你好，地球。";

    }

    public override void Destroy()
    {
        m_ReturnButton.onClick.RemoveAllListeners();

        m_StarImage = null;
        m_ReturnButton = null;
        m_CircleImage = null;
        m_CircleTransform = null;
        m_TextText = null;
    }

    public override void Open()
    {
        Tween();
    }

    public override void Close()
    {
    }

    private readonly float m_StarShowTime = 1f;
    private readonly float m_CircleShowTime = 2f;
    private readonly float m_CircleBackgroundShowTime = 2f;
    private readonly float m_CircleBackgroundWaitTime = 8f;
    private readonly float m_CircleBackgroundHideTime = 0.5f;
    private readonly float m_TextShowTime = 24f;
    private readonly float m_TextWaitTime = 0.2f;


    private void Tween()
    {
        Sequence circleSequence = DOTween.Sequence();

        //显示球
        circleSequence.Append(m_CircleImage.DOColor(Color.white, m_CircleShowTime));

        //显示球背景
        for (int index = 0; index < m_CircleTransform.childCount; index++)
        {
            Image image = m_CircleTransform.GetChild(index).GetComponent<Image>();
            circleSequence.Append(image.DOColor(Color.white, m_CircleBackgroundShowTime));
        }
        circleSequence.AppendInterval(m_CircleBackgroundWaitTime);
        Color hideColor = Color.white;
        hideColor.a = 0;
        //隐藏球背景
        for (int index = 0; index < m_CircleTransform.childCount; index++)
        {
            Image image = m_CircleTransform.GetChild(index).GetComponent<Image>();
            circleSequence.Append(image.DOColor(hideColor, m_CircleBackgroundHideTime));
        }

        Sequence textSequence = DOTween.Sequence();

        textSequence.AppendInterval(m_CircleShowTime + 1f);

        int textLength = m_ShowText.Length;
        float singleTextTime = m_TextShowTime / textLength;

        for (int index = 1; index <= textLength; index++)
        {
            string text = m_ShowText.Substring(0, index);
            //显示文字
            textSequence.AppendCallback(() =>
            {
                m_TextText.text = text;
            });
            textSequence.AppendInterval(singleTextTime);
        }

        //显示星星
        textSequence.Append(m_StarImage.DOColor(Color.white, m_StarShowTime));

        //等待
        textSequence.AppendInterval(m_TextWaitTime);

        textSequence.AppendCallback(() =>
        {
            m_ReturnButton.gameObject.SetActive(true);
        });
    }

    private void ReturnButtonOnClick()
    {
        Stone_StateManager stateManager = Stone_RunTime.GetManager<Stone_StateManager>(Stone_StateManager.Name);
        stateManager.EnterState(StartState.Name, true);
    }


}
