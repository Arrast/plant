using UnityEngine;
using versoft.asset_manager;

public class TestShop : MonoBehaviour
{
    private StoreManager _storeManager;
    private StoreManager StoreManager
    {
        get
        {
            if (_storeManager == null)
            {
                _storeManager = ServiceLocator.Instance.Get<StoreManager>();
            }
            return _storeManager;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            StoreManager.TryPurchasingProduct("product_1");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StoreManager.TryPurchasingProduct("product_3");
        }
    }
}
