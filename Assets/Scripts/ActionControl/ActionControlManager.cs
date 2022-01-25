using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionControlManager : Stone_Manager
{
    public const string Name = "ActionControlManager";
    public override string GetName()
    {
        return ActionControlManager.Name;
    }

    private bool m_EnableControl;

    private Stone_EventManager EventManager;

    public override void Init()
    {
        m_EnableControl = true;

        EventManager = Stone_RunTime.GetManager<Stone_EventManager>(Stone_EventManager.Name);
        EventManager.AddListener(GameEventDefine.GamePlayWinEvent, this, GamePlayWin);
    }

    public override void UnInit()
    {
        EventManager = null;
    }

    public override void Update()
    {
        if(!m_EnableControl)
        {
            return;
        }

        //左键点击
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 50, LayerMask.GetMask("Default"));

            if (hits == null)
            {
                return;
            }

            Vector3 selectDirection = Vector3.zero;
            string peiceName = string.Empty;

            for (int index = 0; index < hits.Length; ++index)
            {
                RaycastHit hit = hits[index];

                GameObject gameObject = hit.collider.gameObject;

                if (gameObject.tag == GamePlay_CreateMaxStar.SelectLeftTag)
                {
                    selectDirection = Vector3.left;
                    break;
                }
                if (gameObject.tag == GamePlay_CreateMaxStar.SelectRightTag)
                {
                    selectDirection = Vector3.right;
                    break;
                }
                if (gameObject.tag == GamePlay_CreateMaxStar.SelectUpTag)
                {
                    selectDirection = Vector3.up;
                    break;
                }
                if (gameObject.tag == GamePlay_CreateMaxStar.SelectDownTag)
                {
                    selectDirection = Vector3.down;
                    break;
                }
                if (gameObject.tag == GamePlay_CreateMaxStar.PieceTag)
                {
                    peiceName = gameObject.name;
                }
            }

            if (selectDirection != Vector3.zero)
            {
                SendPlayerClickSeleteEvent(selectDirection);
            }
            else if (!string.IsNullOrEmpty(peiceName))
            {
                SendPlayerClickPieceEvent(peiceName);
            }

        }
    }

    private void SendPlayerClickSeleteEvent(Vector3 direction)
    {
        GameEventDefine.PlayerClickSeleteEventInfo info = new GameEventDefine.PlayerClickSeleteEventInfo();
        info.direction = direction;
        EventManager.Execute(GameEventDefine.PlayerClickSeleteEvent, info);
    }

    private void SendPlayerClickPieceEvent(string pieceName)
    {
        string[] chips = pieceName.Split(',');
        if(chips.Length!=2)
        {
            return;
        }
        int x = int.Parse(chips[0]);
        int y = int.Parse(chips[1]);

        GameEventDefine.PlayerClickPieceEventInfo info = new GameEventDefine.PlayerClickPieceEventInfo();
        info.logicPos = new Vector3(x,y,0);
        EventManager.Execute(GameEventDefine.PlayerClickPieceEvent, info);
    }

    public void GamePlayWin()
    {
        m_EnableControl = false;
    }
}
