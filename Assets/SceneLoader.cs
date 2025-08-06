//using System.Diagnostics;
using System.Xml;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        TextAsset xmlData = Resources.Load<TextAsset>("ExamTestingScene"); // No file extension
        if (xmlData == null)
        {
            Debug.LogError("Scene XML not found in Resources folder!");
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData.text);

        XmlNodeList objectNodes = xmlDoc.SelectNodes("GameObjects/GameObject");

        foreach (XmlNode objNode in objectNodes)
        {
            string name = objNode.SelectSingleNode("Name").InnerText;
            int type = int.Parse(objNode.SelectSingleNode("Type").InnerText);
            int rigidBody = int.Parse(objNode.SelectSingleNode("RigidBody").InnerText);

            Vector3 position = ReadVector3(objNode.SelectSingleNode("Position"));
            Vector3 scale = ReadVector3(objNode.SelectSingleNode("Scale"));
            Vector3 rotation = ReadVector3(objNode.SelectSingleNode("Rotation"));

            // Mapping to the game engine's type enum to Unity's PrimitiveType enum
            PrimitiveType unityType = PrimitiveType.Cube;

            switch (type)
            {
                case 2: unityType = PrimitiveType.Cube; break;
                case 3: unityType = PrimitiveType.Plane; break;
                case 4: unityType = PrimitiveType.Sphere; break;
                case 5: unityType = PrimitiveType.Capsule; break;
                case 10: unityType = PrimitiveType.Cylinder; break;
                case 6: unityType = PrimitiveType.Quad; break;
                default:
                    Debug.LogWarning($"Unsupported primitive type: {type}");
                    continue;
            }

            GameObject go = GameObject.CreatePrimitive(unityType);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.transform.eulerAngles = rotation;

            if (rigidBody != 0)
                go.AddComponent<Rigidbody>();
        }
    }

    Vector3 ReadVector3(XmlNode node)
    {
        float x = float.Parse(node.SelectSingleNode("x").InnerText);
        float y = float.Parse(node.SelectSingleNode("y").InnerText);
        float z = float.Parse(node.SelectSingleNode("z").InnerText);
        return new Vector3(x, y, z);
    }
}