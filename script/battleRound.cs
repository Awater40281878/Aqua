using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class battleRound : MonoBehaviour
{	

	public static battleRound instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	///<summary>
	/// 1.player 2.enemy 3.ally
	/// </summary>
	public int Phase = 0;
	public int Count = 0;
	public void SetPhase(int faction)
	{
		switch (faction)
		{	
			case 1:
				PYPhase();
				break;
			case 2:
				EYPhase();
				break;
			default:
				break;
		}
	}
	private void EYPhase()
	{
		Image img = GameObject.Find("UIanime").transform.GetChild(0).GetComponent<Image>();
		img.sprite = ImgMgr.instance.BattleUIanime.EnemyTurn;
		myani.instance.Start_Battle();
		Phase = 2;
		//print("startEnemyPhase");
	}
	public void StartEY()
	{
		GameObject EY = GameObject.Find("Enemy_Unit");
		if (Count >= EY.transform.childCount)
		{
			Count = 0;
			SetPhase(1);
			return;
		}
		Unit_map_Date targetDate = null;
		foreach (var item in Map_Unit_Mgr.instance.Enemy_Unit)
		{
			if (item.date.name == EY.transform.GetChild(Count).gameObject.name)
			{
				targetDate = item;
			}
		}
		if (targetDate==null)
		{
			print("§ä¤£¨ì");
		}
		Routing_Protocol.instance.FindProtocol_EY(EY.transform.GetChild(Count).gameObject, targetDate.date.Mov);
		Count++;
	}
	private void PYPhase()
	{
		foreach (var item in Map_Unit_Mgr.instance.Player_Unit)
		{
			item.CanMoveCont = 1;
			item.MoveCont = 0;
			item.MoveStep = 0;
			item.Idle = false;
		}
		Image img = GameObject.Find("UIanime").transform.GetChild(0).GetComponent<Image>();
		img.sprite = ImgMgr.instance.BattleUIanime.PlayerTurn;
		myani.instance.Start_Battle();
		Phase = 1;
		//print("startPYPhase");
	}

	public bool ClickIdle(int faction)
	{
		List<Unit_map_Date> date = new List<Unit_map_Date>();
		switch (faction)
		{
			case 1:
				date = Map_Unit_Mgr.instance.Player_Unit;
				break;
			case 2:
				date = Map_Unit_Mgr.instance.Enemy_Unit;
				break;
			default:
				break;
		}
		bool AllIdle = false;
		foreach (var item in date)
		{
			if (item.Idle == true)
			{
				AllIdle = true;
			}
			else
			{
				return false;
			}
		}
		return AllIdle;
	}
}
