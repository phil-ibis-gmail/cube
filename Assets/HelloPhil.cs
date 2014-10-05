using UnityEngine;
using System.Collections;

public class HelloPhil : MonoBehaviour {

	public enum eColors
	{
		TC_Red,
		TC_Orange,
		TC_Green,
		TC_Blue,
		TC_White,
		TC_Yellow,
		TC_Black,
		TC_Num_Colors,
	};

	static public float w = 0.25f;
	private static Vector2 [,] TextureCoords = new Vector2 [7,4]
	{
		{new Vector2(0.0f*w,4.0f*w),new Vector2(1.0f*w,4.0f*w),new Vector2(0.0f*w,3.0f*w),new Vector2(1.0f*w,3.0f*w)},
		{new Vector2(1.0f*w,4.0f*w),new Vector2(2.0f*w,4.0f*w),new Vector2(1.0f*w,3.0f*w),new Vector2(2.0f*w,3.0f*w)},
		{new Vector2(2.0f*w,4.0f*w),new Vector2(3.0f*w,4.0f*w),new Vector2(2.0f*w,3.0f*w),new Vector2(3.0f*w,3.0f*w)},
		{new Vector2(2.0f*w,3.0f*w),new Vector2(3.0f*w,3.0f*w),new Vector2(2.0f*w,2.0f*w),new Vector2(3.0f*w,2.0f*w)},
		{new Vector2(1.0f*w,3.0f*w),new Vector2(2.0f*w,3.0f*w),new Vector2(1.0f*w,2.0f*w),new Vector2(2.0f*w,2.0f*w)},
		{new Vector2(0.0f*w,3.0f*w),new Vector2(1.0f*w,3.0f*w),new Vector2(0.0f*w,2.0f*w),new Vector2(1.0f*w,2.0f*w)},
		{new Vector2(3.0f*w,3.0f*w),new Vector2(4.0f*w,3.0f*w),new Vector2(3.0f*w,2.0f*w),new Vector2(4.0f*w,2.0f*w)},
	};

	public enum eUnitCubeFaces
	{
		UC_Face_Front,
		UC_Face_Back,
		UC_Face_Left,
		UC_Face_Right,
		UC_Face_Top,
		UC_Face_Bottom,
	};

	private Vector3 [] mFaceNormals = new Vector3[6]
	{
		new Vector3(0.0f,0.0f,-1.0f),
		new Vector3(0.0f,0.0f,1.0f),

		new Vector3(-1.0f,0.0f,0.0f),
		new Vector3(1.0f,0.0f,0.0f),
		
		new Vector3(0.0f,1.0f,0.0f),
		new Vector3(0.0f,-1.0f,0.0f),
	};

	private static int [,] CubeVertexIndexes = new int[6,4]
	{
		{2,3,0,1},
		{6,7,10,11},
		{19,17,16,18},
		{23,21,20,22},
		{4,5,8,9},
		{15,13,12,14},
	};

	void SetFaceToColor(Vector2 [] uvs, eUnitCubeFaces face, eColors color)
	{
		uvs [CubeVertexIndexes[System.Convert.ToInt32(face),0]] = TextureCoords[System.Convert.ToInt32(color),0];
		uvs [CubeVertexIndexes[System.Convert.ToInt32(face),1]] = TextureCoords[System.Convert.ToInt32(color),1];
		uvs [CubeVertexIndexes[System.Convert.ToInt32(face),2]] = TextureCoords[System.Convert.ToInt32(color),2];
		uvs [CubeVertexIndexes[System.Convert.ToInt32(face),3]] = TextureCoords[System.Convert.ToInt32(color),3];
	}

	public void SetFace(eUnitCubeFaces face, eColors color)
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] uv = new Vector2[mesh.vertices.Length];
		for (int i=0; i<uv.Length; i++)
			uv [i] = mesh.uv [i];
		SetFaceToColor (uv, face, color);
		mesh.uv = uv;
	}

	public void SetAllFaces(eColors color)
	{
		SetFace (eUnitCubeFaces.UC_Face_Front, color);
		SetFace (eUnitCubeFaces.UC_Face_Back, color);
		SetFace (eUnitCubeFaces.UC_Face_Left, color);
		SetFace (eUnitCubeFaces.UC_Face_Right, color);
		SetFace (eUnitCubeFaces.UC_Face_Top, color);
		SetFace (eUnitCubeFaces.UC_Face_Bottom, color);
	}
	
	public eColors GetFaceColor(eUnitCubeFaces face)
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		for(int i=0;i<6;i++)
		{
			Vector3 v0 = transform.TransformPoint(mesh.vertices[CubeVertexIndexes[i,0]]);
			Vector3 v1 = transform.TransformPoint(mesh.vertices[CubeVertexIndexes[i,1]]);
            Vector3 v2 = transform.TransformPoint(mesh.vertices[CubeVertexIndexes[i,2]]);
 	        Vector3 v3 = transform.TransformPoint(mesh.vertices[CubeVertexIndexes[i,3]]);
 			
			Vector3 v23=v2-v3;
			Vector3 v13=v1-v3;
			Vector3 n = Vector3.Cross(v23,v13).normalized;
			//Debug.Log (n.ToString()+mFaceNormals[System.Convert.ToInt32 (face)].ToString());
			if(Vector3.SqrMagnitude(n-mFaceNormals[System.Convert.ToInt32 (face)]) < 0.001f)
			{
				for(int j=0;j<System.Convert.ToInt32(eColors.TC_Num_Colors);j++)
				{
					if(TextureCoords[j,0] == mesh.uv[CubeVertexIndexes[i,0]])
					{
						return (eColors)j;
					}
				}
			}
		}
		return eColors.TC_Black;
	}
	
	public bool EqualsY(float value)
	{
		return Mathf.Abs(this.transform.position.y - value) < 0.01f;
	}
	
	public bool EqualsX(float value)
	{
		return Mathf.Abs(this.transform.position.x - value) < 0.01f;
	}
	
	public bool EqualsZ(float value)
	{
		return Mathf.Abs(this.transform.position.z - value) < 0.01f;
	}
	
	public bool IsTopLayer {get {return EqualsY(2.0f);}}
	public bool IsBottomLayer {get {return EqualsY(-2.0f);}}
	public bool IsLeftFace {get {return EqualsX(-2.0f);}}
	public bool IsRightFace {get {return EqualsX(2.0f);}}
}
