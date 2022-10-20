using UnityEngine;
using UnityEngine.UI;
using versoft.asset_manager;
using versoft.plant.game_logic;

public class ResourceUIWidget : MonoBehaviour
{
    [SerializeField]
    private PlantStat plantResource;

    [SerializeField]
    private ResourceIcon resourceIcon;

    [SerializeField]
    private Image resourceProgressBar;

    public PlantStat PlantResource => plantResource;

    public void Start()
    {
        SetResourceValue(0);
        if(resourceIcon == null) 
        { return; }

        resourceIcon.Init(plantResource);
    }

    public void SetResourceValue(float value)
    {
        resourceProgressBar.fillAmount = value;

    }
}
