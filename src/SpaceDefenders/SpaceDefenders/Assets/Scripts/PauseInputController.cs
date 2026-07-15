using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class PauseInputController : MonoBehaviour
{
    public GameObject pauseMenuObject;
    private bool isButtonHandled = false;

    void Update()
    {
        bool secondaryKeyPressed = CheckSecondaryButton(XRNode.LeftHand) || CheckSecondaryButton(XRNode.RightHand);

        if (Input.GetKeyDown(KeyCode.Escape) || secondaryKeyPressed)
        {
            if (!isButtonHandled)
            {
                TogglePause();
                isButtonHandled = true;
            }
        }
        else
        {
            isButtonHandled = false;
        }
    }

    private bool CheckSecondaryButton(XRNode node)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
            {
                return isPressed;
            }
        }
        return false;
    }

    private void TogglePause()
    {
        if (pauseMenuObject == null) return;

        bool isCurrentlyActive = pauseMenuObject.activeSelf;

        if (!isCurrentlyActive)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        pauseMenuObject.SetActive(!isCurrentlyActive);
    }
}