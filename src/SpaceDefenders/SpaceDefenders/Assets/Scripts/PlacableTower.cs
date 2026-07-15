using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlacableTower : MonoBehaviour
{
    public Material validMaterial;
    public Material invalidMaterial;
    public LayerMask obstacleLayer;
    public int cost = 10;

    // YENİ EKLENENLER: Yerleştirme sesi değişkenleri
    public AudioClip placeSound;
    private AudioSource audioSource;

    private XRGrabInteractable m_GrabInteractable;
    private Tower m_TowerScript;
    private Renderer[] m_Renderers;
    private System.Collections.Generic.Dictionary<Renderer, Material[]> m_OriginalMaterialsMap = new System.Collections.Generic.Dictionary<Renderer, Material[]>();
    private bool m_IsValid = false;
    private bool m_IsPlaced = false;
    private IXRSelectInteractor m_CurrentInteractor;

    void Awake()
    {
        m_GrabInteractable = GetComponent<XRGrabInteractable>();
        m_TowerScript = GetComponent<Tower>();

        audioSource = GetComponent<AudioSource>();
        
        m_Renderers = GetComponentsInChildren<Renderer>(true);
        CaptureOriginalMaterials();

        if (m_TowerScript != null) m_TowerScript.enabled = false;
        
        if (m_GrabInteractable != null)
        {
            m_GrabInteractable.selectEntered.AddListener(OnSelectEntered);
            m_GrabInteractable.selectExited.AddListener(OnSelectExited);
        }

        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null) sphere.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void CaptureOriginalMaterials()
    {
        foreach (var renderer in m_Renderers)
        {
            if (renderer != null && !m_OriginalMaterialsMap.ContainsKey(renderer))
            {
                m_OriginalMaterialsMap[renderer] = renderer.sharedMaterials;
            }
        }
    }

    void OnDestroy()
    {
        if (m_GrabInteractable != null)
        {
            m_GrabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            m_GrabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private bool m_LastValidity = true;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (m_IsPlaced) return;

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        CheckValidity();
        
        if (m_IsValid != m_LastValidity || m_Renderers == null || m_Renderers.Length == 0)
        {
            UpdateVisuals();
            m_LastValidity = m_IsValid;
        }
    }

    void UpdateVisuals()
    {
        if (m_Renderers == null || m_Renderers.Length == 0)
        {
            m_Renderers = GetComponentsInChildren<Renderer>(true);
            if (m_Renderers.Length == 0) return;
        }

        CaptureOriginalMaterials();

        Material matToUse = m_IsValid ? validMaterial : invalidMaterial;
        if (matToUse == null) return;

        foreach (var renderer in m_Renderers)
        {
            if (renderer != null)
            {
                Material[] mats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = matToUse;
                }
                renderer.sharedMaterials = mats;
            }
        }
    }

    private System.Collections.Generic.List<Collider> m_CollidingObjects = new System.Collections.Generic.List<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (m_IsPlaced) return;
        if (other.gameObject == gameObject) return;
        if (other.isTrigger) return;
        if (other.CompareTag("Floor") || other.name.ToLower().Contains("floor")) return;
        
        if (((1 << other.gameObject.layer) & obstacleLayer.value) != 0)
        {
            if (!m_CollidingObjects.Contains(other))
                m_CollidingObjects.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (m_CollidingObjects.Contains(other))
            m_CollidingObjects.Remove(other);
    }

    void CheckValidity()
    {
        if (ResourceManager.Instance != null && ResourceManager.Instance.chips < cost)
        {
            m_IsValid = false;
            return;
        }

        m_CollidingObjects.RemoveAll(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy);
        
        m_IsValid = m_CollidingObjects.Count == 0;

        if (!m_IsValid) return;

        Vector3 checkPos = transform.position;
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            checkPos = transform.TransformPoint(sphere.center);
        }

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null)
        {
            System.Collections.Generic.List<Transform> waypointList = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in pathObj.transform)
            {
                if (child.name.StartsWith("Waypoint"))
                {
                    waypointList.Add(child);
                }
            }

            for (int i = 0; i < waypointList.Count - 1; i++)
            {
                float distToSegment = DistanceToSegment(checkPos, waypointList[i].position, waypointList[i+1].position);
                if (distToSegment < 0.05f) 
                {
                    m_IsValid = false;
                    return;
                }
            }
        }
    }

    float DistanceToSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ap = p - a;
        float t = Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        Vector3 closestPoint = a + t * ab;
        return Vector3.Distance(p, closestPoint);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (Time.timeScale == 0f) return;
        m_CurrentInteractor = args.interactorObject;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (Time.timeScale == 0f) return;
        if (m_IsPlaced) return;

        if (m_IsValid)
        {
            PlaceTower();
        }
        else
        {
            Destroy(gameObject);
        }
        m_CurrentInteractor = null;
    }

    void PlaceTower()
    {
        if (ResourceManager.Instance != null && ResourceManager.Instance.TrySpendChips(cost))
        {
            m_IsPlaced = true;

            SphereCollider sphere = GetComponent<SphereCollider>();
            if (sphere != null) sphere.isTrigger = false;

            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            foreach (var kvp in m_OriginalMaterialsMap)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.sharedMaterials = kvp.Value;
                }
            }

            if (m_TowerScript != null) m_TowerScript.enabled = true;

            m_GrabInteractable.enabled = false;
            
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            gameObject.layer = LayerMask.NameToLayer("Default");
            Debug.Log("Tower Placed!");


            if (audioSource != null && placeSound != null)
            {
                audioSource.PlayOneShot(placeSound);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}