using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ButtonTextHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {

        transform.Find("Text").GetComponent<Text>().text = transform.name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
