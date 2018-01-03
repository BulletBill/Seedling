using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform FollowObject;
	bool ShouldFollow;
	Vector2 FollowPos;
	Vector2 FollowPrevPos;

	public Vector2 FollowMargin;
	Vector2 FollowSmoothing = new Vector2(1.0f, 1.0f);

	public float KeyboardScrollSpeed = 1.0f;
	public float MouseScrollSpeed = 1.0f;

	public float MinZoomLevel = 3.0f;
	public float MaxZoomLevel = 7.0f;

	Camera ParentCamera;

	//Constrains movement of the camera
	bool BoundsSet = false;
	Vector2 HalfCameraSize;
	Vector2 MapSize;
	Rect CameraBounds;

	[ExecuteInEditMode]
	void OnValidate() {
		if (MinZoomLevel < 0.0f) { MinZoomLevel = 0.0f; }
		if (MaxZoomLevel < MinZoomLevel) { MaxZoomLevel = MinZoomLevel; }
	}

	// Use this for initialization
	void Awake() {
		//Setting up boundry related values in Awake so that other objects can call SetCameraBounds in their Start() functions
		ParentCamera = this.gameObject.GetComponent<Camera>();

		SetCameraSize(ParentCamera.orthographicSize);
	}

	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		HandleInput();
		UpdateCameraPosition();
	}

	void UpdateCameraPosition() {
		Vector3 NewPosition = ParentCamera.transform.position;
		NewPosition.x += Input.GetAxis("CameraHorizontal") * KeyboardScrollSpeed * Time.deltaTime;
		NewPosition.y += Input.GetAxis("CameraVertical") * KeyboardScrollSpeed * Time.deltaTime;

		if (null != FollowObject) {
			FollowPos = FollowObject.GetComponent<SpriteRenderer>().bounds.center;

			if (FollowPrevPos != FollowPos) {
				ShouldFollow = true;
			}

			if (ShouldFollow) {

				if (Mathf.Abs(NewPosition.x - FollowPos.x) > FollowMargin.x) {
					NewPosition.x = Mathf.Lerp(NewPosition.x, FollowPos.x, FollowSmoothing.x * Time.deltaTime);
				}
				if (Mathf.Abs(NewPosition.y - FollowPos.y) > FollowMargin.y) {
					NewPosition.y = Mathf.Lerp(NewPosition.y, FollowPos.y, FollowSmoothing.y * Time.deltaTime);
				}
			}

			FollowPrevPos = FollowPos;
		}

		if (BoundsSet) {
			if (NewPosition.x < CameraBounds.x) { NewPosition.x = CameraBounds.x; }
			if (NewPosition.x > CameraBounds.width) { NewPosition.x = CameraBounds.width; }
			if (NewPosition.y < CameraBounds.y) { NewPosition.y = CameraBounds.y; }
			if (NewPosition.y > CameraBounds.height) { NewPosition.y = CameraBounds.height; }
		}

		//Stop following the target if the camera is no longer moving
		if (NewPosition == ParentCamera.transform.position) {
			ShouldFollow = false;
		} else {
			ParentCamera.transform.position = NewPosition;
		}
	}

	void HandleInput() {
		float ZoomDelta = -Input.GetAxis("Mouse ScrollWheel") * 4.0f;

		if (ZoomDelta != 0.0f) {
			SetCameraSize(ParentCamera.orthographicSize + ZoomDelta);
		}
	}

	//Accessors
	public void SetMapBounds(float SizeX, float SizeY) {
		if (SizeX < 0) { SizeX = 0; }
		if (SizeY < 0) { SizeY = 0; }

		MapSize.x = SizeX;
		MapSize.y = SizeY;
		CalculateCameraBounds();
	}

	public void SetCameraSize(float NewSize) {

		NewSize = Mathf.Clamp(NewSize, MinZoomLevel, MaxZoomLevel);

		ParentCamera.orthographicSize = NewSize;
		
		//Setup camera size for keeping in bounds
		float height = 2f * ParentCamera.orthographicSize;
		float width = height * ParentCamera.aspect;
		HalfCameraSize = new Vector2(width / 2, height / 2);
		FollowMargin = new Vector2(width, height) * 0.4f;		

		CalculateCameraBounds();
	}


	void CalculateCameraBounds() {
		// Early return
		if (MapSize.x <= 0.0f && MapSize.y <= 0.0f) { return; }
		CameraBounds = new Rect(HalfCameraSize.x, HalfCameraSize.y, MapSize.x - HalfCameraSize.x, MapSize.y - HalfCameraSize.y);
		BoundsSet = true;
	}
}