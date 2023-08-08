
using System;
using UnityEngine;
[Serializable]
public class Unit_Loc
{
	public int x;
	public int y;
	public int z;
	public Unit_Loc(Vector3 loc)
	{
		this.x = (int)loc.x;
		this.y = (int)loc.y;
		this.z = (int)loc.z;

	}
}
