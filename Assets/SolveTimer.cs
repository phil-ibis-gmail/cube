using UnityEngine;
using System.Collections;
using System.Threading;

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
	public GUIText mTimeText;
	public GUIText mMovesText;
	public GUITexture mButton;
	public GUITexture mMethodButton;
	public GUIText mMethodText;
	private SceneScript mScene;
	private float mTime;
	private string mTagString;

	// Use this for initialization
	void Start () {
		mState=States.WaitForStart;
		mScene = SolveTimer.FindObjectOfType<SceneScript>();
		mTagString = "phil";
		
	}
	
	void OnGUI() {
		mTagString = GUI.TextField(new Rect(70, 70, 200, 20), mTagString, 25);
	}
	
	// Update is called once per frame
	void Update () {
	
		// click to toggle method.
		if(Input.GetMouseButtonDown(0))
		{
			if(mMethodButton.HitTest (Input.mousePosition))
			{
				if(mMethodText.text == "white_cross")
					mMethodText.text="roux";
				else if(mMethodText.text == "roux")
					mMethodText.text = "white_cross";
			}
		}
	
		if(mState == States.WaitForStart)
		{
			mTimeText.text = "0";
			mMovesText.text = "0";
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
			mTimeText.text = "0";
			mMovesText.text = "0";
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
				mTimeText.text = System.Convert.ToInt32(seconds+1).ToString();
				mMovesText.text = "0";
				if(seconds < 1)
					mTimeText.color=Color.red;
				else 
					mTimeText.color=Color.white;
			}
			else
			{
				mState = States.Solving;
				mTime=Time.time;
				mScene.ResetMoveCounter();
			}
		}
		else if(mState == States.Solving)
		{
			int tseconds=System.Convert.ToInt32(Time.time-mTime);
			int minutes = tseconds/60;
			int seconds=tseconds%60;
			
			mTimeText.text = string.Format ("{0}:{1}",minutes,seconds.ToString ("D2"));
			mMovesText.text = string.Format ("{0}",mScene.Moves);
			if(Input.GetMouseButtonDown(0))
			{
				if(mButton.HitTest(Input.mousePosition))
				{
					postResults(tseconds,mTagString,mScene.Moves,mScene.IsSolved());
					mState = States.Solved;
				}
			}
			
		}
		if(mState == States.Solved)
		{
			mState = States.WaitForStart;
		}
	}
	
	void postResults(int seconds, string tag, int moves,bool solved)
	{
		string addr="http://www.ibis-stuff.ca/glasses/rubik.php";
		string url_tag = WWW.EscapeURL(tag);
		string method=WWW.EscapeURL (mMethodText.text);
		string is_solved=solved?"solved":"not_solved";
		string url = string.Format ("{0}?seconds={1}&tag={2}&moves={3}&solved={4}&method={5}",addr,seconds,url_tag,moves,is_solved,mMethodText.text);

		(new Thread( () => 
		{
			var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
			request.Method="PUT";
			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
			string returnString = response.StatusCode.ToString();
		})).Start();
	}
}
