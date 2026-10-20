using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(300)]
public sealed class CombatR7Status : MonoBehaviour
{
    [SerializeField] TMP_Text enemyLabel, clock;
    [SerializeField] Image enemySocket, enemyIcon;
    [SerializeField] Sprite normalSocket, eliteSocket, bossSocket, normalIcon, eliteIcon, bossIcon;
    [SerializeField] TMP_Text playerHp, enemyHp, round;
    [SerializeField] Image comboFill;
    [SerializeField] Sprite normalFill, goldFill;
    PlayerHealth player;
    SkeletonController enemy;
    string opponent;
    string encounter;
    bool boss, elite;
    void Start()
    {
        player = FindObjectOfType<PlayerHealth>(); enemy = FindObjectOfType<SkeletonController>();
        var run = EndlessDuel.I ? EndlessDuel.I.Run : null;
        boss = run != null && run.Slot == 5; elite = run != null && run.Slot == 4;
        encounter = run == null ? "TUTORIAL" : $"{run.Difficulty.Label().ToUpperInvariant()} \u00b7 DUEL {run.Slot + 1}/6";
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
        round.text = encounter;
        round.enabled = !GamePauseController.InputBlocked && !CombatSession.Finished;
        comboFill.sprite = slayer ? goldFill : normalFill;
        float seconds = CombatSession.I ? CombatSession.I.Seconds : 0;
        clock.text = $"{(int)seconds / 60:00}:{(int)seconds % 60:00}";
    }
}

