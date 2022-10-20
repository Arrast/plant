using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using versoft.plant.game_logic;

public class ResourceUpdateWidget : MonoBehaviour
{
    [SerializeField]
    private PlantStat plantStat;

    [SerializeField]
    private DraggableElement draggableElement;

    [SerializeField]
    private ResourceIcon resourceIcon;

    private System.Action<PlantStat> _widgetEnteredTarget;
    private System.Action<PlantStat> _widgetLeftTarget;

    public void Init(System.Action<PlantStat> widgetEnteredTarget, System.Action<PlantStat> widgetLeftTarget)
    {
        if (draggableElement == null || resourceIcon == null || plantStat == PlantStat.None) 
        { return; }

        _widgetEnteredTarget = widgetEnteredTarget;
        _widgetLeftTarget = widgetLeftTarget;
        resourceIcon.Init(plantStat);
        draggableElement.WidgetEnteredTarget = WidgetEnteredTarget;
        draggableElement.WidgetLeftTarget = WidgetLeftTarget;
    }

    private void WidgetEnteredTarget()
    {
        _widgetEnteredTarget?.Invoke(plantStat);
    }

    private void WidgetLeftTarget()
    {
        _widgetLeftTarget?.Invoke(plantStat);
    }
}
