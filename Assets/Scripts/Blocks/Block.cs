using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block
{
	public int width{
		get{return mWidth; } set{ mWidth = value; }
	}
	public int mWidth;

	public int length{
		get{return mLength; } set{ mLength = value; }
	}
	public int mLength;
	public int cost{ get; set; }
	public Block prevBlock{ get; set; }
//	public GameObject mObj{
//		get{return obj; } set{ obj = value; }
//	}
	public GameObject obj;
	public GameObject mObj{
		get{return obj; } set{ obj = value; }
	}

	public GameObject unit;
	public GameObject mUnit{
		get{return unit; } set{ unit = value; }
	}

}
