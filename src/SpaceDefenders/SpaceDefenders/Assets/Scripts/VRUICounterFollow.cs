using UnityEngine;
using TMPro;

public class VRUICounterFollow : MonoBehaviour
{
    [Header("UI Elementleri")]
    public TextMeshProUGUI chipText; 

    [Header("VR Takip Ayarları")]
    public Transform vrCamera; 
    public Vector3 offset = new Vector3(0.3f, 0.2f, 1.0f); 
    public float smoothSpeed = 5f; 

    void Start()
    {
        if (vrCamera == null && Camera.main != null)
        {
            vrCamera = Camera.main.transform;
        }
        UpdateChipUI();
    }

    void LateUpdate()
    {
        if (vrCamera == null) return;

        Vector3 targetPosition = vrCamera.position + 
                                 (vrCamera.right * offset.x) + 
                                 (vrCamera.up * offset.y) + 
                                 (vrCamera.forward * offset.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(transform.position - vrCamera.position);

        UpdateChipUI();
    }

    private void UpdateChipUI()
    {
        if (chipText != null && ResourceManager.Instance != null)
        {
            chipText.text = ResourceManager.Instance.GetChips().ToString(); 
        }
    }
}