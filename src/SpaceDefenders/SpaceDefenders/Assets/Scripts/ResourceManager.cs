using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int chips = 15;
    public TextMeshProUGUI chipText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public int GetChips()
    {
        return chips;
    }

    public void AddChips(int amount)
    {
        chips += amount;
        UpdateUI();
    }

    public bool TrySpendChips(int amount)
    {
        if (chips >= amount)
        {
            chips -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (chipText != null)
        {
            chipText.text = "Chips: " + chips;
        }
    }
}