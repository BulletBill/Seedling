using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ColorToTile {
	public string name;
	public Color32 color;
	public GameObject prefab;
}

//This class exists to have a central definition for converting colored pixels to tiles
//All values should be defined in the editor and saved as a prefab to be used in the approriate scene
public class TileDefs : MonoBehaviour {

	static public TileDefs ColorToTile { get; protected set; }

	public List<ColorToTile> colorToPrefab;

	void Start() {
	}

}