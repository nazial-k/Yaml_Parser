using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using System.IO;

[System.Serializable]
public class MaterialData
{
    public string name;
    public string color;
    public float metallic;
    public float smoothness;
}

[System.Serializable]
public class ObjectData
{
    public Dictionary<string, float> position;
    public Dictionary<string, float> rotation;
    public Dictionary<string, float> scale;
    public List<MaterialData> materials;

    public void PrintMaterialData()
    {
        foreach (MaterialData material in materials)
        {
            Debug.Log("Material Name: " + material.name + ", Color: " + material.color);
            // Print other material properties as needed
        }
    }
    public void PrintPositionData()
    {
        if (position != null)
        {
            Debug.Log("Position:");
            Debug.Log("  x: " + position["x"]);
            Debug.Log("  y: " + position["y"]);
            Debug.Log("  z: " + position["z"]);
        }
        else
        {
            Debug.Log("Position data not found in YAML file.");
        }
    }

}


public class YamlParser : MonoBehaviour
{
    public string yamlFileName = "data.yml"; // Name of your YAML file (without extension)

    public GameObject targetGameObject;

    private void Start()
    {
        // Construct the full file path
        string yamlFilePath = "Resources/" + yamlFileName;

        // Load YAML file as text asset
        string yamlText = File.ReadAllText(Application.dataPath + "/" + yamlFilePath);

        Debug.Log(yamlText);

        if (yamlText!= null)
        {

            // Parse YAML data
            var deserializer = new DeserializerBuilder().Build();
            var objectData = deserializer.Deserialize<ObjectData>(new StringReader(yamlText));

            if (objectData != null)
            {
                Debug.Log("Successfully Parsed YAML data!");
                //objectData.PrintMaterialData();
                //objectData.PrintPositionData();
            }
            else
            {
                Debug.LogError("Failed to deserialize YAML data!");
            }

            if (targetGameObject != null)
                {
                    targetGameObject.transform.localPosition = new Vector3(objectData.position["x"], objectData.position["y"], objectData.position["z"]);
                    targetGameObject.transform.localRotation = Quaternion.Euler(objectData.rotation["x"], objectData.rotation["y"], objectData.rotation["z"]);
                    targetGameObject.transform.localScale = new Vector3(objectData.scale["x"], objectData.scale["y"], objectData.scale["z"]);

                    Renderer targetRenderer = targetGameObject.GetComponent<Renderer>();
                    if (targetRenderer != null && targetRenderer.material != null)
                    {
                    // Find the MaterialData named "Material1"
                        MaterialData targetMaterial = objectData.materials.Find(m => m.name == "Material1");

                        if (targetMaterial != null)
                        {
                            // Assuming the color property is in a format the material understands (e.g., "Color" or "_Color")
                            Color newColor = Color.white;  // Default color if parsing fails
                            if (ColorUtility.TryParseHtmlString(targetMaterial.color, out newColor))
                            {
                            targetRenderer.material.color = newColor;
                            Debug.Log("Successfully applied color from Material1 to target object material.");
                            }
                            else
                            {
                            Debug.LogError("Failed to parse color value from Material1: " + targetMaterial.color);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Material named 'Material1' not found in YAML data.");
                        }
                    }
                    else
                    {
                    Debug.LogError("Renderer component not found on targetGameObject.");
                    }
                } 
        }
        else
        {
            Debug.LogError("Failed to load YAML file: " + yamlFilePath);
        }
    }
}