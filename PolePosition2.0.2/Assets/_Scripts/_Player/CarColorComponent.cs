using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that manages the change in color of the car
/// </summary>
public class CarColorComponent : MonoBehaviour
{
    [SerializeField] private MeshRenderer _carBodyMeshRenderer;

    // All the possible materials (Colors)
    [SerializeField] private List<Material> _carMaterials = new List<Material>();

    public void SetCarColor(int colorID)
    {
        Material[] materials;

        // This assumes that the car body color is in the index 1
        // If the car color isnt the second material this will break
        if (colorID >= 0 && colorID < _carMaterials.Count)
        {
            materials = _carBodyMeshRenderer.materials;
            materials[1] = _carMaterials[colorID];
            _carBodyMeshRenderer.materials = materials;
        }
    }
}
