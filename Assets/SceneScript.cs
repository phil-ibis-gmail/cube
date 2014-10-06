using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneScript : MonoBehaviour {

	private HelloPhil [] mCubes;
	private List<RotationTarget> mTargets= new List<RotationTarget>();
	private bool mIsRandomizing;
	public bool IsRandomizing {get {return mIsRandomizing;}}
	private int mMoves;
	public int Moves {get {return mMoves;}}

	private Vector3 [] light_axes = 
	{
		Vector3.up, Vector3.down,
		Vector3.left, Vector3.right,
		Vector3.forward, Vector3.back,
	};

	// Use this for initialization
	void Start () {
		mCubes = FindObjectsOfType (typeof(HelloPhil)) as HelloPhil [];
		Light light = FindObjectOfType<Light> () as Light;
		light.transform.position = light_axes [0] * 20.0f;
		light.transform.LookAt (Vector3.zero);
		for (int i=1; i<6; i++) 
		{
			Light newlight= (Light)Instantiate(light);
			newlight.transform.position = light_axes[i]*20.0f;
			newlight.transform.LookAt(Vector3.zero);
		}

		ResetCube ();
		mMoves=0;
	}

	enum eRKLayers
	{
		RK_Layer_Y_Top,
		RK_Layer_Y_Middle,
		RK_Layer_Y_Bottom,
		RK_Layer_Y_All,

		RK_Layer_X_Left,
		RK_Layer_X_Middle,
		RK_Layer_X_Right,
		RK_Layer_X_All,

		RK_Layer_Z_Back,
		RK_Layer_Z_Middle,
		RK_Layer_Z_Front,
		RK_Layer_Z_All,
		
		RK_Num_Layers,
	};

	class RotationTarget
	{
		public eRKLayers mLayer;
		public float mTargetAngle;
		public float mCurrentAngle;
		public float mStartAngle;
		public float mCurrentTime;
		public float mTotalDeltas;

		public RotationTarget(eRKLayers layer, float angle_end, float angle_start)
		{
			mLayer=layer;
			mTargetAngle=angle_end;
			mCurrentAngle=angle_start;
			mStartAngle=angle_start;
			mCurrentTime=0.0f;
			mTotalDeltas=0.0f;
		}
	};

	private void AddRotationTarget(eRKLayers layer, float angle_end, float angle_start)
	{
		mTargets.Add (new RotationTarget (layer, angle_end,angle_start));
		mMoves++;
	}

	public void ResetMoveCounter()
	{
		mMoves=0;
	}

	private bool IsTargetLayer(HelloPhil cube, eRKLayers layer)
	{
		switch(layer)
		{
		case eRKLayers.RK_Layer_Y_Top: 		return(Mathf.Abs(cube.transform.position.y - 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_Y_Middle:	return(Mathf.Abs(cube.transform.position.y - 0.0f) < 0.1f);
		case eRKLayers.RK_Layer_Y_Bottom:	return(Mathf.Abs(cube.transform.position.y + 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_Y_All: 		return true;
		
		case eRKLayers.RK_Layer_X_Left:		return(Mathf.Abs(cube.transform.position.x - 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_X_Middle:	return(Mathf.Abs(cube.transform.position.x - 0.0f) < 0.1f);
		case eRKLayers.RK_Layer_X_Right:	return(Mathf.Abs(cube.transform.position.x + 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_X_All: 		return true;

		case eRKLayers.RK_Layer_Z_Back:		return(Mathf.Abs(cube.transform.position.z - 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_Z_Middle:	return(Mathf.Abs(cube.transform.position.z - 0.0f) < 0.1f);
		case eRKLayers.RK_Layer_Z_Front:	return(Mathf.Abs(cube.transform.position.z + 2.0f) < 0.1f);
		case eRKLayers.RK_Layer_Z_All: 		return true;

		}
		return false;
	}

	private Vector3 GetRotationAxis(eRKLayers layer)
	{
		switch(layer)
		{
		case eRKLayers.RK_Layer_Y_Top: 	
		case eRKLayers.RK_Layer_Y_Middle:
		case eRKLayers.RK_Layer_Y_Bottom:
		case eRKLayers.RK_Layer_Y_All:
			return Vector3.up;
			
		case eRKLayers.RK_Layer_X_Left:
		case eRKLayers.RK_Layer_X_Middle:
		case eRKLayers.RK_Layer_X_Right:
		case eRKLayers.RK_Layer_X_All:
			return Vector3.right;
			
		case eRKLayers.RK_Layer_Z_Back:
		case eRKLayers.RK_Layer_Z_Middle:
		case eRKLayers.RK_Layer_Z_Front:
		case eRKLayers.RK_Layer_Z_All:
			return Vector3.forward;
		}
		return Vector3.up;
	}

	private eRKLayers GetLayer_Y(Vector3 v)
	{
		if(v.y < -3.0f) return eRKLayers.RK_Layer_Y_All;
		if(v.y > 3.0f) return eRKLayers.RK_Layer_Y_All;
		if (v.y < -1.0f && v.y > -3.0f)	return eRKLayers.RK_Layer_Y_Bottom;
		if (v.y <  1.0f && v.y > -1.0f)	return eRKLayers.RK_Layer_Y_Middle;
		if (v.y <  3.0f && v.y > 1.0f)	return eRKLayers.RK_Layer_Y_Top;
		return eRKLayers.RK_Layer_Y_All; // default.
	}

	private eRKLayers GetLayer_X(Vector3 v)
	{
		if (v.z < -2.99f)
		{
			if (v.x < -3.0f)
					return eRKLayers.RK_Layer_X_All;
			if (v.x > 3.0f)
					return eRKLayers.RK_Layer_X_All;
			if (v.x < -1.0f && v.x > -3.0f)
					return eRKLayers.RK_Layer_X_Right;
			if (v.x < 1.0f && v.x > -1.0f)
					return eRKLayers.RK_Layer_X_Middle;
			if (v.x < 3.0f && v.x > 1.0f)
					return eRKLayers.RK_Layer_X_Left;
			return eRKLayers.RK_Layer_X_All; // default.
				
		} 
		else 
		{
			if (v.z < -3.0f)
				return eRKLayers.RK_Layer_Z_All;
			if (v.z > 3.0f)
				return eRKLayers.RK_Layer_Z_All;
			if (v.z < -1.0f && v.z > -3.0f)
				return eRKLayers.RK_Layer_Z_Front;
			if (v.z < 1.0f && v.z > -1.0f)
				return eRKLayers.RK_Layer_Z_Middle;
			if (v.z < 3.0f && v.z > 1.0f)
				return eRKLayers.RK_Layer_Z_Back;
			return eRKLayers.RK_Layer_Z_All; // default.
		}
	}

	class KeyboardMapElement
	{
		public KeyboardMapElement(KeyCode code, eRKLayers layer) {mKeyCode=code;mLayer=layer;}
		public KeyCode mKeyCode;
		public eRKLayers mLayer;
	};

	static KeyboardMapElement [] keyboardMap = 
	{
		new KeyboardMapElement(KeyCode.L,eRKLayers.RK_Layer_X_Right),
		new KeyboardMapElement(KeyCode.V,eRKLayers.RK_Layer_X_Middle),
		new KeyboardMapElement(KeyCode.R,eRKLayers.RK_Layer_X_Left),

		new KeyboardMapElement(KeyCode.U,eRKLayers.RK_Layer_Y_Top),
		new KeyboardMapElement(KeyCode.H,eRKLayers.RK_Layer_Y_Middle),
		new KeyboardMapElement(KeyCode.D,eRKLayers.RK_Layer_Y_Bottom),

		new KeyboardMapElement(KeyCode.F,eRKLayers.RK_Layer_Z_Front),
		new KeyboardMapElement(KeyCode.B,eRKLayers.RK_Layer_Z_Back),
		new KeyboardMapElement(KeyCode.M,eRKLayers.RK_Layer_Z_Middle),


		new KeyboardMapElement(KeyCode.I,eRKLayers.RK_Layer_X_All),
		new KeyboardMapElement(KeyCode.O,eRKLayers.RK_Layer_Y_All),
		new KeyboardMapElement(KeyCode.P,eRKLayers.RK_Layer_Z_All),

	};

	class SelectionSetElement
	{
		public SelectionSetElement(HelloPhil cube, Transform t) {mCube=cube;OriginalPosition=t.position;OriginalRotation=t.rotation;}
		public HelloPhil mCube;
		public Vector3 OriginalPosition;
		public Quaternion OriginalRotation;
	};

	class SelectionSet
	{
		public List<SelectionSetElement> mSelectionSet = new List<SelectionSetElement>();
		public void Add(SelectionSetElement e) {mSelectionSet.Add (e);}
		public void Clear() {mSelectionSet.Clear ();}
		public eRKLayers mSelectedLayer;
		public GameObject mSelectedCube;
		public RaycastHit mRaycastHit;
		public Vector3 mMouseSelected;

		public enum States
		{
			WaitingToPickAxis,
			Spinning,
		};
		private States mState;
		public void SetState(States s) {mState = s;}
		public States GetState() {return mState;}
		public enum MouseCheck
		{
			Y,
			X,
		};
		public MouseCheck mMouseCheck;
		public float GetMouseDelta(Vector3 pos)
		{
			if (mMouseCheck == MouseCheck.X)
				return mMouseSelected.x - pos.x;
			//if (mMouseCheck == MouseCheck.Y)
				return -mMouseSelected.y + pos.y; // y-coords are reversed.
		}
	};

	private SelectionSet mSelectionSet = null;

	// Update is called once per frame
	void Update () 
	{
		// check if we've finished randomizing.
		if(mIsRandomizing && mTargets.Count == 0)
		{
			mIsRandomizing = false;
		}
		
		for (int i=0; i<keyboardMap.Length; i++)
		{
			if (Input.GetKeyDown (keyboardMap [i].mKeyCode))
			{
				float angle = 90.0f;
				if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
						angle = -90.0f;
				if(mSelectionSet == null) // if not currently rotating with mouse.
					AddRotationTarget (keyboardMap [i].mLayer, angle,0.0f);
			}
		}

		if (Input.GetKeyDown (KeyCode.Z))
		{
			ResetCube();
		}

		if(Input.GetKeyDown (KeyCode.Q))
		{
			ScrambleCube();
		}

		if(Input.GetKeyDown (KeyCode.K))
		{
			IsSolved();
		}

		if (Input.GetMouseButton (0))
		{
			if(mTargets.Count == 0) // if not currently finishing a rotation generated by a keypress of mouse up.
			{
				if(mSelectionSet == null)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit = new RaycastHit();
					if(Physics.Raycast(ray,out hit) && hit.transform.gameObject.name == "Cube")
					{
						// record what cube we clicked, and then wait for the user to pick an axis by moving
						// within 10 pixels horizontally or vertically.
						mSelectionSet = new SelectionSet();
						mSelectionSet.mMouseSelected=Input.mousePosition;
						mSelectionSet.SetState(SelectionSet.States.WaitingToPickAxis);
						mSelectionSet.mSelectedCube=hit.transform.gameObject;
						mSelectionSet.mRaycastHit=hit;
					}
					else
					{
						// picked nothing (or a hit plane), so we interpret this as a rotate all.
						mSelectionSet = new SelectionSet();
						mSelectionSet.mMouseSelected=Input.mousePosition;
						mSelectionSet.SetState(SelectionSet.States.WaitingToPickAxis);
						mSelectionSet.mSelectedCube=null;
						mSelectionSet.mRaycastHit=hit;
					}
				}
				if(mSelectionSet != null)
				{
					if(mSelectionSet.GetState() == SelectionSet.States.WaitingToPickAxis)
					{
						float delta_x = Mathf.Abs (mSelectionSet.mMouseSelected.x-Input.mousePosition.x);
						float delta_y = Mathf.Abs (mSelectionSet.mMouseSelected.y-Input.mousePosition.y);
						if(delta_x > 10.0f || delta_y > 10.0f)
						{
							if(mSelectionSet.mSelectedCube != null) // then we hit a cube.
							{
								if(delta_x > delta_y) // user has chosen horizontal.
								{
									mSelectionSet.mSelectedLayer = GetLayer_Y(mSelectionSet.mRaycastHit.point);
									mSelectionSet.mMouseCheck = SelectionSet.MouseCheck.X;
								}
								else
								{
									mSelectionSet.mSelectedLayer = GetLayer_X(mSelectionSet.mRaycastHit.point);
									mSelectionSet.mMouseCheck = SelectionSet.MouseCheck.Y;
								}
							}
							else // then we hit nothing, so rotate whole cube.
							{
								if(delta_x > delta_y)
								{
									// horizontal movement is always about y-axis
									mSelectionSet.mSelectedLayer=eRKLayers.RK_Layer_Y_All;
									mSelectionSet.mMouseCheck = SelectionSet.MouseCheck.X;
									
								}
								else
								{
									// vertical movement is either X or Z, depending on which detection plane 
									// was hit by the mouse.
									if(mSelectionSet.mRaycastHit.transform != null && mSelectionSet.mRaycastHit.transform.gameObject.name == "LeftHitPlane")
									{
										mSelectionSet.mSelectedLayer=eRKLayers.RK_Layer_X_All;
									}
									else if(mSelectionSet.mRaycastHit.transform != null && mSelectionSet.mRaycastHit.transform.gameObject.name == "RightHitPlane")
									{
										mSelectionSet.mSelectedLayer=eRKLayers.RK_Layer_Z_All;
									}
									else
									{
										// default if can't figure out where hit.
										mSelectionSet.mSelectedLayer=eRKLayers.RK_Layer_Z_All;
									}
									mSelectionSet.mMouseCheck = SelectionSet.MouseCheck.Y;
								}
							}
							
							for (int i=0; i<mCubes.Length; i++) 
							{
								if(IsTargetLayer(mCubes[i],mSelectionSet.mSelectedLayer))
								{
									mSelectionSet.Add (new SelectionSetElement(mCubes[i],mCubes[i].transform));
								}
							}
							mSelectionSet.SetState (SelectionSet.States.Spinning);
						}
					}

					if(mSelectionSet.GetState() == SelectionSet.States.Spinning)
					{
						float delta = mSelectionSet.GetMouseDelta(Input.mousePosition);//
						float angle = 90.0f*(delta)/100.0f;
						if(angle > 90.0f) angle=90.0f;
						if(angle < -90.0f) angle=-90.0f;
						Vector3 rotation_axis = GetRotationAxis(mSelectionSet.mSelectedLayer);
						Quaternion rotation = Quaternion.AngleAxis(angle,rotation_axis);
						foreach(SelectionSetElement e in mSelectionSet.mSelectionSet)
						{
							Vector3 ev = e.OriginalPosition - Vector3.zero;
							e.mCube.transform.position=rotation*ev;
							e.mCube.transform.rotation=rotation*e.OriginalRotation;
						}
					}
				}
			}
		}
		else
		{
			if(mSelectionSet != null)
			{
				// only apply the final rotation if player made it past the initial 10pixel threshold.
				if(mSelectionSet.GetState() == SelectionSet.States.Spinning)
				{
					float delta = mSelectionSet.GetMouseDelta(Input.mousePosition);//
					float angle = 90.0f*(delta)/100.0f;
					if(angle < -90.0f) angle=-90.0f;
					if(angle > 90.0f) angle=90.0f;

					Vector3 rotation_axis = GetRotationAxis(mSelectionSet.mSelectedLayer);
					Quaternion rotation = Quaternion.AngleAxis(angle,rotation_axis);
					foreach(SelectionSetElement e in mSelectionSet.mSelectionSet)
					{
						Vector3 ev = e.OriginalPosition - Vector3.zero;
						e.mCube.transform.position=rotation*ev;
						e.mCube.transform.rotation=rotation*e.OriginalRotation;
					}
					float target_angle=0.0f;
					if(angle < -22.5f) target_angle = -90.0f;
					if(angle > 22.5f) target_angle=90.0f;
					
					AddRotationTarget(mSelectionSet.mSelectedLayer,target_angle,angle);
				}
				// whether or not we picked a direction, clear sleection set.
				mSelectionSet.Clear();
				mSelectionSet = null;
			}
		}

		UpdateTarget ();
	}

	private void UpdateTarget()
	{
		if (mTargets.Count == 0)
			return;;

		RotationTarget target = mTargets [0];
		if (target != null)
		{
			float animation_time = 0.15f; // i.e seconds to rotate something.
			target.mCurrentTime += Time.deltaTime;
			float lerp_t = 1.0f-((animation_time - target.mCurrentTime)/animation_time);
			float absolute_angle = Mathf.Lerp(target.mStartAngle,target.mTargetAngle,lerp_t);
			float delta_angle = absolute_angle-target.mCurrentAngle;
			target.mCurrentAngle=absolute_angle;
			target.mTotalDeltas+=delta_angle;

			Vector3 axis = GetRotationAxis(target.mLayer);

			for (int i=0; i<mCubes.Length; i++) 
			{
				if(IsTargetLayer (mCubes[i],target.mLayer))
					mCubes[i].transform.RotateAround(Vector3.zero,axis,delta_angle);
			}
			if(Mathf.Abs(target.mCurrentAngle - target.mTargetAngle)< 0.01f)
			{
				mTargets.RemoveAt(0);
			}
		}
	}


	void ResetCube()
	{
		for (int i=0; i<mCubes.Length; i++)
			mCubes [i].transform.rotation = Quaternion.identity;

		mCubes [0].transform.position = new Vector3(2,-2,-2);
		mCubes [1].transform.position = new Vector3(2,-2,0);
		mCubes [2].transform.position = new Vector3(2,-2,2);
		
		mCubes [3].transform.position = new Vector3(2,0,-2);
		mCubes [4].transform.position = new Vector3(2,0,0);
		mCubes [5].transform.position = new Vector3(2,0,2);
		
		mCubes [6].transform.position = new Vector3(2,2,-2);
		mCubes [7].transform.position = new Vector3(2,2,0);
		mCubes [8].transform.position = new Vector3(2,2,2);
		
		mCubes [9].transform.position = new Vector3(0,-2,-2);
		mCubes [10].transform.position = new Vector3(0,-2,0);
		mCubes [11].transform.position = new Vector3(0,-2,2);
		
		mCubes [12].transform.position = new Vector3(0,0,-2);
		mCubes [13].transform.position = new Vector3 (0, 0, 0);
		mCubes [14].transform.position = new Vector3(0,0,2);
		
		mCubes [15].transform.position = new Vector3(0,2,-2);
		mCubes [16].transform.position = new Vector3(0,2,0);
		mCubes [17].transform.position = new Vector3(0,2,2);
		
		mCubes [18].transform.position = new Vector3(-2,-2,-2);
		mCubes [19].transform.position = new Vector3(-2,-2,0);
		mCubes [20].transform.position = new Vector3(-2,-2,2);
		
		mCubes [21].transform.position = new Vector3(-2,0,-2);
		mCubes [22].transform.position = new Vector3(-2,0,0);
		mCubes [23].transform.position = new Vector3(-2,0,2);
		
		mCubes [24].transform.position = new Vector3(-2,2,-2);
		mCubes [25].transform.position = new Vector3(-2,2,0);
		mCubes [26].transform.position = new Vector3(-2,2,2);

		float scale = 2.0f;
		for (int i=0; i<mCubes.Length; i++) 
		{
			mCubes[i].transform.localScale=new Vector3(scale,scale,scale);
		}

		for (int i=0; i<mCubes.Length; i++)
		{
			mCubes[i].SetAllFaces(HelloPhil.eColors.TC_Black);

			if(mCubes[i].transform.position.y == 2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Top, HelloPhil.eColors.TC_White);
			
			if(mCubes[i].transform.position.y == -2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Bottom, HelloPhil.eColors.TC_Yellow);
			
			if(mCubes[i].transform.position.x == 2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Right, HelloPhil.eColors.TC_Blue);
			
			if(mCubes[i].transform.position.x == -2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Left, HelloPhil.eColors.TC_Green);
			
			if(mCubes[i].transform.position.z == 2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Front, HelloPhil.eColors.TC_Orange);
			
			if(mCubes[i].transform.position.z == -2)
				mCubes[i].SetFace (HelloPhil.eUnitCubeFaces.UC_Face_Back, HelloPhil.eColors.TC_Red);
			
		}
	}
	
	public void ScrambleCube()
	{
		mIsRandomizing=true;
		for(int i=0;i<50;i++)
		{
			eRKLayers layer = (eRKLayers)(Random.value*(int)eRKLayers.RK_Num_Layers);
			int index = (int)(Random.value*2.0f);
			float target_angle=90.0f;
			if(index == 0) target_angle=-90.0f;
			AddRotationTarget (layer,target_angle,0.0f);
		}
	}

	private bool check_face_same(HelloPhil cube, HelloPhil.eColors [] colors, HelloPhil.eUnitCubeFaces face)
	{
		HelloPhil.eColors color = cube.GetFaceColor(face);
		int index = System.Convert.ToInt32 (face); 
		if(colors[index] == HelloPhil.eColors.TC_Black)
			colors[index]=color;
		if(colors[index] != color)
		{
			Debug.Log (string.Format ("not solved {0} {1} != {2}",face.ToString (),colors[index].ToString (),color.ToString ()));
			return false;
		}
		return true;
	}
	
	public bool IsSolved()
	{
		HelloPhil.eColors [] colors = new HelloPhil.eColors[6] 
		{
			HelloPhil.eColors.TC_Black,
			HelloPhil.eColors.TC_Black,
			HelloPhil.eColors.TC_Black,
			HelloPhil.eColors.TC_Black,
			HelloPhil.eColors.TC_Black,
			HelloPhil.eColors.TC_Black,
		};
		
		//i.e does every face have all edges the same color.
		for (int i=0; i<mCubes.Length; i++)
		{
			if(mCubes[i].EqualsY(2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Top))
					return false;
			
			if(mCubes[i].EqualsY(-2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Bottom))
					return false;
			
			if(mCubes[i].EqualsX(2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Right))
					return false;
			
			if(mCubes[i].EqualsX(-2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Left))
					return false;
			
			if(mCubes[i].EqualsZ(2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Back))
					return false;

			if(mCubes[i].EqualsZ(-2))
				if(!check_face_same (mCubes[i],colors,HelloPhil.eUnitCubeFaces.UC_Face_Front))
					return false;
		}
		return true;
	}
}
