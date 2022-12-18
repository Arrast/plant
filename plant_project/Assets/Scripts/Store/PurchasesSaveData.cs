using System;
using System.Collections.Generic;

[Serializable]
public class PurchaseItem
{
    public string ProductId;
    public int NumberOfPurchases;
}

[Serializable]
public class PurchasesSaveData
{
    public List<PurchaseItem> Purchases = new List<PurchaseItem>();

    public void AddPurchase(string productId)
    {
        bool found = false;
        //foreach(var product in Purchases)
        for (int i = 0; i < Purchases.Count; i++)
        {
            if (Purchases[i].ProductId == productId)
            {
                Purchases[i].NumberOfPurchases++;
                found = true;
            }
        }

        if (!found)
        {
            Purchases.Add(new PurchaseItem
            {
                ProductId = productId,
                NumberOfPurchases = 1
            });
        }
    }
}
