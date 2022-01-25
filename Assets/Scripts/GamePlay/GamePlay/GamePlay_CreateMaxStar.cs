using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameEventDefine;

public class GamePlay_CreateMaxStar : IGamePlayController
{
    public const string SelectLeftTag = "select_left";
    public const string SelectRightTag = "select_right";
    public const string SelectUpTag = "select_up";
    public const string SelectDownTag = "select_down";
    public const string PieceTag = "piece";

    private const string WinSoundName = "SomeOtherTime.mp3";
    private const string ExplodeSoundName = "Explode.mp3";
    private const string UpdateStarGradeSoundName = "Fuse.mp3";

    public class CreateMaxStarInfo : Stone_BaseUserConfigData
    {
        public int Width;   //宽
        public int Height;  //高
        public float PieceDistanceArt;  //块表现距离
        public string PiecePrefabPath;  //块预制体路径
        public string[] StarPrefabPaths;//星星预制体路径
        public string SeletePrefabPath; //选择预制体路径
        public string ExplodePrefabPath;//爆炸效果预制体路径
        public string UpGradePrefabPath;//升级效果预制体路径
        public int MaxStarGrade;    //星星最大等级
        public int MixStarGrade;    //星星最小等级
        public int MaxExceedStarGrade;  //星星最大超出量
        public int[] InitGrades;        //初始化等级
        public int[] InitGradeCounts;   //初始化等级数量
        public int CreateStarWaitRoundValue;  //创建流星等待局数
        public int CreateStarMinCount;  //创建流星最小数量
        public int CreateStarMaxCount;  //创建流星最大数量
        public int CreateStarMinGrade;  //创建流星 最小等级
        public int CreateStarMaxGrade;  //创建流星 最小等级
        public int LockMaxCount;        //锁定最大值
        public float EffectSoundValue;  //音效大小

        public string GamePlayTip_Rule;         //提示：规则
        public string GamePlayTip_SeletePiece;  //提示：选择方块
        public string GamePlayTip_SelectDirection;  //提示：选择方向
        public string GamePlayTip_TerrainMove;  //提示：当前地形在移动
        public string GamePlayTip_Terrain;      //提示：地形移动
        public string GamePlayTip_ExceedStar;   //提示：超出上限提示
        public string GamePlayTip_DownGrade;    //提示：降级星星等级
        public string GamePlayTip_UpGrade;      //提示：升级星星等级
        public string GamePlayTip_ExplodeStar;  //提示：星球爆炸
        public string GamePlayTip_PoundStar;    //提示：星球冲击碎裂
        public string GamePlayTip_CreateStar;   //提示：创建流星
        public string GamePlayTip_AddLock;      //提示：添加锁定
        public string GamePlayTip_DeleteLock;   //提示：取消锁定
        public string GamePlayTip_LockStar;     //提示：锁定星星
        public string GamePlayTip_LockCount;    //提示：锁定回合
        public string GamePlayTip_Win;          //提示：胜利
    }

    private Transform m_Root;

    private CreateMaxStarInfo m_Info;

    private List<PieceControl> m_PieceList;
    private Dictionary<Vector3, PieceControl> m_PieceDict;

    private bool m_HasSelect;
    private bool m_PlayerMove;

    private Vector3 m_SelectPos;
    private Vector3 m_SelectDirection;

    private Transform m_SelectTransform;
    private GameObject m_SelectLeftGameObject;
    private GameObject m_SelectRightGameObject;
    private GameObject m_SelectUpGameObject;
    private GameObject m_SelectDownGameObject;

    private Vector3 m_AutoMoveDirection;
    private bool m_EnablePlayerControl;

    private List<Vector3> m_UpdateLogicPosList;
    private List<Vector3> m_MissUpdateLogicPosList;

    private int m_CreateStarWaitCount;
    private Dictionary<Vector3, int> m_CreateStarDict;

    private PieceControl m_LockPieceControl;
    private bool m_HasLock;
    private int m_LockCount;

    private int m_RunCount;

    private int m_UpdateTimer;

    private UpdateGamePlayTipEventInfo m_UpdateGamePlayTipInfo;
    private UpdateFixedTipEventInfo m_UpdateFixedTipInfo;
    private UpdateLockTipEventInfo m_UpdateLockTipInfo;

    private Sequence m_WinSequence;

    private Dictionary<string, Sequence> m_ExplodeSequenceDict;
    private Dictionary<string, Sequence> m_UpGradeSequenceDict;
    private Dictionary<string, Sequence> m_StarSequenceDict;

    private List<PieceControl> m_BeforeControls;

    private bool m_IsWin;

    private Stone_ResourceManager ResourceManager;
    private Stone_EventManager EventManager;
    private Stone_TimerManager TimerManager;
    private Stone_SoundManager SoundManager;

