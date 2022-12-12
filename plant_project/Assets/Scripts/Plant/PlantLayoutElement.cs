using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLayoutElement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> plantPositions;

    [SerializeField]
    private Transform plantHolderBackground;

    [SerializeField]
    private string plantLayoutElementId;

    private Dictionary<string, PlantView> _plantViews = new Dictionary<string, PlantView>();
    public Dictionary<string, PlantView> Plantviews => _plantViews;

}
