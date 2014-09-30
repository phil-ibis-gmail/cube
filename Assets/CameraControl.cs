using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float HScale = 400.0f;
	public float VScale = 400.0f;
	public GUIText mDebugText;
	Quaternion QStart;

	Quaternion QTarget;
	float CurrentLookTime=0.0f;
	float TotalLookTime=0.15f;
	Quaternion qBegin;
	Vector3 posBegin;

	// Use this for initialization
	void Start () {
		transform.LookAt (Vector3.zero);
		QStart = transform.rotation;
		posBegin = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float h = HScale * Input.GetAxis ("Horizontal");
		transform.RotateAround (Vector3.zero, Vector3.up, h * Time.deltaTime);

		float v = VScale * Input.GetAxis ("Vertical");
		transform.RotateAround (Vector3.zero, Vector3.right, v * Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.X)) 
		{
			transform.position = new Vector3(9.0f,7.0f,-9.0f);
			transform.rotation = QStart;
			//transform.RotateAround (Vector3.zero,Vector3.up,200);
			//transform.LookAt (Vector3.zero);
		}

		if (Input.GetKeyDown (KeyCode.N) )
		{
			//qBegin=transform.rotation;
			//posBegin=transform.position;
			if(CurrentLookTime > 0.0f)
			{
				qBegin=Quaternion.Slerp (qBegin, QTarget, 1.0f-CurrentLookTime/TotalLookTime);
			}
			else
			{
				qBegin=Quaternion.Euler (0,0,0);
			}
			
			QTarget=Quaternion.Euler (0,90.0f,0);
			////transform.position = QTarget * transform.position;
			////transform.LookAt (Vector3.zero);
			CurrentLookTime=TotalLookTime;
		}

		if (CurrentLookTime > 0.0f)
		{
			Quaternion slerped = Quaternion.Slerp (qBegin, QTarget, 1.0f-CurrentLookTime/TotalLookTime);
			transform.position = slerped * posBegin;
			transform.LookAt (Vector3.zero);
			CurrentLookTime = CurrentLookTime - Time.deltaTime;
		}

		if(Input.GetKeyUp(KeyCode.N))
		{
			if(CurrentLookTime > 0.0f)
			{
				qBegin=Quaternion.Slerp (qBegin, QTarget, 1.0f-CurrentLookTime/TotalLookTime);
			}
			else
			{
				qBegin=Quaternion.Euler (0,90,0);
			}
			QTarget=Quaternion.Euler(0,0,0);
			CurrentLookTime=TotalLookTime;
		}
		
		if(Input.GetMouseButton(0))
		{
			mDebugText.text = string.Format (Input.mousePosition.ToString());
		}
		
		//transform.rotation=QTarget*QStart;
	}
}
