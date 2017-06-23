using UnityEngine;
using System.Collections;

public class OutlineParticleEffect : MonoBehaviour {

	public GameObject ParticlePrefab;
	public float Rate = 10; //Particles per second
	float TimeBetweenSpawns = 0;

	float TimeSinceLastSpawn = 0;

	// Use this for initialization
	void Start () {
		if (Rate != 0) {
			TimeBetweenSpawns = 1.0f / Rate;
		}
	}

	// Update is called once per frame
	void Update () {
		TimeSinceLastSpawn += Time.deltaTime;

		while (TimeSinceLastSpawn > TimeBetweenSpawns) {
			SpawnParticleOnOutline();
			TimeSinceLastSpawn -= 1.0f / Rate;
		}
	}

	void SpawnParticleOnOutline() {
		PolygonCollider2D col = GetComponentInParent<PolygonCollider2D>();

		int pathID = Random.Range(0, col.pathCount);

		Vector2[] points = col.GetPath(pathID);
		int pointIndex = Random.Range(0, points.Length);

		Vector2 pointA = points[ pointIndex ];
		Vector2 pointB = points[ (pointIndex + 1) % points.Length ];

		Vector2 spawnPoint = Vector2.Lerp(pointA, pointB, Random.Range(0.0f, 1.0f));

		SpawnParticleAtPosition(spawnPoint + (Vector2)this.transform.position);
	}

	void SpawnParticleAtPosition(Vector2 position) {
		//Instantiate(ParticlePrefab, position, Quaternion.identity);
		SimplePool.Spawn(ParticlePrefab, position, Quaternion.identity);
	}
}
