
using System;
using UnityEngine;
[Serializable]
public class Unit_map_Date
{
	public Unit_Loc loc;
	public Unit_Data date;

	public Unit_map_Date(Unit_Loc _loc, Unit_Data _date)
	{
		this.loc = _loc;
		this.date = _date;
	}
}
