using TMPro;
using UnityEngine;
using UnityEngine.UI;
using versoft.asset_manager;

public class CurrencyWidget : MonoBehaviour
{
    [SerializeField]
    private Image currencyIcon;

    [SerializeField]
    private TextMeshProUGUI currencyAmountLabel;

    public void SetCurrency(string currencyId, int amount)
    {
        if (!string.IsNullOrEmpty(currencyId))
        {
            LoadIcon(currencyId);
        }

        UpdateAmount(amount);
    }

    private async void LoadIcon(string currencyId)
    {
        var assetManager = ServiceLocator.Instance.Get<AssetManager>();
        if (assetManager == null)
        { return; }

        string resourcePath = $"Sprites/UI/{currencyId}_icon";
        var sprite = await assetManager.LoadAsset<Sprite>(resourcePath);
        if (sprite != null)
        {
            currencyIcon.sprite = sprite;
        }
    }

    public void UpdateAmount(int amount)
    {
        if (currencyAmountLabel == null)
        { return; }

        currencyAmountLabel.text = $"{amount}";
    }
}
