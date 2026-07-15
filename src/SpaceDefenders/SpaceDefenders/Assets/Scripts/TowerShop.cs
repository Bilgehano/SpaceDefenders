using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TowerShop : MonoBehaviour
{
    public GameObject towerPrefab;
    private XRBaseInteractable m_Interactable;
    private XRInteractionManager m_InteractionManager;

    void Awake()
    {
        m_Interactable = GetComponent<XRBaseInteractable>();
        m_InteractionManager = Object.FindAnyObjectByType<XRInteractionManager>();
    }

    void OnEnable()
    {
        if (m_Interactable != null)
        {
            m_Interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void OnDisable()
    {
        if (m_Interactable != null)
        {
            m_Interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (towerPrefab == null || m_InteractionManager == null) return;

        // Use the interactor's attach transform for spawning to keep it in the hand
        Transform attachTransform = args.interactorObject.GetAttachTransform(m_Interactable);
        Vector3 spawnPos = attachTransform != null ? attachTransform.position : args.interactorObject.transform.position;
        Quaternion spawnRot = attachTransform != null ? attachTransform.rotation : Quaternion.identity;

        GameObject newTower = Instantiate(towerPrefab, spawnPos, spawnRot);
        XRGrabInteractable grab = newTower.GetComponent<XRGrabInteractable>();
        
        if (grab != null)
        {
            // Explicitly exit the shop selection to free up the interactor
            m_InteractionManager.SelectExit(args.interactorObject, m_Interactable);
            
            // Hand off the grab to the new tower
            m_InteractionManager.SelectEnter(args.interactorObject, grab);
            
            // Briefly disable the shop to avoid immediate re-grab or interference
            StartCoroutine(DisableShopBriefly());
        }
    }

    private System.Collections.IEnumerator DisableShopBriefly()
    {
        m_Interactable.enabled = false;
        yield return new WaitForSeconds(0.5f);
        m_Interactable.enabled = true;
    }
}