    public void Init(string configName)
    {
        Stone_UserConfigManager userConfigManager = Stone_RunTime.GetManager<Stone_UserConfigManager>(Stone_UserConfigManager.Name);
        m_Info = userConfigManager.GetConfig<CreateMaxStarInfo>(configName);

        m_PieceList = new List<PieceControl>();
        m_PieceDict = new Dictionary<Vector3, PieceControl>();
        m_UpdateLogicPosList = new List<Vector3>();
        m_MissUpdateLogicPosList = new List<Vector3>();
        m_CreateStarDict = new Dictionary<Vector3, int>();
        m_ExplodeSequenceDict = new Dictionary<string, Sequence>();
        m_UpGradeSequenceDict = new Dictionary<string, Sequence>();
        m_StarSequenceDict = new Dictionary<string, Sequence>();
        m_BeforeControls = new List<PieceControl>();

        m_EnablePlayerControl = true;
        m_HasSelect = false;
        m_PlayerMove = false;
        m_CreateStarWaitCount = 0;
        m_RunCount = 0;
        m_AutoMoveDirection = Vector3.right;
        m_IsWin = false;

        m_UpdateGamePlayTipInfo = new UpdateGamePlayTipEventInfo();
        m_UpdateFixedTipInfo = new UpdateFixedTipEventInfo();
        m_UpdateLockTipInfo = new UpdateLockTipEventInfo();

        ResourceManager = Stone_RunTime.GetManager<Stone_ResourceManager>(Stone_ResourceManager.Name);
        EventManager = Stone_RunTime.GetManager<Stone_EventManager>(Stone_EventManager.Name);
        TimerManager = Stone_RunTime.GetManager<Stone_TimerManager>(Stone_TimerManager.Name);
        SoundManager = Stone_RunTime.GetManager<Stone_SoundManager>(Stone_SoundManager.Name);

        GameObject root = new GameObject();
        root.name = "Terrain";
        m_Root = root.transform;

        CreateTerrain();

        SendGamePlayTip(m_Info.GamePlayTip_SeletePiece);
        SendFixedTip(m_Info.GamePlayTip_Rule);

        EventManager.AddListener<PlayerClickPieceEventInfo>(PlayerClickPieceEvent, this, PlayerClickPiece);
        EventManager.AddListener<PlayerClickSeleteEventInfo>(PlayerClickSeleteEvent, this, PlayerClickSelete);

        m_UpdateTimer = TimerManager.StarTimer(Update, interval: 0.5f);
    }

    public void UnInit()
    {
        EventManager.DeleteTargetAllListener(this);
        m_ExplodeSequenceDict.StopAllSequence();
        m_UpGradeSequenceDict.StopAllSequence();
        m_StarSequenceDict.StopAllSequence();

        for (int index = 0; index < m_PieceList.Count; index++)
        {
            m_PieceList[index].UnInit();
        }

        ResourceManager.DestroyGameObject(m_Root);

        TimerManager.StopTimer(m_UpdateTimer);

        ResourceManager = null;
        EventManager = null;
        TimerManager = null;
    }

    private void Win()
    {
        Stone_UIManager uiManager = Stone_RunTime.GetManager<Stone_UIManager>(Stone_UIManager.Name);

        SendGamePlayTip(string.Empty);
        SendFixedTip(string.Empty);

        SendLockTip(m_Info.GamePlayTip_Win);

        SoundManager.PlayBGM(WinSoundName);

        EventManager.Execute(GamePlayWinEvent);

        m_WinSequence = DOTween.Sequence();

        //等待
        m_WinSequence.AppendInterval(6f);

        //隐藏界面
        m_WinSequence.AppendCallback(() => {
            uiManager.DestroyUI(MainPlayPanel.Name);
            m_Root.gameObject.SetActive(false);
        });

        //显示胜利界面
        m_WinSequence.AppendCallback(() => {

            uiManager.OpenUI(WinPanel.Name);
        });
    }

