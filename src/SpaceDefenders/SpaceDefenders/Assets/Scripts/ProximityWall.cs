using UnityEngine;

[ExecuteAlways]
public class ProximityWall : MonoBehaviour
{
    public Transform player;
    public float maxDistance = 1.2f;
    public float minDistance = 0.3f;
    
    private MeshRenderer m_Renderer;
    private MaterialPropertyBlock m_PropBlock;
    private static readonly int s_BaseColorId = Shader.PropertyToID("_BaseColor");

    void OnEnable()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_PropBlock = new MaterialPropertyBlock();
        InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        if (m_Renderer == null) return;

        Material mat = Application.isPlaying ? m_Renderer.material : m_Renderer.sharedMaterial;
        if (mat == null) return;

        // Ensure material is set up for transparency and additive blending
        // Note: Modifying sharedMaterial properties here will affect the asset in the Editor
        mat.SetFloat("_Surface", 1.0f);
        mat.SetFloat("_Blend", 1.0f); // Additive
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_ZWrite", 0);
        mat.SetInt("_Cull", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void Update()
    {
        if (player == null)
        {
            if (Camera.main != null) player = Camera.main.transform;
            else return;
        }

        if (m_Renderer == null) return;

        float distance = GetDistanceToPlayer();
        float alpha = Mathf.Clamp01(1.0f - Mathf.InverseLerp(minDistance, maxDistance, distance));

        // Use MaterialPropertyBlock to set alpha per-object
        m_Renderer.GetPropertyBlock(m_PropBlock);
        Color color = m_Renderer.sharedMaterial.GetColor(s_BaseColorId);
        color.a = alpha;
        m_PropBlock.SetColor(s_BaseColorId, color);
        m_Renderer.SetPropertyBlock(m_PropBlock);

        // Toggle renderer for optimization and absolute invisibility
        bool shouldBeVisible = alpha > 0.001f;
        if (m_Renderer.enabled != shouldBeVisible)
        {
            m_Renderer.enabled = shouldBeVisible;
        }
    }

    private float GetDistanceToPlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector3 normal = transform.up; 
        return Mathf.Abs(Vector3.Dot(toPlayer, normal));
    }
}
