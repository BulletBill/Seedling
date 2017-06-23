using UnityEngine;
using System.Collections;

public class FlatParticle : MonoBehaviour {

	public enum AlphaTypes { NONE, LINEAR, SQRT }

	public Vector2 MinVelocity = new Vector2(-0.025f, 0.1f);
	public Vector2 MaxVelocity = new Vector2(0.025f, 0.2f);
	Vector2 Velocity;

	public float TotalLifeTime = 2.0f;
	float RandomLifeTime;
	float LifeTime;

	public bool SubParticle = false;

	public AlphaTypes AlphaFallOff;
	Color StartingColor;
	float Alpha;

	// Cached Components
	SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
        StartingColor = spriteRenderer.color;
	}

	void OnEnable () {
		Velocity = new Vector2(Random.Range(MinVelocity.x, MaxVelocity.x), Random.Range(MinVelocity.y, MaxVelocity.y));

		RandomLifeTime = TotalLifeTime * Random.Range(0.9f, 1.1f);
		LifeTime = 0;

		spriteRenderer.color = StartingColor;
    }
	
	// Update is called once per frame
	void Update () {
		LifeTime += Time.deltaTime;
		
		if (LifeTime > RandomLifeTime && false == SubParticle) {
			SimplePool.Despawn(this.gameObject);
			return;
		}

		if (AlphaFallOff != AlphaTypes.NONE) {
			if (AlphaFallOff == AlphaTypes.LINEAR) {
				Alpha = Mathf.Clamp01(1.0f - (LifeTime / RandomLifeTime));
			}

			if (AlphaFallOff == AlphaTypes.SQRT) {
				Alpha = Mathf.Sqrt(Alpha);
			}

			Color newColor = StartingColor;
			newColor.a *= Alpha;
			spriteRenderer.color = newColor;

		}

		this.transform.Translate(Velocity * Time.deltaTime);
	}
}
