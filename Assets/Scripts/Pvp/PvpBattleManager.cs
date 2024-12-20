using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class PvpBattleManager : MonoBehaviour
{
    private static PvpBattleManager _instance = null;
    public static PvpBattleManager Instance => _instance;

    [SerializeField] private PvpUIScreen pvpUiScreen;
    [SerializeField] private PvpBattleLog uiBattleLog;
    [SerializeField] private PvpUIPlayerInformation uiPlayerInformation;
    [SerializeField] private Transform buttons;
    [SerializeField] private Maps map;
    [SerializeField] private Transform[] playerTrans;
    [SerializeField] private Transform[] opponentTrans;
    [SerializeField] private PvpUIOpponentInformation uiOpponentInformation;

    public PvpUIScreen PvpUiScreen => pvpUiScreen;
    public PvpBattleLog PvpUiBattleLog => uiBattleLog;
    public PvpUIPlayerInformation UiPlayerInformation => uiPlayerInformation;
    public PvpUIOpponentInformation UIOpponentInformation => uiOpponentInformation;

    private Animator playerAnimator;
    private Animator opponentAnimator;
    private Player myPlayer;
    [HideInInspector] public Player opponentPlayer = null;

    private int[] animCodeList = new[]
    {
        Constants.PlayerBattleAttack1,
        Constants.PlayerBattleDie,
        Constants.PlayerBattleHit
    };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        InitializePvp();
    }

    private void InitializePvp()
    {
        if (GameManager.Instance.Pvp != null)
        {
            Set(GameManager.Instance.Pvp);
            GameManager.Instance.Pvp = null;
        }
    }

    public void Set(S_PlayerMatchNotification pvp)
    {
        SetDungeon(pvp.DungeonCode);
        SetPlayerData(pvp.PlayerData);
        SetOpponentData(pvp.OpponentData);
        SetBattleLog(pvp.BattleLog);
    }

    private void SetPlayerData(PlayerStatus playerData)
    {
        if (playerData == null) return;

        uiPlayerInformation.Set(playerData);
        SetCharacter(playerData.PlayerClass, true);
    }

    private void SetOpponentData(PlayerStatus opponentData)
    {
        if (opponentData == null) return;

        uiOpponentInformation.Set(opponentData);
        SetCharacter(opponentData.PlayerClass, false);
        uiOpponentInformation.gameObject.SetActive(true);
    }

    private void SetBattleLog(BattleLog battleLog)
    {
        if (battleLog != null)
            uiBattleLog.Set(battleLog);
    }

    private void SetCharacter(int classCode, bool isPlayer)
    {
        int idx = classCode - Constants.PlayerCodeFactor;
        Transform[] trans = isPlayer ? playerTrans : opponentTrans;
        Animator animator = null;

        for (int i = 0; i < trans.Length; i++)
        {
            bool select = i == idx;
            trans[i].gameObject.SetActive(select);

            if (select)
            {
                animator = trans[i].GetComponent<Animator>();
                if (isPlayer)
                {
                    myPlayer = trans[i].GetComponent<Player>();
                }
                else
                {
                    opponentPlayer = trans[i].GetComponent<Player>();
                }
            }
        }

        if (isPlayer)
            playerAnimator = animator;
        else
            opponentAnimator = animator;
    }

    public void SetDungeon(int dungeonCode)
    {
        SetMap(dungeonCode);
    }

    public void SetMap(int id)
    {
        map.SetMap(id);
    }

    public void CheckUserTurn(bool userTurn)
    {
        foreach (Transform child in buttons)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
                button.interactable = userTurn;
        }
    }

    public void PlayerHit(bool isMyPlayer)
    {
        TriggerPlayerAction(Constants.PlayerBattleHit, isMyPlayer);
    }

    public void PlayerAnim(int idx, bool isMyPlayer)
    {
        if (idx < 0 || idx >= animCodeList.Length) return;

        int animCode = animCodeList[idx];
        TriggerPlayerAction(animCode, isMyPlayer);
    }

    private void TriggerPlayerAction(int code, bool isMyPlayer)
    {
        Animator animator = isMyPlayer ? playerAnimator : opponentAnimator;
        if (animator == null) return;

        SetAnimatorTransform(isMyPlayer, animator);
        animator.applyRootMotion = (code == Constants.PlayerBattleDie);
        animator.SetTrigger(code);
    }

    private void SetAnimatorTransform(bool isMyPlayer, Animator animator)
    {
        animator.transform.localEulerAngles = isMyPlayer ? Vector3.zero : new Vector3(0, 180, 0);
        animator.transform.localPosition = Vector3.zero;
    }
}
