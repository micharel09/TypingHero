using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI wordText;
    public TMP_InputField inputField;
    public Slider countdownSlider;

    [Header("Timing Settings")]
    [Range(0.8f, 3.0f)]
    public float attackTimeLimit = 2.2f;

    [Header("Word Settings")]
    public string[] attackWords = { "slash", "strike", "cut", "stab" };
    public static bool MuteWordCorrect = false;

    string target = "";
    int index = 0;
    float timer = 0f;

    // Sự kiện để thông báo ra ngoài
    public static event Action OnWordCorrect;
    public static event Action OnWordTimeout;
    public static event System.Action OnWordAdvanced;

    // NEW: mỗi ký tự đúng
    public static event Action<char> OnCorrectChar;

    public ParrySystem parry;

    void Start()
    {
        NextPrompt();
        FocusInput();
    }

    void Update()
    {
        HandleTimer();
        HandleTyping();
    }

    void HandleTimer()
    {
        timer -= Time.deltaTime;

        if (countdownSlider)
        {
            float timeRatio = timer / attackTimeLimit;
            countdownSlider.value = timeRatio;

            var fillImage = countdownSlider.fillRect.GetComponent<Image>();
            if (fillImage)
            {
                if (timeRatio > 0.6f) fillImage.color = Color.green;
                else if (timeRatio > 0.3f) fillImage.color = Color.yellow;
                else fillImage.color = Color.red;
            }
        }

        if (timer <= 0f)
        {
            Debug.Log("MISS (timeout)");
            OnWordTimeout?.Invoke();
            NextPrompt();
            FocusInput();
        }
    }

    void HandleTyping()
    {
        foreach (char c in Input.inputString)
        {
            if (!char.IsLetter(c)) continue;

            if (index < target.Length && char.ToLowerInvariant(c) == char.ToLowerInvariant(target[index]))
            {
                index++;

                // NEW: phát sự kiện ký tự đúng cho Slayer
                OnCorrectChar?.Invoke(c);

                if (wordText)
                    wordText.text = $"<color=#7CFC00>{target.Substring(0, index)}</color>{target.Substring(index)}";

                if (index >= target.Length)
                {
                    if (!MuteWordCorrect) OnWordCorrect?.Invoke();
                    OnWordAdvanced?.Invoke();

                    NextPrompt();
                    FocusInput();
                }

            }
        }
    }

    void NextPrompt()
    {
        target = attackWords[UnityEngine.Random.Range(0, attackWords.Length)];
        index = 0;
        timer = attackTimeLimit;

        if (wordText) wordText.text = target;
        if (inputField) inputField.text = "";
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
