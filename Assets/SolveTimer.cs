using UnityEngine;
using System.Collections;

public class SolveTimer : MonoBehaviour {

	enum States 
	{
		WaitForStart,
		Randomizing,
		Inspection,
		Solving,
		Solved,
	};
	
	States mState;
	public GUIText mText;
	public GUITexture mButton;
	private SceneScript mScene;
	private float mTime;

	// Use this for initialization
	void Start () {
		mState=States.WaitForStart;
		mScene = SolveTimer.FindObjectOfType<SceneScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(mState == States.WaitForStart)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(mButton.HitTest(Input.mousePosition))
				{
					mScene.ScrambleCube();
					mState = States.Randomizing;
				}
			}
		}
		else if(mState == States.Randomizing)
		{
			if(!mScene.IsRandomizing)
			{
				mState = States.Inspection;
				mTime = Time.time+5.0f;
			}
		}
		else if(mState == States.Inspection)
		{
			float seconds = mTime - Time.time;
			if(seconds > 0.0f)
			{
				mText.text = System.Convert.ToInt32(seconds+1).ToString();
				if(seconds < 1)
					mText.color=Color.red;
				else 
					mText.color=Color.white;
			}
			else
			{
				mState = States.Solving;
				mTime=Time.time;
			}
		}
		else if(mState == States.Solving)
		{
			int tseconds=System.Convert.ToInt32(Time.time-mTime);
			int minutes = tseconds/60;
			int seconds=tseconds%60;
			
			mText.text = string.Format ("{0}:{1}",minutes,seconds.ToString ("D2"));
			if(Input.GetMouseButtonDown(0))
			{
				if(mButton.HitTest(Input.mousePosition))
				{
					mState = States.Solved;
				}
			}
		}
		if(mState == States.Solved)
		{
			mState = States.WaitForStart;
		}
	}
}