    public void Update()
    {
        if(m_IsWin)
        {
            TimerManager.StopTimer(m_UpdateTimer);
            Win();
        }

        if (m_BeforeControls.Count > 0)
        {
            for (int index = 0; index < m_BeforeControls.Count; index++)
            {
                m_BeforeControls[index].HideControl();
            }
            m_BeforeControls.Clear();
        }

        if (m_EnablePlayerControl)
        {
            if(m_HasLock == false)
            {
                for(int index = 0;index<m_PieceList.Count;index++)
                {
                    PieceControl pieceControl = m_PieceList[index];
                    if(pieceControl.GetHasSart())
                    {
                        int starGrade = pieceControl.GetStarGrade();
                        if(starGrade>=m_Info.MaxStarGrade)
                        {
                            m_HasLock = true;
                            m_LockCount = -1;
                            m_LockPieceControl = pieceControl;
                            pieceControl.ShowLock();

                            SendFixedTip(m_Info.GamePlayTip_AddLock);
                            SendLockTip(m_Info.GamePlayTip_LockStar);

                            break;
                        }
                    }
                }
            }
            return;
        }

        if(m_PlayerMove)
        {
            m_PlayerMove = false;

            //玩家操作的移动
            StarMove(m_SelectPos, m_SelectDirection, 1);

            //添加整体移动
            AddTerrainMove();

            m_MissUpdateLogicPosList.Clear();
            m_CreateStarWaitCount += 1;
            return;
        }


        if (m_UpdateLogicPosList.Count == 0)
        {
            TerrainFinish();
            return;
        }

        SendGamePlayTip(m_Info.GamePlayTip_TerrainMove);

        while (m_UpdateLogicPosList.Count > 0)
        {
            Vector3 logicPos = m_UpdateLogicPosList[0];
            m_UpdateLogicPosList.Remove(logicPos);

            PieceControl pieceControl;
            if (!m_PieceDict.TryGetValue(logicPos, out pieceControl))
            {
                continue;
            }

            if (!pieceControl.GetHasSart())
            {
                continue;
            }

            StarMove(logicPos, m_AutoMoveDirection, -1);
            break;
        }

        for(int index = 0;index< m_MissUpdateLogicPosList.Count;index++)
        {
            m_UpdateLogicPosList.Remove(m_MissUpdateLogicPosList[index]);
        }
        m_MissUpdateLogicPosList.Clear();

        if (m_UpdateLogicPosList.Count == 0)
        {
            TerrainFinish();
        }
    }

    private void TerrainFinish()
    {
        m_EnablePlayerControl = true;
        m_MissUpdateLogicPosList.Clear();

        m_RunCount += 1;

        UpdateAutoMoveDirection();

        if (m_CreateStarWaitCount == m_Info.CreateStarWaitRoundValue - 1)
        {
            WillCreateMeteoricStream();
        }
        else if (m_CreateStarWaitCount == m_Info.CreateStarWaitRoundValue)
        {
            m_CreateStarWaitCount = 0;

            CreateMeteoricStream();
        }

        if (m_HasLock)
        {
            m_LockCount += 1;
            SendLockTip(string.Format(m_Info.GamePlayTip_LockCount, m_LockCount));

            if (m_LockCount == m_Info.LockMaxCount)
            {
                m_IsWin = true;
            }
        }
    }

    private void WillCreateMeteoricStream()
    {
        int createCount = Random.Range(m_Info.CreateStarMinCount, m_Info.CreateStarMaxCount);
        if(createCount == 0)
        {
            return;
        }

        for (int index = 0; index < createCount; index++)
        {
            Vector3 logicPos = new Vector3(Random.Range(0, m_Info.Width - 1), Random.Range(0, m_Info.Height - 1));
            if (m_CreateStarDict.ContainsKey(logicPos))
            {
                continue;
            }

            int starGrade = Random.Range(m_Info.CreateStarMinGrade, m_Info.CreateStarMaxGrade);

            PieceControl pieceControl;
            if (m_PieceDict.TryGetValue(logicPos, out pieceControl))
            {
                pieceControl.ShowWillCreateArt(starGrade);
            }
            m_CreateStarDict.Add(logicPos, starGrade);
        }

        SendFixedTip(m_Info.GamePlayTip_CreateStar);
    }

