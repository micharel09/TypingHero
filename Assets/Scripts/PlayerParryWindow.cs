using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryWindow : MonoBehaviour
{
    public Collider2D parryHitbox;      // kéo ParryHitbox (collider) vào đây
    public float parryDuration = 0.18f; // thời gian cửa sổ parry

    void Awake() { if (parryHitbox) parryHitbox.enabled = false; }

    public void OpenParryWindow(float duration = -1f)
    {
        StopAllCoroutines();
        StartCoroutine(CoParry(duration > 0 ? duration : parryDuration));
    }

    IEnumerator CoParry(float d)
    {
        if (!parryHitbox) yield break;
        parryHitbox.enabled = true;
        yield return new WaitForSeconds(d);
        parryHitbox.enabled = false;
    }

    // Khi đòn của boss chạm đúng lúc đang bật → coi như parry thành công
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu bạn gắn script này trực tiếp lên ParryHitbox, hàm này sẽ nhận trigger
        // Tag cho hitbox tấn công của boss là "EnemyAttack" (bước B7)
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log(">> PERFECT PARRY!");
        }
    }
}
