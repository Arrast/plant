using System.Collections;
using System.Collections.Generic;
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

    private List<string> bucketList;
    private List<string> BucketList
    {
        get
        {
            if (bucketList == null)
            {
                bucketList = StoreManager.GetStoreBucketsList();
            }
            return bucketList;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (BucketList != null && BucketList.Count > 0)
            {
                var bucketIndex = Random.Range(0, BucketList.Count);
                StoreManager.TryPurchasingSeedFromBucket(BucketList[bucketIndex]);
                // string randomPlant = StoreManager.GetRandomSeed(BucketList[bucketIndex]);
                // UnityEngine.Debug.LogError($"I pulled {randomPlant} from {BucketList[bucketIndex]}");
            }
        }
    }
}
