using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Ana menü sahnesini yüklemek için gerekli kütüphane

public class BaseHealth : MonoBehaviour
{
    [Header("Reactor Health")]
    public int maxHealth = 10;
    public int health = 10;

    [Header("UI")]
    public TextMeshProUGUI hpText;
    public string label = "REACTOR";

    [Header("Health Bar UI (Slider)")]
    public Slider healthSlider;

    [Header("Damage Feedback (illusion)")]
    public Renderer reactorRenderer;
    public int materialIndex = 0;
    public Color flashColor = new Color(1f, 0.15f, 0.1f);
    public float flashDuration = 0.25f;
    public float shakeMagnitude = 0.03f;

    private Color m_OriginalColor;
    private bool m_HasOriginalColor;
    private Vector3 m_OriginalPos;
    private bool m_IsDead;

    void Awake()
    {
        health = maxHealth;
        m_OriginalPos = transform.position;
        if (reactorRenderer == null) reactorRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        UpdateUI();
        SetupHealthSlider();
    }

    void SetupHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    public void TakeDamage(int amount)
    {
        if (m_IsDead) return;

        health -= amount;
        if (health < 0) health = 0;
        Debug.Log("Reactor HP: " + health + " / " + maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        UpdateUI();

        StopAllCoroutines();
        StartCoroutine(DamageFeedback());

        if (health <= 0) Die();
    }

    void UpdateUI()
    {
        if (hpText == null) return;
        hpText.text = label + " HP: " + health + " / " + maxHealth;
        float t = maxHealth > 0 ? (float)health / maxHealth : 0f;
        hpText.color = Color.Lerp(new Color(1f, 0.2f, 0.15f), new Color(0.2f, 0.9f, 1f), t);
    }

    IEnumerator DamageFeedback()
    {
        Material targetMat = null;
        if (reactorRenderer != null && materialIndex < reactorRenderer.materials.Length)
        {
            targetMat = reactorRenderer.materials[materialIndex];
        }

        bool canColor = targetMat != null && targetMat.HasProperty("_BaseColor");
        if (canColor)
        {
            if (!m_HasOriginalColor)
            {
                m_OriginalColor = targetMat.GetColor("_BaseColor");
                m_HasOriginalColor = true;
            }
            targetMat.SetColor("_BaseColor", flashColor);
            if (targetMat.HasProperty("_EmissionColor"))
            {
                targetMat.EnableKeyword("_EMISSION");
                targetMat.SetColor("_EmissionColor", flashColor * 2f);
            }
        }

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float falloff = 1f - (elapsed / flashDuration);
            transform.position = m_OriginalPos + Random.insideUnitSphere * (shakeMagnitude * falloff);
            yield return null;
        }
        transform.position = m_OriginalPos;

        if (canColor && m_HasOriginalColor)
        {
            targetMat.SetColor("_BaseColor", m_OriginalColor);
            if (targetMat.HasProperty("_EmissionColor"))
                targetMat.SetColor("_EmissionColor", Color.black);
        }
    }

    void Die()
    {
        m_IsDead = true;
        Debug.Log("Reactor destroyed - Game Over!");
        if (hpText != null)
        {
            hpText.text = label + " DOWN";
            hpText.color = new Color(1f, 0.2f, 0.15f);
        }


        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}