    private void CreateMeteoricStream()
    {
        Dictionary<Vector3, int>.Enumerator enumerator = m_CreateStarDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Vector3 logicPos = enumerator.Current.Key;
            int starGrade = enumerator.Current.Value;

            PieceControl pieceControl;
            if (m_PieceDict.TryGetValue(logicPos, out pieceControl))
            {
                pieceControl.HideWillCreateArt(starGrade);

                if (pieceControl.GetHasSart())
                {
                    StarAbsorb(starGrade, pieceControl);
                }
                else
                {
                    AddStarToPiece(logicPos, starGrade);
                }
            }
        }
        m_CreateStarDict.Clear();
    }

    private void CreateTerrain()
    {
        for (int i = 0; i < m_Info.Width; i++)
        {
            for (int j = 0; j < m_Info.Height; j++)
            {
                PieceControl pieceControl = new PieceControl();

                Vector3 logicPos = new Vector3(i, j, 0);
                Vector3 artPos = LogicPos2ArtPos(logicPos);

                pieceControl.Init();
                pieceControl.SetPosition(logicPos, artPos);

                GameObject pieceGameObject = ResourceManager.Instance(m_Info.PiecePrefabPath);
                pieceGameObject.tag = PieceTag;
                pieceGameObject.name = logicPos.x + "," + logicPos.y;
                pieceGameObject.transform.SetParent(m_Root);
                pieceGameObject.transform.localPosition = artPos;

                pieceControl.SetPieceGameObject(pieceGameObject);

                m_PieceList.Add(pieceControl);
                m_PieceDict.Add(logicPos, pieceControl);
            }
        }

        Dictionary<Vector3, int> logicPos2GradeDict = new Dictionary<Vector3, int>();
        for (int index = 0; index < m_Info.InitGrades.Length; index++)
        {
            int grade = m_Info.InitGrades[index];
            int count = m_Info.InitGradeCounts[index];

            for (int update = 0; update < count; update++)
            {
                Vector3 logicPos = new Vector3();
                do
                {
                    logicPos.x = Random.Range(0, m_Info.Width - 1);
                    logicPos.y = Random.Range(0, m_Info.Height - 1);
                } while (!(logicPos2GradeDict.ContainsKey(logicPos) == false));

                logicPos2GradeDict.Add(logicPos, grade);

                AddStarToPiece(logicPos, grade);
            }
        }
    }

    private void AddStarToPiece(Vector3 logicPos, int grade)
    {
        PieceControl pieceControl;
        if (!m_PieceDict.TryGetValue(logicPos, out pieceControl))
        {
            return;
        }

        if (pieceControl.GetHasSart())
        {
            return;
        }

        UpdateStarGrade(pieceControl, grade);

        //Vector3 artPos = pieceControl.GetArtPos();

        //GameObject starGameObject = ResourceManager.Instance(m_Info.StarPrefabPaths[grade - 1]);
        //starGameObject.name = "star grade:" + grade;
        //Transform starTransform = starGameObject.transform;
        //starTransform.SetParent(m_Root);
        //starTransform.localPosition = artPos;

        //pieceControl.AddStar(starGameObject, grade, 0);
    }

    private void PlayerClickPiece(PlayerClickPieceEventInfo info)
    {
        if (!m_EnablePlayerControl)
        {
            return;
        }

        Vector3 logicPos = info.logicPos;

        PieceControl pieceControl;
        if (!m_PieceDict.TryGetValue(logicPos, out pieceControl))
        {
            return;
        }

        if (!pieceControl.GetHasSart())
        {
            return;
        }

        m_HasSelect = true;
        m_SelectPos = logicPos;

        if (m_SelectTransform == null)
        {
            GameObject selectGameObject = ResourceManager.Instance(m_Info.SeletePrefabPath);
            m_SelectTransform = selectGameObject.transform;
            m_SelectTransform.SetParent(m_Root);

            m_SelectLeftGameObject = m_SelectTransform.Find("Directions/Left").gameObject;
            m_SelectRightGameObject = m_SelectTransform.Find("Directions/Right").gameObject;
            m_SelectUpGameObject = m_SelectTransform.Find("Directions/Up").gameObject;
            m_SelectDownGameObject = m_SelectTransform.Find("Directions/Down").gameObject;

            m_SelectLeftGameObject.tag = SelectLeftTag;
            m_SelectRightGameObject.tag = SelectRightTag;
            m_SelectUpGameObject.tag = SelectUpTag;
            m_SelectDownGameObject.tag = SelectDownTag;
        }
        m_SelectTransform.gameObject.SetActive(true);

        m_SelectTransform.localPosition = pieceControl.GetArtPos();
        m_SelectLeftGameObject.SetActive(logicPos.x != 0);
        m_SelectRightGameObject.SetActive(logicPos.x != m_Info.Width - 1);
        m_SelectUpGameObject.SetActive(logicPos.y != m_Info.Height - 1);
        m_SelectDownGameObject.SetActive(logicPos.y != 0);

        if (m_RunCount < 1)
        {
            SendGamePlayTip(m_Info.GamePlayTip_SelectDirection);
        }
    }

    private void PlayerClickSelete(PlayerClickSeleteEventInfo info)
    {
        if (!m_EnablePlayerControl)
        {
            return;
        }

        if (!m_HasSelect)
        {
            return;
        }

        m_SelectTransform.gameObject.SetActive(false);

        m_HasSelect = false;
        m_PlayerMove = true;
        m_EnablePlayerControl = false;

        m_SelectDirection = info.direction;
    }

    private void AddTerrainMove()
    {
        if (m_AutoMoveDirection == Vector3.right)
        {
            for (int i = m_Info.Height - 1; i >= 0; --i)
            {
                for (int j = 0; j < m_Info.Width; ++j)
                {
                    Vector3 logicPos = new Vector3(i, j, 0);
                    m_UpdateLogicPosList.Add(logicPos);
                }
            }
        }
        else if (m_AutoMoveDirection == Vector3.down)
        {
            for (int i = 0; i < m_Info.Width; i++)
            {
                for (int j = m_Info.Height - 1; j >= 0; --j)
                {
                    Vector3 logicPos = new Vector3(i, j, 0);
                    m_UpdateLogicPosList.Add(logicPos);
                }
            }
        }
        else if (m_AutoMoveDirection == Vector3.left)
        {
            for (int i = m_Info.Height - 1; i >= 0; --i)
            {
                for (int j = m_Info.Width - 1; j >= 0; --j)
                {
                    Vector3 logicPos = new Vector3(i, j, 0);
                    m_UpdateLogicPosList.Add(logicPos);
                }
            }
        }
        else if (m_AutoMoveDirection == Vector3.up)
        {
            for (int i = 0; i < m_Info.Width; i++)
            {
                for (int j = 0; j < m_Info.Height; ++j)
                {
                    Vector3 logicPos = new Vector3(i, j, 0);
                    m_UpdateLogicPosList.Add(logicPos);
                }
            }
        }
    }

    private void StarMove(Vector3 moveLogicPos, Vector3 moveDirection, int speed)
    {
        PieceControl movePiece;
        if (!m_PieceDict.TryGetValue(moveLogicPos, out movePiece))
        {
            return;
        }

        if (!movePiece.GetHasSart())
        {
            return;
        }

        Vector3 nextLogicPos;
        PieceControl nextPieceControl = null;

        if (speed == -1)
        {
            nextLogicPos = moveLogicPos + moveDirection;
            while (true)
            {
                if (nextLogicPos.x < 0 || nextLogicPos.x >= m_Info.Width || nextLogicPos.y < 0 || nextLogicPos.y >= m_Info.Height)
                {
                    break;
                }
                if (!m_PieceDict.TryGetValue(nextLogicPos, out nextPieceControl))
                {
                    break;
                }
                if (nextPieceControl.GetHasSart())
                {
                    break;
                }
                nextLogicPos += moveDirection;
            }
        }
        else
        {
            nextLogicPos = moveLogicPos + moveDirection * speed;
            m_PieceDict.TryGetValue(nextLogicPos, out nextPieceControl);
        }

        if (nextPieceControl == null)
        {
            return;
        }


        movePiece.ShowControl(nextLogicPos - moveLogicPos);
        m_BeforeControls.Add(movePiece);

        nextPieceControl.ShowControl();
        m_BeforeControls.Add(nextPieceControl);

        //进行碰撞
        if (nextPieceControl.GetHasSart())
        {
            int moveGrade = movePiece.GetStarGrade();
            int staticGrade = nextPieceControl.GetStarGrade();

            Vector3 powV3 = new Vector3(Mathf.Abs(moveLogicPos.x - nextLogicPos.x), Mathf.Abs(moveLogicPos.y - nextLogicPos.y));
            int pow = powV3.x > 0 ? (int)powV3.x : (int)powV3.y;
            pow -= 1;

            Vector3 moveNewLogicPos = nextLogicPos - (nextLogicPos - moveLogicPos).normalized;
            PieceControl moveNewPiece;
            m_PieceDict.TryGetValue(moveNewLogicPos, out moveNewPiece);


            //作用力 > 任一方格等级
            if (pow > moveGrade || pow > staticGrade)
            {
                if (m_HasLock && (
                    (movePiece == m_LockPieceControl && moveGrade < m_Info.MaxStarGrade) ||
                    (nextPieceControl == m_LockPieceControl && staticGrade < m_Info.MaxStarGrade)))
                {
                    m_HasLock = false;
                    m_LockPieceControl.HideLock();
                    m_LockPieceControl = null;

                    SendFixedTip(m_Info.GamePlayTip_DeleteLock);
                    SendLockTip(string.Empty);
                }

                moveGrade -= nextPieceControl.GetStarGrade();
                staticGrade -= movePiece.GetStarGrade();

                if (moveGrade > 0)
                {
                    if (moveNewPiece.GetHasSart())
                    {
                        StarAbsorb(moveGrade, moveNewPiece);
                    }
                    else
                    {
                        StarMoveToNextPiece(movePiece, moveNewPiece, moveGrade);
                        AddExplodeSequence(moveNewPiece);
                    }
                }

                if (m_HasLock && movePiece == m_LockPieceControl)
                {
                    if (moveGrade < m_Info.MaxStarGrade)
                    {
                        m_HasLock = false;
                        movePiece.HideLock();
                        m_LockPieceControl = null;

                        SendLockTip(string.Empty);
                    }
                    else
                    {
                        movePiece.HideLock();
                        m_LockPieceControl = moveNewPiece;
                        moveNewPiece.ShowLock();
                    }
                }

                AddExplodeSequence(nextPieceControl);

                StarExplode(movePiece);
                StarGradeChange(nextPieceControl, staticGrade);

                SendFixedTip(m_Info.GamePlayTip_DownGrade);
            }
            //作用力 < 两个方格的等级
            else
            {
                StarAbsorb(movePiece, nextPieceControl);
            }
        }
        //直接移动
        else
        {
            if (m_HasLock && movePiece == m_LockPieceControl)
            {
                movePiece.HideLock();

                m_LockPieceControl = nextPieceControl;
                nextPieceControl.ShowLock();
            }

            StarMoveToNextPiece(movePiece, nextPieceControl, movePiece.GetStarGrade());
        }

    }

    private void StarAbsorb(PieceControl movePiece, PieceControl staticPiece)
    {
        int newGrade = movePiece.GetStarGrade() + staticPiece.GetStarGrade();

        //超出最大等级
        if (newGrade >= m_Info.MaxStarGrade)
        {
            //超出超出额
            if (newGrade > m_Info.MaxExceedStarGrade)
            {
                Vector3 statcLogicPos = staticPiece.GetLogicPos();

                if (m_HasLock && (movePiece == m_LockPieceControl || staticPiece == m_LockPieceControl))
                {
                    m_HasLock = false;
                    m_LockPieceControl.HideLock();
                    m_LockPieceControl = null;

                    SendLockTip(string.Empty);
                }

                AddExplodeSequence(movePiece);
                StarExplode(movePiece);

                AddExplodeSequence(staticPiece);
                StarExplode(staticPiece);

                StarSplit(statcLogicPos, newGrade);
            }
            //未超出额外值
            else
            {
                if (m_HasLock && movePiece == m_LockPieceControl)
                {
                    movePiece.HideLock();
                    m_LockPieceControl = staticPiece;
                    staticPiece.ShowLock();
                }

                StarExplode(movePiece);
                UpdateStarGrade(staticPiece, newGrade);

                SendFixedTip(m_Info.GamePlayTip_ExceedStar);
            }
        }
        //未超出最大等级
        else
        {
            if (m_HasLock && movePiece == m_LockPieceControl)
            {
                movePiece.HideLock();
                m_LockPieceControl = staticPiece;
                staticPiece.ShowLock();
            }

            StarExplode(movePiece);
            UpdateStarGrade(staticPiece, newGrade);

            SendFixedTip(m_Info.GamePlayTip_UpGrade);
        }
    }

    private void StarAbsorb(int moveGrade, PieceControl staticPiece)
    {
        int staticGrade = staticPiece.GetStarGrade();

        staticGrade += moveGrade;

        //超出最大等级
        if (staticGrade >= m_Info.MaxStarGrade)
        {
            //超出超出额
            if (staticGrade > m_Info.MaxExceedStarGrade)
            {
                Vector3 maxLogicPos = staticPiece.GetLogicPos();

                if (m_HasLock && staticPiece == m_LockPieceControl)
                {
                    m_HasLock = false;
                    staticPiece.HideLock();
                    SendLockTip(string.Empty);
                }

                AddExplodeSequence(staticPiece);
                StarExplode(staticPiece);

                StarSplit(maxLogicPos, staticGrade);
            }
            //未超出额外值
            else
            {
                UpdateStarGrade(staticPiece, staticGrade);

                SendFixedTip(m_Info.GamePlayTip_ExceedStar);
            }
        }
        //未超出最大等级
        else
        {
            UpdateStarGrade(staticPiece, staticGrade);
        }
    }

    private void StarSplit(Vector3 logicPos, int starGrade)
    {
        int newStarGrade = starGrade / 4;

        Vector3[] sides = new Vector3[]
        {
            logicPos + Vector3.left,
            logicPos + Vector3.right,
            logicPos + Vector3.up,
            logicPos + Vector3.down,
        };

        for (int index = 0; index < sides.Length; index++)
        {
            Vector3 sideLogicPos = sides[index];
            PieceControl sidePieceControl;
            if (!m_PieceDict.TryGetValue(sideLogicPos, out sidePieceControl))
            {
                continue;
            }

            if (sidePieceControl.GetHasSart())
            {
                StarAbsorb(newStarGrade, sidePieceControl);
            }
            else
            {
                AddStarToPiece(sideLogicPos, newStarGrade);
            }
        }
    }

    private void StarGradeChange(PieceControl source, int newGrade)
    {
        if (newGrade <= 0)
        {
            AddExplodeSequence(source);
            StarExplode(source);

            SendFixedTip(m_Info.GamePlayTip_PoundStar);
        }
        else
        {
            UpdateStarGrade(source, newGrade);
        }
    }

    private void StarExplode(PieceControl source)
    {
        GameObject starGameObject = source.GetStarGameObject();

        source.DeleteStar();

        ResourceManager.DestroyGameObject(starGameObject);
    }

    private void UpdateStarGrade(PieceControl source, int starGrade)
    {
        Vector3 logicPos = source.GetLogicPos();
        Vector3 artPos = source.GetArtPos();

        m_MissUpdateLogicPosList.Add(logicPos);

        int curStarGrade = source.GetStarGrade();
        bool isUpdate = starGrade > curStarGrade;


        string name = "upgrade_" + logicPos.ToString();
        Sequence sequence = DOTween.Sequence();


        while (m_UpGradeSequenceDict.ContainsKey(name))
        {
            name += "_1";
        }

        m_UpGradeSequenceDict.Add(name, sequence);

        //加锁
        if (m_HasLock == false && starGrade >= m_Info.MaxStarGrade)
        {
            m_HasLock = true;
            m_LockCount = -1;
            m_LockPieceControl = source;
            m_LockPieceControl.ShowLock();

            SendFixedTip(m_Info.GamePlayTip_AddLock);
            SendLockTip(m_Info.GamePlayTip_LockStar);
        }

        if (isUpdate)
        {       
            //声音
            float volume = starGrade / (float)m_Info.MaxExceedStarGrade;
            SoundManager.PlayEffectSound(UpdateStarGradeSoundName, volume);

            float showTime = 0.15f;
            float rotationTime = 1f;
            float hideTime = 0.15f;
            Color hideColor = Color.white;
            hideColor.a = 0;

            //特效
            GameObject upGradeGameObject = ResourceManager.Instance(m_Info.UpGradePrefabPath);
            Transform upGradeTransform = upGradeGameObject.transform;
            upGradeTransform.SetParent(m_Root);
            upGradeTransform.localPosition = artPos;
            Transform animTransform = upGradeTransform.Find("Anim");

            Vector3 initotation = new Vector3(0, Random.Range(0, 360), 0);
            Vector3 rotation = new Vector3(0, Random.Range(180, 270), 0);

            //显示
            for (int index = 0; index < animTransform.childCount; index++)
            {
                Transform transform = animTransform.GetChild(index);
                Quaternion quaternion = transform.localRotation;
                quaternion.eulerAngles = initotation;
                transform.localRotation = quaternion;

                Material material = transform.GetComponent<MeshRenderer>().material;
                sequence.Insert(0, material.DOColor(Color.white, showTime));
                sequence.Insert(0, transform.DOLocalRotate(initotation + rotation, rotationTime));
            }


            sequence.InsertCallback(showTime, () =>
            {
                //销毁旧的
                GameObject starGameObject = source.GetStarGameObject();
                if (starGameObject != null)
                {
                    ResourceManager.DestroyGameObject(starGameObject);
                }

                //显示新的
                int prefabPathIndex = starGrade > m_Info.MaxStarGrade ? m_Info.MaxStarGrade : starGrade;
                GameObject newStarGameObjcet = ResourceManager.Instance(m_Info.StarPrefabPaths[prefabPathIndex - 1]);
                newStarGameObjcet.transform.SetParent(m_Root);
                newStarGameObjcet.transform.localPosition = source.GetArtPos();

                source.AddStar(newStarGameObjcet, starGrade, starGrade - m_Info.MaxStarGrade);
            });

            float showWaitTime = 0.5f;

            //隐藏特效
            for (int index = 0; index < animTransform.childCount; index++)
            {
                Material material = animTransform.GetChild(index).GetComponent<MeshRenderer>().material;
                sequence.Insert(showTime + showWaitTime, material.DOColor(hideColor, hideTime));
            }

            //等待音效播完
            float waitSoundTime = 2f;

            //销毁特效
            sequence.InsertCallback(showTime + hideTime + showWaitTime + waitSoundTime, () =>
              {
                  ResourceManager.DestroyGameObject(upGradeGameObject);
                  m_UpGradeSequenceDict.Remove(name);
              });
        }else
        {
            AddExplodeSequence(source);

            //销毁旧的
            GameObject starGameObject = source.GetStarGameObject();
            if (starGameObject != null)
            {
                ResourceManager.DestroyGameObject(starGameObject);
            }

            //显示新的
            int prefabPathIndex = starGrade > m_Info.MaxStarGrade ? m_Info.MaxStarGrade : starGrade;
            GameObject newStarGameObjcet = ResourceManager.Instance(m_Info.StarPrefabPaths[prefabPathIndex - 1]);
            newStarGameObjcet.transform.SetParent(m_Root);
            newStarGameObjcet.transform.localPosition = source.GetArtPos();

            source.AddStar(newStarGameObjcet, starGrade, starGrade - m_Info.MaxStarGrade);
        }
    }

    private void StarMoveToNextPiece(PieceControl source, PieceControl next,int starGrade)
    {
        GameObject starGameObject = source.GetStarGameObject();
        source.DeleteStar();

        string name = "UpdateStarGrade_" + source.GetLogicPos().ToString();
        Sequence sequence = DOTween.Sequence();
        m_StarSequenceDict.AddSequenceBySingleRun(name, sequence);

        Vector3 nextArtPos = next.GetArtPos();
        sequence.Append(starGameObject.transform.DOMove(nextArtPos, 0.2f));

        sequence.AppendCallback(() => {
            //销毁旧的
            if (starGameObject != null)
            {
                ResourceManager.DestroyGameObject(starGameObject);
            }

            //显示新的
            int prefabPathIndex = starGrade > m_Info.MaxStarGrade ? m_Info.MaxStarGrade : starGrade;
            GameObject newStarGameObjcet = ResourceManager.Instance(m_Info.StarPrefabPaths[prefabPathIndex - 1]);
            newStarGameObjcet.transform.SetParent(m_Root);
            newStarGameObjcet.transform.localPosition = next.GetArtPos();

            next.AddStar(newStarGameObjcet, starGrade, starGrade - m_Info.MaxStarGrade);
        });

    }


    private Vector3 LogicPos2ArtPos(Vector3 logicPos)
    {
        Vector3 artPos = logicPos * m_Info.PieceDistanceArt;
        return artPos;
    }

    private void UpdateAutoMoveDirection()
    {
        string directionName = string.Empty;
        if (m_AutoMoveDirection == Vector3.right)
        {
            m_AutoMoveDirection = Vector3.down;
            directionName = "向下";
        }
        else if (m_AutoMoveDirection == Vector3.down)
        {
            m_AutoMoveDirection = Vector3.left;
            directionName = "向左";
        }
        else if (m_AutoMoveDirection == Vector3.left)
        {
            m_AutoMoveDirection = Vector3.up;
            directionName = "向上";
        }
        else if (m_AutoMoveDirection == Vector3.up)
        {
            m_AutoMoveDirection = Vector3.right;
            directionName = "向右";
        }

        SendGamePlayTip(string.Format(m_Info.GamePlayTip_Terrain, directionName));
    }

    private void SendGamePlayTip(string tip)
    {
        m_UpdateGamePlayTipInfo.tip = tip;
        EventManager.Execute(UpdateGamePlayTipEvent, m_UpdateGamePlayTipInfo);
    }

    private void SendFixedTip(string tip)
    {
        m_UpdateFixedTipInfo.tip = tip;
        EventManager.Execute(UpdateFixedTipEvent, m_UpdateFixedTipInfo);
    }

    private void SendLockTip(string tip)
    {
        m_UpdateLockTipInfo.tip = tip;
        EventManager.Execute(UpdateLockTipEvent, m_UpdateLockTipInfo);
    }

    private void AddExplodeSequence(PieceControl pieceControl)
    {
        //声音
        int starGrade = pieceControl.GetStarGrade();
        float volume = starGrade / (float)m_Info.MaxExceedStarGrade;
        SoundManager.PlayEffectSound(ExplodeSoundName, volume);

        //特效
        Vector3 logicPos = pieceControl.GetLogicPos();
        Vector3 artPos = pieceControl.GetArtPos();

        GameObject explodeGameObject = ResourceManager.Instance(m_Info.ExplodePrefabPath);
        Transform explodeTransform = explodeGameObject.transform;
        explodeTransform.SetParent(m_Root);
        explodeTransform.localPosition = artPos;
        Transform animTransform = explodeTransform.Find("Anim");

        string name = "explode_" + logicPos.ToString();
        Sequence sequence = DOTween.Sequence();

        m_ExplodeSequenceDict.AddSequenceBySingleRun(name, sequence);

        float showTime = 0.05f;
        Color hideColor = Color.white;
        hideColor.a = 0;
        for (int index = 0; index < animTransform.childCount; index++)
        {
            Material material = animTransform.GetChild(index).GetComponent<MeshRenderer>().material;
            sequence.Append(material.DOColor(Color.white, showTime*2));
        }
        for (int index = 0; index < animTransform.childCount; index++)
        {
            Material material = animTransform.GetChild(index).GetComponent<MeshRenderer>().material;
            sequence.Append(material.DOColor(hideColor, showTime));
        }
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() =>
        {
            ResourceManager.DestroyGameObject(explodeGameObject);
        });
    }

}
