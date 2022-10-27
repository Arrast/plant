
public static class Const
{
    public const string PrefabsFolder = "Prefabs/";
    public const string AnimationFolder = "Animations/"; 
    
    public const string DefaultPlantAsset = "generic_plant";
    public static string PlantAssetPath = versoft.asset_manager.Const.BaseAddressablesAddress +  "Sprites/Game Assets/Plants/";

    // The lifetime in Days
    public static float PlantLifetime = 3;

    // The number of hours to deplete one unit
    public static float HoursToDeplete = 3.0f;

    // The speed at which resource depletes
    // It should go from 1 to 0 in 3 hours. 
    public static float ResourceDepletionPerSecond = 1.0f / (HoursToDeplete * 3600);
    public static float MaxPlantValue = 2.0f;
    public static float LowResourceThreshold = 0.25f;
    public static float HighResourceThreshold = 1.1f;
    public static float MaxPlantPercentage = 1.25f;
    public static float MaxTimeInBadConditions = 600.0f;
}
