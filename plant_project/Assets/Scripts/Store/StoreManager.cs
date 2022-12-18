using System;
using versoft.asset_manager;
using versoft.data_model;

public enum ProductType
{
    SeedBucket,
    ShelfUnlock,
    ShelfCosmetic,
    SunblockCosmetic,
    PotCosmetic
}

public class StoreManager
{
    public void TryPurchasingProduct(string productId)
    {
        var playerManager = ServiceLocator.Instance.Get<PlayerManager>();
        if (playerManager == null)
        { return; }

        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return; }

        var product = dataModelDatabase.Get<Products>(productId);
        if (product == null)
        { return; }

        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        if (gameManager == null)
        { return; }

        if (!playerManager.CanAfford(product.Cost))
        {
            UnityEngine.Debug.LogError($"Can't afford pulling from {productId}");
            return;
        }

        if (!CanPurchaseProduct(product))
        {
            UnityEngine.Debug.LogError($"Can't purchase this product {productId}");
            return;
        }

        if (TryGivingReward(product))
        {
            playerManager.SpendCurrency(product.Cost);
        }
    }

    private bool TryGivingReward(Products product)
    {
        switch (product.ProductType)
        {
            case ProductType.SeedBucket:
                return TryGivingPlant(product);
            case ProductType.ShelfUnlock:
                return TryUnlockingShelf(product);
            default:
                return false;
        }
    }

    private bool TryUnlockingShelf(Products product)
    {
        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        var playerManager = ServiceLocator.Instance.Get<PlayerManager>();

        gameManager.UnlockShelf(product.ProductReference);
        return true;
    }

    private bool TryGivingPlant(Products product)
    {
        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        var playerManager = ServiceLocator.Instance.Get<PlayerManager>();

        string plantBucketId = product.ProductReference;
        int numberOfTries = 0;
        string randomPlant;
        do
        {
            randomPlant = GetRandomSeed(plantBucketId);
            numberOfTries++;
        }
        while (!playerManager.CanAcceptPlant(randomPlant) && numberOfTries < 5);

        if (string.IsNullOrEmpty(randomPlant))
        {
            UnityEngine.Debug.LogError("We couldn't give a plant to the user");
            return false;
        }

        gameManager.GivePlant(randomPlant);
        return true;
    }

    private bool CanPurchaseProduct(Products product)
    {
        switch (product.ProductType)
        {
            case ProductType.SeedBucket:
                {
                    var playerManager = ServiceLocator.Instance.Get<PlayerManager>();
                    return playerManager.HasSpaceForPlants();
                }
            case ProductType.ShelfUnlock:
                {
                    var playerManager = ServiceLocator.Instance.Get<PlayerManager>();
                    return !playerManager.HasUnlockedShelf(product.ProductReference);
                }
            default:
                {
                    return false;
                }
        }
    }

    public string GetRandomSeed(string bucketId)
    {
        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return string.Empty; }

        var storeInfo = dataModelDatabase.Get<SeedBucket>(bucketId);
        if (storeInfo == null)
        { return string.Empty; }

        var randomManager = ServiceLocator.Instance.Get<RandomManager>();
        if (randomManager == null)
        { return string.Empty; }

        var randomChance = randomManager.GetRandom(0, 10000);
        int accumulated = 0;
        foreach (var storeBucket in storeInfo.Bucket)
        {
            accumulated += (int)Math.Round(storeBucket.Rate * 100);
            if (accumulated >= randomChance)
            {
                return storeBucket.PlantId;
            }
        }

        return string.Empty;
    }
}
