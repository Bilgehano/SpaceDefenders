using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SoundMenuController : MonoBehaviour
{
    public GameObject pauseMenuObject; 
    private UIDocument uiDocument;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        var backBtn = root.Q<Button>("BackButton");
        if (backBtn != null)
        {
            backBtn.clicked += GoBackToPauseMenu;
        }

        var volumeSlider = root.Q<Slider>("VolumeSlider");
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.RegisterValueChangedCallback(evt => {
                AudioListener.volume = evt.newValue;
            });
        }
    }

    private void GoBackToPauseMenu()
    {
        if (pauseMenuObject != null)
        {
            pauseMenuObject.SetActive(true); 
            gameObject.SetActive(false); 
        }
    }
}