using System;
using System.Collections.Generic;
using System.Diagnostics;
using versoft.asset_manager;
using versoft.data_model;

public class StoreManager 
{
    Random random;

    public StoreManager()
    {
        // Initialize Random
        // TODO (victor): Centralize this.
        random = new Random(Guid.NewGuid().GetHashCode());
    }

    public void TryPurchasingSeedFromBucket(string bucketId)
    {
        var playerManager = ServiceLocator.Instance.Get<PlayerManager>();
        if (playerManager == null)
        { return; }
        
        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return; }

        var storeInfo = dataModelDatabase.Get<Store>(bucketId);
        if (storeInfo == null)
        { return; }

        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        if (gameManager == null)
        { return; }

        if (!playerManager.CanAfford(storeInfo.Cost))
        {
            UnityEngine.Debug.LogError($"Can't afford pulling from {bucketId}");
            return;
        }

        if (!playerManager.HasSpaceForPlants())
        {
            UnityEngine.Debug.LogError("There's no room for new plants");
            return;
        }

        int numberOfTries = 0;
        string randomPlant;
        do
        {
            randomPlant = GetRandomSeed(bucketId);
            numberOfTries ++;
        }
        while (!playerManager.CanAcceptPlant(randomPlant) && numberOfTries < 5);

        if (string.IsNullOrEmpty(randomPlant))
        {
            UnityEngine.Debug.LogError("We couldn't give a plant to the user");
            return;
        }

        playerManager.SpendCurrency(storeInfo.Cost);
        gameManager.GivePlant(randomPlant);
    }

    public List<string> GetStoreBucketsList()
    {
        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return null; }

        var storeInfo = dataModelDatabase.GetList<Store>();
        if (storeInfo == null)
        { return null; }

        List<string> list = new List<string>();
        foreach(var store in storeInfo)
        {
            list.Add(store.Id);
        }

        return list;
    }

    public string GetRandomSeed(string bucketId)
    {
        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return string.Empty; }

        var storeInfo = dataModelDatabase.Get<Store>(bucketId);
        if (storeInfo == null)
        { return string.Empty; }

        var randomChance = random.Next(0, 10000);
        int accumulated = 0;
        foreach(var storeBucket in storeInfo.Bucket)
        {
            accumulated += (int) Math.Round(storeBucket.Rate * 100);
            if(accumulated >= randomChance)
            { 
                return storeBucket.PlantId; 
            }
        }

        return string.Empty;
    }
}
