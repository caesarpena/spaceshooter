using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteExplorer : MonoBehaviour {

    //public RectTransform parentPanel;
    //public GameObject prefabButton;

    private string[] prefabList = { "Asteroid", "Background", "Blackhole", "Cloud", "Moon", "Planet", "Ship", "Starfield", "Station", "Sun" };

    // Use this for initialization
    void Start () {

       /* for (int i = 0; i < prefabList.Length; i++)
        {
            string prefabName = prefabList[i];

            GameObject button = Instantiate(prefabButton);
            button.transform.SetParent(parentPanel, false);
            button.transform.localScale = new Vector3(1, 1, 1);
            button.name = prefabName;
            button.GetComponent<Button>().onClick.AddListener(() => GenerateStellarSprite(prefabName));
        }*/
    }
	
    public void GenerateStellarSprite(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);

        if (prefabName == "Background")
        {
            Destroy(transform.Find("Background").gameObject);

            GameObject newGO = Instantiate(prefab, transform, true);
            newGO.name = newGO.name.Replace("(Clone)", "");
            newGO.transform.parent = transform;
        }
        else if (prefabName == "Starfield")
        {
            Destroy(transform.Find("Starfield").gameObject);

            GameObject newGO = Instantiate(prefab, transform, true);
            newGO.name = newGO.name.Replace("(Clone)", "");
            newGO.transform.parent = transform;
        }
        else
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "Background" && child.gameObject.name != "Starfield")
                    children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));

            GameObject newGO = Instantiate(prefab, transform, true);
            newGO.name = newGO.name.Replace("(Clone)", "");
        }
    }
}
