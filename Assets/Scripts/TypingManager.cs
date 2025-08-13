using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI wordText;     // KÉO BossWordText (Text TMP) vào đây
    public TMP_InputField inputField;     // (tuỳ chọn) KÉO PlayerInput vào đây nếu bạn dùng InputField
    public UnityEngine.UI.Slider countdownSlider;  // KÉO CountdownSlider vào đây
    public BossSimpleAttack boss;         // (để trống lúc này)
    public Animator playerAnimator;

    [Header("Timing Settings")]
    [Range(0.8f, 3.0f)]
    public float attackTimeLimit = 2.2f;  // Tăng từ 1.6f lên 2.2f

    string[] attackWords = { "slash", "strike", "cut", "stab" };
    string target = "";
    int index = 0;
    float timer = 0f;

    void Start()
    {
        NextPrompt();
        FocusInput();
    }

    void Update()
    {
        // đếm ngược thời gian cho chữ hiện tại
        timer -= Time.deltaTime;

        // Cập nhật countdown slider
        if (countdownSlider)
        {
            float timeRatio = timer / attackTimeLimit; // tỉ lệ thời gian còn lại
            countdownSlider.value = timeRatio;

            // Đổi màu slider theo thời gian còn lại
            Image fillImage = countdownSlider.fillRect.GetComponent<Image>();
            if (fillImage)
            {
                if (timeRatio > 0.6f) fillImage.color = Color.green;  // Xanh lá: nhiều thời gian
                else if (timeRatio > 0.3f) fillImage.color = Color.yellow; // Vàng: cảnh báo
                else fillImage.color = Color.red;    // Đỏ: gấp rút!
            }
        }

        if (timer <= 0f)
        {
            Debug.Log("MISS (timeout)");
            NextPrompt();
            FocusInput();
        }

        // bắt ký tự gõ
        foreach (char c in Input.inputString)
        {
            // bỏ qua phím không phải ký tự (Enter, Shift, Backspace...)
            if (!char.IsLetter(c)) continue;

            if (index < target.Length && char.ToLowerInvariant(c) == char.ToLowerInvariant(target[index]))
            {
                index++;
                // tô màu phần đã gõ
                wordText.text = $"<color=#7CFC00>{target.Substring(0, index)}</color>{target.Substring(index)}";

                if (index >= target.Length)
                {
                    Debug.Log("ATTACK OK");
                    if (playerAnimator) playerAnimator.SetTrigger("Attack");
                    if (boss) boss.TakeDamage(10);
                    NextPrompt();
                    FocusInput();
                }
            }
        }
    }

    void NextPrompt()
    {
        target = attackWords[Random.Range(0, attackWords.Length)];
        index = 0;
        timer = attackTimeLimit; // Sử dụng giá trị từ Inspector

        if (wordText) wordText.text = target;
        if (inputField) inputField.text = ""; // ô nhập chỉ hiển thị những gì bạn gõ
    }

    void FocusInput()
    {
        if (inputField)
        {
            inputField.ActivateInputField();
            inputField.caretPosition = inputField.text.Length;
        }
    }
}