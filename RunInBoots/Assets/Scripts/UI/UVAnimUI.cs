using UnityEngine;
using UnityEngine.UI;

public class UVAnimUI : MonoBehaviour
{
    public Vector2 uvSpeed = new Vector2(0.1f, 0.0f); // Speed of the UV animation
    private Material material; // The material to animate

    void Start()
    {
        // Get the material of the Image component
        Image image = GetComponent<Image>();
        if (image != null && image.material != null)
        {
            Material instnace = new Material(image.material);
            image.material=instnace;
            material = instnace;
        }
        else
        {
            Debug.LogError("No material found on the Image component!");
        }
    }

    void Update()
    {
        if (material != null)
        {
            // Calculate the new texture offset
            Vector2 offset = material.mainTextureOffset;
            offset += uvSpeed * Time.deltaTime;

            // Apply the offset to the material
            material.mainTextureOffset = offset;
        }
    }
}
