using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WaveStartButton : MonoBehaviour
{
    public WaveManager waveManager;

    public void OnPress()
    {
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }
    
    // For XR interaction events
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        OnPress();
    }
    
    public void OnActivated(ActivateEventArgs args)
    {
        OnPress();
    }
}