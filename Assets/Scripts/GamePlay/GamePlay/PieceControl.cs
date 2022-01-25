using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PieceControl
{
    private Vector3 m_LogicPos;
    private Vector3 m_ArtPos;

    private bool m_HasSart;
    private int m_StarGrade;

    private GameObject m_PieceGameObject;
    private GameObject m_WillCreateGameObject;
    private GameObject m_LockGameObject;
    private GameObject m_ControlGameObject;
    private GameObject m_ControlDirectionGameObject;

    private GameObject m_StarGameObject;

    public void Init()
    {
        m_HasSart = false;
        m_PieceGameObject = null;
        m_StarGameObject = null;
        m_StarGrade = -1;
    }

    public void UnInit()
    {
        if (m_PieceGameObject != null)
        {
            GameObject.DestroyImmediate(m_PieceGameObject);
            m_PieceGameObject = null;
        }

        if (m_StarGameObject != null)
        {
            GameObject.DestroyImmediate(m_StarGameObject);
            m_StarGameObject = null;
        }
    }

    public void SetPosition(Vector3 logicPos, Vector3 artPos)
    {
        m_LogicPos = logicPos;
        m_ArtPos = artPos;
    }

    public Vector3 GetLogicPos()
    {
        return m_LogicPos;
    }

    public Vector3 GetArtPos()
    {
        return m_ArtPos;
    }

    public void AddStar(GameObject gameObject, int starGrade, int exceedGrade)
    {
        m_HasSart = true;

        m_StarGameObject = gameObject;
        m_StarGrade = starGrade;

        if (exceedGrade > 0)
        {
            Transform exceedNumberTransform = m_StarGameObject.transform.Find("ExceedNumber");
            for (int index = 0; index < 6; index++)
            {
                exceedNumberTransform.GetChild(index).gameObject.SetActive((index + 1) == exceedGrade);
            }
            Transform exceedTransform = m_StarGameObject.transform.Find("Exceed");
            for (int index = 0; index < 6; index++)
            {
                exceedTransform.GetChild(index).gameObject.SetActive((index + 1) == exceedGrade);
            }
        }
    }

    public void DeleteStar()
    {
        m_HasSart = false;

        m_StarGameObject = null;
        m_StarGrade = -1;
    }

    public bool GetHasSart()
    {
        return m_HasSart;
    }

    public int GetStarGrade()
    {
        return m_StarGrade;
    }

    public void SetPieceGameObject(GameObject gameObject)
    {
        m_PieceGameObject = gameObject;
        m_WillCreateGameObject = m_PieceGameObject.transform.Find("WillCreate").gameObject;
        m_LockGameObject = m_PieceGameObject.transform.Find("Lock").gameObject;
        m_ControlGameObject = m_PieceGameObject.transform.Find("Control").gameObject;
        m_ControlDirectionGameObject = m_ControlGameObject.transform.Find("Direction").gameObject;
    }

    public GameObject GetStarGameObject()
    {
        return m_StarGameObject;
    }

    public void ShowWillCreateArt(int grade)
    {
        m_WillCreateGameObject.SetActive(true);
        m_WillCreateGameObject.transform.GetChild(grade - 1).gameObject.SetActive(true);
    }

    public void HideWillCreateArt(int grade)
    {
        m_WillCreateGameObject.SetActive(false);
        m_WillCreateGameObject.transform.GetChild(grade - 1).gameObject.SetActive(false);
    }

    public void ShowLock()
    {
        m_LockGameObject.SetActive(true);
    }

    public void HideLock()
    {
        m_LockGameObject.SetActive(false);
    }

    public void ShowControl(Vector3 direction)
    {
        m_ControlGameObject.SetActive(true);

        if (direction.x > 0)//右
        {
            m_ControlDirectionGameObject.transform.localEulerAngles = new Vector3(0,0,0);
            m_ControlDirectionGameObject.SetActive(true);
        }
        else if (direction.x < 0)//左
        {
            m_ControlDirectionGameObject.transform.localEulerAngles = new Vector3(0, 180f, 0);
            m_ControlDirectionGameObject.SetActive(true);
        }
        else if (direction.y > 0)//上
        {
            m_ControlDirectionGameObject.transform.localEulerAngles = new Vector3(0, 270f, 0);
            m_ControlDirectionGameObject.SetActive(true);
        }
        else if (direction.y < 0)//下
        {
            m_ControlDirectionGameObject.transform.localEulerAngles = new Vector3(0, 90f, 0);
            m_ControlDirectionGameObject.SetActive(true);
        }
    }

    public void ShowControl()
    {
        m_ControlGameObject.SetActive(true);
        m_ControlDirectionGameObject.SetActive(false);
    }

    public void HideControl()
    {
        m_ControlGameObject.SetActive(false);
        m_ControlDirectionGameObject.SetActive(false);

    }
}
