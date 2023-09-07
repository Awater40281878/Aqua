
using System;
using UnityEngine;
[Serializable]
public class Unit_map_Date
{
	public Vector3 loc;
	public Unit_Data date;
	public bool IsUesItem = false;
	public bool IsAttack = false;
	public int CanMoveCont = 1;
	public int MoveCont = 0;
	public int MoveStep = 0;
	public bool EnemyInRange = false;
	public bool Idle = false;



	public Unit_map_Date(Vector3 _loc, Unit_Data _date)
	{
		this.loc = _loc;
		this.date = _date;
	}
}
