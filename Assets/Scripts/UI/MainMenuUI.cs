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

    public WorldDefinition worldDefinition;

    [Header("Flow")]
    [SerializeField] string gameSceneName = "Game";
    [Tooltip("Tự focus vào ô nhập khi vào menu.")]
    [SerializeField] bool autoFocusInput = true;

    bool loadingScene;
    [SerializeField] GameObject preparation;
    [SerializeField] CanvasGroup menuActions;
    [SerializeField] Button[] menuButtons;
    [SerializeField] Button backButton;
    [SerializeField] TMP_Text preparationTitle;
    bool replayTutorial;
    int transitionFrame = -1;
    Button opener;
    int lastMenuIndex;

    internal RectTransform IntroMenuRoot => menuActions ? menuActions.transform as RectTransform : null;

    bool Blocked => LeaderboardModal.BlocksShortcuts || SettingsModal.BlocksShortcuts;

    public void OpenPlay() => OpenPreparation(false);
    public void ClosePreparation()
    {
        if (!preparation || !preparation.activeSelf || transitionFrame == Time.frameCount) return;
        transitionFrame = Time.frameCount;
        preparation.SetActive(false);
        menuActions.interactable = menuActions.blocksRaycasts = true;
        if (opener) opener.Select();
    }
    void OpenPreparation(bool tutorial)
    {
        if (Blocked || loadingScene || transitionFrame == Time.frameCount || preparation.activeSelf) return;
        transitionFrame = Time.frameCount;
        replayTutorial = tutorial;
        opener = menuButtons[tutorial ? 1 : 0];
        preparationTitle.text = tutorial ? "Tutorial" : "Play";
        menuActions.interactable = menuActions.blocksRaycasts = false;
        preparation.SetActive(true);
        UpdateValidationUI(nameInput.text);
        nameInput.Select(); nameInput.ActivateInputField();
    }

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
            // Preserve legacy saved names for correction without silently truncating them.
            nameInput.characterLimit = 0;
            nameInput.text = PlayerIdentity.Name ?? string.Empty;
            nameInput.characterLimit = PlayerIdentity.MaxNameLength;
            nameInput.restoreOriginalTextOnEscape = false;
            nameInput.onValueChanged.AddListener(OnNameChanged);
            nameInput.onSubmit.AddListener(OnSubmit);   // TMP có onSubmit
        }

        if (playButton) playButton.onClick.AddListener(Play);

        UpdateValidationUI(nameInput ? nameInput.text : string.Empty);
    }

    System.Collections.IEnumerator Start()
    {
        loadingScene = true;
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (events) events.enabled = false;
        var session = DungeonSession.I;
        yield return session.LoadWorld(session.Definition ? session.Definition : worldDefinition);
        if (events) events.enabled = true;
        loadingScene = false;
        if (preparation) { preparation.SetActive(false); menuButtons[0].Select(); yield break; }
        if (autoFocusInput && nameInput)
        {
            nameInput.Select();
            nameInput.ActivateInputField();
            nameInput.caretPosition = nameInput.text.Length;
        }
    }

    void Update()
    {
        if (preparation)
        {
            if (Blocked || loadingScene || transitionFrame == Time.frameCount) return;
            if (preparation.activeSelf && Input.GetKeyDown(KeyCode.Escape)) { ClosePreparation(); return; }
            var system = UnityEngine.EventSystems.EventSystem.current;
            if (!system) return;
            if (!preparation.activeSelf)
            {
                int selected = System.Array.FindIndex(menuButtons, b => b.gameObject == system.currentSelectedGameObject);
                if (selected >= 0) lastMenuIndex = selected;
                else selected = lastMenuIndex;
                int step = Input.GetKeyDown(KeyCode.UpArrow) ? -1 : Input.GetKeyDown(KeyCode.DownArrow) ? 1 :
                    Input.GetKeyDown(KeyCode.Tab) ? (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1 : 1) : 0;
                if (step != 0) menuButtons[(selected + step + menuButtons.Length) % menuButtons.Length].Select();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                var cardTargets = new Selectable[] { nameInput, difficultySelector, backButton, playButton };
                int selected = System.Array.FindIndex(cardTargets, b => b && b.gameObject == system.currentSelectedGameObject);
                int step = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1 : 1;
                for (int i = 0; i < cardTargets.Length; i++)
                {
                    selected = (selected + step + cardTargets.Length) % cardTargets.Length;
                    if (cardTargets[selected] && cardTargets[selected].IsInteractable()) { cardTargets[selected].Select(); break; }
                }
            }
            return;
        }
        if (!Input.GetKeyDown(KeyCode.Tab) || (LeaderboardModal.BlocksShortcuts || SettingsModal.BlocksShortcuts)) return;
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

    void LateUpdate()
    {
        if (!preparation || preparation.activeSelf || Blocked || loadingScene || !menuActions || !menuActions.interactable) return;
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (events)
        {
            int selected = System.Array.FindIndex(menuButtons, b => b && b.gameObject == events.currentSelectedGameObject);
            if (selected >= 0) lastMenuIndex = selected;
            else if (!events.currentSelectedGameObject) menuButtons[lastMenuIndex].Select();
        }
        foreach (var button in menuButtons)
        {
            bool focused = events && events.currentSelectedGameObject == button.gameObject;
            bool hovered = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)button.transform, Input.mousePosition);
            var label = button.GetComponentInChildren<TMP_Text>();
            label.color = focused || hovered ? new Color32(220, 193, 138, 255) : new Color32(242, 231, 207, 255);
            var marker = button.transform.Find("Selection");
            if (marker) marker.gameObject.SetActive(focused || hovered);
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
        if (preparation && (!preparation.activeSelf || transitionFrame == Time.frameCount)) return;
        // Cho phép bấm Enter để chơi nếu hợp lệ
        if (playButton && playButton.interactable) Play();
    }

    static bool ValidateName(string raw, out string normalized, out string error)
    {
        if (raw != null && raw.Length > PlayerIdentity.MaxNameLength)
        {
            normalized = null;
            error = $"Name is too long (≤ {PlayerIdentity.MaxNameLength}).";
            return false;
        }
        return PlayerIdentity.Validate(raw, PlayerIdentity.MinNameLength, PlayerIdentity.MaxNameLength, out normalized, out error);
    }

    void UpdateValidationUI(string raw)
    {
        string err;
        string _; // normalized
        bool ok = ValidateName(raw, out _, out err);

        if (playButton) playButton.interactable = ok;
        if (tutorialButton) tutorialButton.interactable = preparation || ok;
        if (errorLabel)
        {
            errorLabel.text = ok ? string.Empty : err;
            errorLabel.gameObject.SetActive(!ok && !string.IsNullOrEmpty(raw)); // ẩn khi chưa gõ gì
        }
    }

    void BeginGame() => DungeonSession.I.BeginGame(gameSceneName);
    public void Tutorial() { if (preparation) OpenPreparation(true); else Launch(true); }

    void Play()
    {
        if (preparation && (!preparation.activeSelf || transitionFrame == Time.frameCount)) return;
        Launch(preparation ? replayTutorial : false);
    }

    void Launch(bool replay)
    {
        if (loadingScene || (LeaderboardModal.BlocksShortcuts || SettingsModal.BlocksShortcuts)) return;
        if (!nameInput) return;

        if (!ValidateName(nameInput.text, out _, out string err) ||
            !PlayerIdentity.TrySetName(nameInput.text, out _, out err))
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
        string normalized = PlayerIdentity.Name;
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
