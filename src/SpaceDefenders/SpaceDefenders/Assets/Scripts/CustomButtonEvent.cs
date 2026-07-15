using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CustomButtonEvent : MonoBehaviour
{
    public VisualTreeAsset mainMenuUI;
    public VisualTreeAsset optionsUI;

    // YENİ EKLENENLER: Tıklama sesi ve kaynağı
    public AudioClip clickSound;
    private AudioSource audioSource;

    private UIDocument uiDocument;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        audioSource = GetComponent<AudioSource>();
        
        ShowMainMenu();
    }


    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void ShowMainMenu()
    {
        uiDocument.visualTreeAsset = mainMenuUI;

        var root = uiDocument.rootVisualElement;


        var startBtn = root.Q<Button>("StartButton");
        if (startBtn != null)
        {
            startBtn.clicked += PlayClickSound; // Önce ses çalar
            startBtn.clicked += StartGame;      // Sonra sahneyi yükler
        }

        var optionsBtn = root.Q<Button>("OptionsButton");
        if (optionsBtn != null)
        {
            optionsBtn.clicked += PlayClickSound;
            optionsBtn.clicked += ShowOptions;
        }

        var exitBtn = root.Q<Button>("ExitButton");
        if (exitBtn != null)
        {
            exitBtn.clicked += PlayClickSound;
            exitBtn.clicked += ExitGame;
        }
    }

    private void ShowOptions()
    {
        uiDocument.visualTreeAsset = optionsUI;

        var root = uiDocument.rootVisualElement;

        var volumeSlider = root.Q<Slider>("VolumeSlider");

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;

            volumeSlider.RegisterValueChangedCallback(evt =>
            {
                AudioListener.volume = evt.newValue;
            });
        }

        var backBtn = root.Q<Button>("BackButton");
        if (backBtn != null)
        {
            backBtn.clicked += PlayClickSound;
            backBtn.clicked += ShowMainMenu;
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("VrRoom");
    }

    private void ExitGame()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}