using System.Xml;
using System.IO;
using UnityEngine;
//using static System.Net.Mime.MediaTypeNames;
//using System.Diagnostics;

public class SceneExporter : MonoBehaviour
{
    [ContextMenu("Export Scene to XML")]
    void ExportScene()
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("GameObjects");
        doc.AppendChild(root);

        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            ExportGameObject(doc, root, go);
        }

        string path = Path.Combine(Application.dataPath, "../ExportedScene.xml");
        doc.Save(path);
        Debug.Log($"Scene exported to: {path}");
    }

    void ExportGameObject(XmlDocument doc, XmlElement root, GameObject go)
    {
        XmlElement objElem = doc.CreateElement("GameObject");

        XmlElement nameElem = doc.CreateElement("Name");
        nameElem.InnerText = go.name;
        objElem.AppendChild(nameElem);

        XmlElement typeElem = doc.CreateElement("Type");
        PrimitiveType? type = GetPrimitiveType(go);
        typeElem.InnerText = ((int)(type ?? PrimitiveType.Cube)).ToString(); // fallback Cube
        objElem.AppendChild(typeElem);

        AppendVector3(doc, objElem, "Position", go.transform.position);
        AppendVector3(doc, objElem, "Scale", go.transform.localScale);
        AppendVector3(doc, objElem, "Rotation", go.transform.eulerAngles);

        XmlElement rigidbodyElem = doc.CreateElement("RigidBody");
        rigidbodyElem.InnerText = go.GetComponent<Rigidbody>() ? "1" : "0";
        objElem.AppendChild(rigidbodyElem);

        root.AppendChild(objElem);

        // Handle children recursively
        foreach (Transform child in go.transform)
        {
            ExportGameObject(doc, root, child.gameObject);
        }
    }

    void AppendVector3(XmlDocument doc, XmlElement parent, string label, Vector3 vec)
    {
        XmlElement vecElem = doc.CreateElement(label);

        XmlElement xElem = doc.CreateElement("x");
        xElem.InnerText = vec.x.ToString();
        vecElem.AppendChild(xElem);

        XmlElement yElem = doc.CreateElement("y");
        yElem.InnerText = vec.y.ToString();
        vecElem.AppendChild(yElem);

        XmlElement zElem = doc.CreateElement("z");
        zElem.InnerText = vec.z.ToString();
        vecElem.AppendChild(zElem);

        parent.AppendChild(vecElem);
    }

    PrimitiveType? GetPrimitiveType(GameObject go)
    {
        if (!go.TryGetComponent<MeshFilter>(out var mf)) return null;
        string meshName = mf.sharedMesh?.name.ToLower();

        if (meshName.Contains("cube")) return PrimitiveType.Cube;
        if (meshName.Contains("plane")) return PrimitiveType.Plane;
        if (meshName.Contains("sphere")) return PrimitiveType.Sphere;
        if (meshName.Contains("capsule")) return PrimitiveType.Capsule;
        if (meshName.Contains("quad")) return PrimitiveType.Quad;

        return null;
    }
}
