
public static class Const
{
    public const string PrefabsFolder = "Prefabs/";
    public const string AnimationFolder = "Animations/"; 
    
    public const string DefaultPlantAsset = "generic_plant";
    public static string PlantAssetPath = versoft.asset_manager.Const.BaseAddressablesAddress +  "Sprites/Game Assets/Plants/";

    // The number of hours to deplete one unit
    public const float HoursToDeplete = 3.0f;

    // The speed at which resource depletes
    // It should go from 1 to 0 in 3 hours. 
    public const float ResourceDepletionPerSecond = 1.0f / (HoursToDeplete * 3600);
    public const float MaxPlantValue = 2.0f;
    public const float LowResourceThreshold = 0.25f;
    public const float HighResourceThreshold = 1.1f;
    public const float MaxPlantPercentage = 1.25f;
    public const float MaxTimeInBadConditions = 600.0f;

    // Id of the soft currency
    public const string SoftCurrencyId = "soft_currency";

    // Max Number of Slots per Area
    public const int MaxNumberOfPlantsPerZone = 4;

}
