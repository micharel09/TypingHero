using UnityEngine;
using System.Collections;
using TMPro;

public class BossSimpleAttack : MonoBehaviour
{
    public Collider2D attackHitbox; // kéo AttackHitbox vào đây
    public float interval = 1.5f;   // nhịp ra đòn
    public float activeTime = 0.15f;
    public int damageToPlayer = 10;

    public PlayerParryWindow player; // kéo Player vào
    public TMP_Text debugHP;         // (tuỳ chọn) Text để hiển thị gì đó

    int bossHP = 100;

    void Start() { StartCoroutine(CoAttackLoop()); }
    public void TakeDamage(int dmg) { bossHP -= dmg; if (debugHP) debugHP.text = $"Boss HP: {bossHP}"; }

    public void PlayerFailedParry() { Debug.Log("Player miss parry!"); } // hook cho TypingManager

    IEnumerator CoAttackLoop()
    {
        while (true)
        {
            // "telegraph" đòn (nếu muốn, bạn có thể đổi màu sprite tại đây)
            yield return new WaitForSeconds(interval - activeTime);

            // kích hoạt hitbox trong thời gian rất ngắn
            attackHitbox.enabled = true;
            yield return new WaitForSeconds(activeTime);
            attackHitbox.enabled = false;
        }
    }

    // Nếu hitbox tấn công chạm Player body (và Player KHÔNG parry kịp) → trúng đòn
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Boss HIT Player!");
        }
    }
}
