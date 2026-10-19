using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class MainMenuUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button playButton;
    [SerializeField] Button tutorialButton;
    [SerializeField] EndlessDifficultySelector difficultySelector;
    [SerializeField] TextMeshProUGUI errorLabel;   // optional

    [Header("Flow")]
    [SerializeField] string gameSceneName = "Game";
    [Tooltip("Tự focus vào ô nhập khi vào menu.")]
    [SerializeField] bool autoFocusInput = true;

    [Header("Validation (expose để thống nhất với PlayerIdentity)")]
    [SerializeField, Range(3, 24)] int minLen = 3;
    [SerializeField, Range(3, 24)] int maxLen = 16;
    bool loadingScene;
    [SerializeField] HowToPlayPanel help;

    void Reset()
    {
        nameInput  = GetComponentInChildren<TMP_InputField>();
        playButton = GetComponentInChildren<Button>();
    }

    void Awake()
    {
        TutorialRun.End();
        EndlessRun.Clear();
        // Đảm bảo hệ nhận diện nạp sẵn tên lưu trước đó
        PlayerIdentity.EnsureLoaded();

        if (nameInput)
        {
            nameInput.text = PlayerIdentity.Name ?? string.Empty;
            nameInput.onValueChanged.AddListener(OnNameChanged);
            nameInput.onSubmit.AddListener(OnSubmit);   // TMP có onSubmit
        }

        if (playButton) playButton.onClick.AddListener(Play);

        UpdateValidationUI(nameInput ? nameInput.text : string.Empty);
    }

    void Start()
    {
        if (autoFocusInput && nameInput)
        {
            nameInput.Select();
            nameInput.ActivateInputField();
            nameInput.caretPosition = nameInput.text.Length;
        }
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab) || LeaderboardModal.BlocksShortcuts || (help && help.IsOpen)) return;
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (!events || !difficultySelector) return;
        var targets = new UnityEngine.UI.Selectable[] { nameInput, difficultySelector, playButton, tutorialButton };
        int current = System.Array.FindIndex(targets, item => item && item.gameObject == events.currentSelectedGameObject);
        int direction = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1 : 1;
        for (int i = 0; i < targets.Length; i++)
        {
            current = (current + direction + targets.Length) % targets.Length;
            if (targets[current] && targets[current].IsInteractable() && targets[current].gameObject.activeInHierarchy)
            { targets[current].Select(); break; }
        }
    }

    void OnDestroy()
    {
        if (nameInput)
        {
            nameInput.onValueChanged.RemoveListener(OnNameChanged);
            nameInput.onSubmit.RemoveListener(OnSubmit);
        }
        if (playButton) playButton.onClick.RemoveListener(Play);
    }

    void OnNameChanged(string _)
    {
        UpdateValidationUI(nameInput.text);
    }

    void OnSubmit(string _)
    {
        // Cho phép bấm Enter để chơi nếu hợp lệ
        if (playButton && playButton.interactable) Play();
    }

    void UpdateValidationUI(string raw)
    {
        string err;
        string _; // normalized
        bool ok = PlayerIdentity.Validate(raw, minLen, maxLen, out _, out err);

        if (playButton) playButton.interactable = ok;
        if (tutorialButton) tutorialButton.interactable = ok;
        if (errorLabel)
        {
            errorLabel.text = ok ? string.Empty : err;
            errorLabel.gameObject.SetActive(!ok && !string.IsNullOrEmpty(raw)); // ẩn khi chưa gõ gì
        }
    }

    void BeginGame() => SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    public void Tutorial() => Launch(true);

    void Play()
    {
        Launch(false);
    }

    void Launch(bool replay)
    {
        if (loadingScene || LeaderboardModal.BlocksShortcuts || (help && help.IsOpen)) return;
        if (!nameInput) return;

        if (!PlayerIdentity.TrySetName(nameInput.text, out string normalized, out string err))
        {
            UpdateValidationUI(nameInput.text);
            if (errorLabel) errorLabel.text = err;
            if (nameInput)
            {
                nameInput.Select();
                nameInput.ActivateInputField();
            }
            return;
        }

        // Đồng bộ lại text (đã được normalize)
        if (nameInput.text != normalized) nameInput.text = normalized;

        // TODO (sau này): Đẩy tên lên Firebase tại đây hoặc trong màn chơi đầu vào.
        // FirebaseRest.Instance.SetDisplayName(normalized); ...

        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("[MainMenuUI] Game Scene Name chưa được set.");
            return;
        }

        // Load gameplay
        loadingScene = true;
        if (replay || TutorialRun.Status == TutorialRun.Progress.Unseen) TutorialRun.Begin();
        else TutorialRun.End();
        if (!TutorialRun.Active) EndlessRun.Prepare();
        BeginGame();
    }
}
