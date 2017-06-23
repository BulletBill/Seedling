using UnityEngine;
using System.Collections;

[System.Serializable]
public class pathNode {

	public enum commands { NONE, FIRE, REVERSE, DETONATE }

	public float relx;
	public float rely;
	public float radius = 0.1f;
	public float wait;
	public commands command;

    //Worldspace coordinates
    public float x { get; protected set; }
    public float y { get; protected set; }

	public pathNode() { }
	public pathNode(float X, float Y) {
		x = X;
		y = Y;
	}
    public pathNode(float X, float Y, float Radius, float Wait, commands Command) {
        x = X;
        y = Y;
        radius = Radius;
        wait = Wait;
        command = Command;
    }

    public void align (float parentX, float parentY) {
        x = relx + parentX;
        y = rely + parentY;
    }

    public void align (Vector2 parentPos) {
        align(parentPos.x, parentPos.y);
    }
}
