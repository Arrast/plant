using Newtonsoft.Json;

public enum Rarity
{
    Common,
    Rare,
    Secret
}

[System.Serializable]
public class StoreBucketData
{
    public string PlantId;
    public float Rate;
}
