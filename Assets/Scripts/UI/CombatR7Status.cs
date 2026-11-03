using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(300)]
public sealed class CombatR7Status : MonoBehaviour
{
    [SerializeField] TMP_Text enemyLabel, clock;
    [SerializeField] Image enemySocket, enemyIcon;
    [SerializeField] Sprite normalSocket, eliteSocket, bossSocket, normalIcon, eliteIcon, bossIcon;
    [SerializeField] TMP_Text playerHp, enemyHp;
    [SerializeField] Image comboFill;
    [SerializeField] Sprite normalFill, goldFill;
    [SerializeField] Slider semiSlayerCharge;
    [SerializeField] Image semiSlayerFill, semiSlayerFrame, semiSlayerSocket;
    [SerializeField] TMP_Text semiSlayerKey;
    [SerializeField] Sprite semiFrame, goldFrame, semiSocket, goldSocket, semiFill, goldSemiFill;
    PlayerHealth player;
    SkeletonController enemy;
    SemiSlayerMode semiSlayer;
    string opponent;
    bool boss, elite;
    internal TMP_Text IntroEnemyLabel => enemyLabel;
    void Start()
    {
        player = FindObjectOfType<PlayerHealth>(); enemy = FindObjectOfType<SkeletonController>();
        semiSlayer = FindObjectOfType<SemiSlayerMode>();
        var run = EndlessDuel.I ? EndlessDuel.I.Run : null;
        boss = run != null && run.Slot == 5; elite = run != null && run.Slot == 4;
        opponent = run == null ? "SKELETON" : run.Enemy.Name.ToUpperInvariant();
    }
    void LateUpdate()
    {
        enemyLabel.text = opponent;
        enemySocket.sprite = boss ? bossSocket : elite ? eliteSocket : normalSocket;
        var rt = enemySocket.rectTransform;
        rt.anchoredPosition = new Vector2(0, boss || elite ? 9 : 0);
        rt.sizeDelta = new Vector2(63, boss || elite ? 72 : 63);
        enemyIcon.sprite = boss ? bossIcon : elite ? eliteIcon : normalIcon;
        var iconSize = enemyIcon.sprite.rect.size;
        enemyIcon.rectTransform.sizeDelta = iconSize * 3;
        enemyIcon.rectTransform.anchoredPosition = new Vector2(Mathf.Floor((20 - iconSize.x) / 2) * 3,
            -Mathf.Floor((19 - iconSize.y) / 2) * 3);
        if (player) playerHp.text = $"{player.Current}/{player.maxHealth}";
        if (enemy) enemyHp.text = $"{enemy.Current}/{enemy.maxHealth}";
        bool slayer = TypingManager.MuteWordCorrect;
        comboFill.sprite = slayer ? goldFill : normalFill;
        if (!semiSlayer) semiSlayer = FindObjectOfType<SemiSlayerMode>();
        bool showSemiSlayer = semiSlayer && EndlessRun.Current != null && !TutorialRun.Active;
        bool charged = showSemiSlayer && semiSlayer.Charge >= 100;
        if (semiSlayerCharge)
        {
            semiSlayerCharge.gameObject.SetActive(showSemiSlayer);
            semiSlayerCharge.SetValueWithoutNotify(showSemiSlayer ? semiSlayer.Charge01 : 0f);
        }
        if (semiSlayerFrame)
        {
            semiSlayerFrame.gameObject.SetActive(showSemiSlayer);
            semiSlayerFrame.sprite = charged ? goldFrame : semiFrame;
        }
        if (semiSlayerSocket)
        {
            semiSlayerSocket.gameObject.SetActive(showSemiSlayer);
            semiSlayerSocket.sprite = charged ? goldSocket : semiSocket;
        }
        if (semiSlayerFill) semiSlayerFill.sprite = charged ? goldSemiFill : semiFill;
        if (semiSlayerKey)
        {
            semiSlayerKey.gameObject.SetActive(showSemiSlayer);
            semiSlayerKey.color = charged ? new Color(0.9f, 0.77f, 0.49f, 1f) : Color.white;
        }
        float seconds = CombatSession.I ? CombatSession.I.Seconds : 0;
        clock.text = $"{(int)seconds / 60:00}:{(int)seconds % 60:00}";
    }
}

