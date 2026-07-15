using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuEvents : MonoBehaviour
{
    public Transform vrCamera; 
    public float distanceFromPlayer = 1.2f; 
    public float heightOffset = -0.1f; 

    public GameObject soundMenuObject; 

    private UIDocument uiDocument;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        if (vrCamera == null && Camera.main != null)
        {
            vrCamera = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        PositionMenuInFrontOfPlayer();
        SetupUIButtons();
    }

    private void PositionMenuInFrontOfPlayer()
    {
        if (vrCamera == null) return;

        Vector3 forwardDirection = vrCamera.forward;
        forwardDirection.y = 0; 
        forwardDirection.Normalize();

        Vector3 targetPosition = vrCamera.position + (forwardDirection * distanceFromPlayer);
        targetPosition.y = vrCamera.position.y + heightOffset;

        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(forwardDirection);
    }

    private void SetupUIButtons()
    {
        if (uiDocument == null) return;
        var root = uiDocument.rootVisualElement;

        var resumeBtn = root.Q<Button>("ResumeButton");
        if (resumeBtn != null)
        {
            resumeBtn.clicked += ResumeGame;
        }

        var soundBtn = root.Q<Button>("SoundButton");
        if (soundBtn != null)
        {
            soundBtn.clicked += ShowSoundSettings;
        }

        var mainMenuBtn = root.Q<Button>("MainMenuButton");
        if (mainMenuBtn != null)
        {
            mainMenuBtn.clicked += GoToMainMenu;
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void ShowSoundSettings()
    {
        if (soundMenuObject != null)
        {
            soundMenuObject.transform.position = this.transform.position;
            soundMenuObject.transform.rotation = this.transform.rotation;
            soundMenuObject.transform.localScale = this.transform.localScale; 

            soundMenuObject.SetActive(true); 
            gameObject.SetActive(false); 
        }
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}