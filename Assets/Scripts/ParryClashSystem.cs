using UnityEngine;
using System.Collections;

public class ParryClashSystem : MonoBehaviour
{
    [Header("Refs")]
    public SkeletonController boss;
    public PlayerAttackEvents playerAttack;
    public PlayerWeaponHitbox playerHitbox;

    [Header("Timing Leniency (seconds)")]
    [Tooltip("Cho phép player đã mở cửa sổ TRƯỚC strike tối đa từng này giây")]
    public float preLeniency = 0.035f;
    [Tooltip("Cho phép player mở cửa sổ SAU strike trong khoảng này giây (sẽ đợi post check)")]
    public float postLeniency = 0.035f;

    [Header("Clash Behaviour")]
    public float stunOnClash = 0.35f;
    public float maxClashDistance = 1.5f;

    [Header("Debug")]
    public bool logs = true;     // log khi clash thành công
    public bool verbose = false; // log từng bước vì sao không clash

    void OnEnable()
    {
        if (boss) boss.OnStrikeFrame += OnBossStrikeFrame;
    }
    void OnDisable()
    {
        if (boss) boss.OnStrikeFrame -= OnBossStrikeFrame;
    }

    void OnBossStrikeFrame()
    {
        if (!playerAttack || !playerHitbox || boss == null) return;

        if (verbose) Debug.Log("[CLASH] Strike frame received");

        // 1) Điều kiện không gian
        if (!SpatialConditionsOk(out string reasonSpatial))
        {
            if (verbose) Debug.Log($"[CLASH] NO spatial: {reasonSpatial}");
            return;
        }

        // 2) Điều kiện thời gian - ngay lập tức hoặc player đã mở cách đây <= preLeniency
        float now = Time.time;
        bool openNow = playerAttack.IsWindowOpen;
        bool openedRecently = (now - playerAttack.LastOpenTime) <= preLeniency;

        if (openNow || openedRecently)
        {
            DoClash("instant/openedRecently");
            return;
        }

        // 3) Nếu chưa mở, đợi thêm postLeniency xem player mở KHÁ SÁT sau strike không
        if (postLeniency > 0f && gameObject.activeInHierarchy)
            StartCoroutine(PostCheck(now));
        else if (verbose) Debug.Log("[CLASH] NO: player window not open and no postLeniency");
    }

    IEnumerator PostCheck(float strikeTime)
    {
        float wait = Mathf.Max(0.001f, postLeniency);
        yield return new WaitForSeconds(wait);

        // sau chờ: vẫn phải pass spatial (phòng trường hợp đã rời)
        if (!SpatialConditionsOk(out _))
        {
            if (verbose) Debug.Log("[CLASH] PostCheck NO: spatial failed after wait");
            yield break;
        }

        // player đã mở trong khoảng postLeniency?
        if (playerAttack.IsWindowOpen || (playerAttack.LastOpenTime - strikeTime) <= postLeniency)
        {
            DoClash("postLeniency");
        }
        else if (verbose) Debug.Log("[CLASH] PostCheck NO: player didn't open in time");
    }

    bool SpatialConditionsOk(out string reason)
    {
        reason = "";
        if (!playerHitbox.Active && !playerAttack.IsWindowOpen)
        { reason = "player window closed"; return false; }

        if (boss.hurtbox && playerHitbox.Collider)
        {
            bool overlap = playerHitbox.Collider.bounds.Intersects(boss.hurtbox.bounds);
            if (!overlap) { reason = "no bounds overlap"; return false; }
        }

        if (playerHitbox.HasDealtDamageThisSwing)
        { reason = "already dealt damage this swing"; return false; }

        float dist = Vector2.Distance(playerHitbox.transform.position, boss.transform.position);
        if (dist > maxClashDistance)
        { reason = $"dist {dist:F2} > {maxClashDistance:F2}"; return false; }

        return true;
    }

    void DoClash(string source)
    {
        boss.CancelCurrentStrikeAndStun(stunOnClash);
        playerHitbox.SuppressNextDamage();

        if (HitStopper.I)
        {
            HitStopper.I.StopEnemyHit();
            HitStopper.I.StopPlayerHit();
        }

        if (logs) Debug.Log($"[CLASH] OK via {source}, stun {stunOnClash:F2}s");
    }
}
