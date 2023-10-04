using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

public class battleUIMgr : MonoBehaviour
{
	public static battleUIMgr instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	// Start is called before the first frame update
	public GameObject InfoUI;
	public GameObject Select;
	public GameObject Item​;
	public GameObject Move​;
	public GameObject Back;
	public GameObject camara;
	public GameObject Date;
	public GameObject Ok;
	public GameObject Action​;
	public GameObject Wait;
	public List<GameObject> allUi = new List<GameObject>();
	public behaviorMod P_behavior;
	public behaviorMod last_behavior;
	private Unit_map_Date Write_Date;
	public Unit_map_Date original_Date;
	public GameObject target;
	//public Vector3 lastLoc;
	public Unit_map_Date lastdate;
	private Vector3 decisionLoc;


	private void Start()
	{
		P_behavior = behaviorMod.NotActed;
		last_behavior = behaviorMod.NotActed;
		InfoUI = GameObject.Find("InfoUI");
		UIMgr Ui = UIMgr.instance;
		Select = Ui.GetChild(InfoUI, 2, 1);
		Item​ = Ui.GetChild(InfoUI, 2, 2);
		Move​ = Ui.GetChild(InfoUI, 2, 3);
		Back = Ui.GetChild(InfoUI, 2, 4);
		//camara = Ui.GetChild(InfoUI, 2, 5);
		//Date = Ui.GetChild(InfoUI, 2, 5);
		Ok = Ui.GetChild(InfoUI, 2, 5);
		Action​ = Ui.GetChild(InfoUI, 2, 6);
		Wait = Ui.GetChild(InfoUI, 2, 7);
		if (InfoUI != null)
		{
			allUi.Add(Select);
			allUi.Add(Item​);
			allUi.Add(Move​);
			allUi.Add(Back);
			//allUi.Add(camara);
			//allUi.Add(Date);
			allUi.Add(Ok);
			allUi.Add(Action​);
			allUi.Add(Wait);
		}
		SetEventOnButton();

	}
	void SetEventOnButton()
	{
		foreach (var uiObject in allUi)
		{
			EventTrigger eventTrigger = uiObject.GetComponent<EventTrigger>();

			if (eventTrigger != null)
			{
				// 清除之前的事件處理
				eventTrigger.triggers.Clear();

				// 新增 OnPointerClick 事件處理
				EventTrigger.Entry clickEntry = new EventTrigger.Entry();
				clickEntry.eventID = EventTriggerType.PointerClick;
				clickEntry.callback.AddListener((data) => { OnClick(uiObject); });
				eventTrigger.triggers.Add(clickEntry);
			}
			else
			{
				Debug.LogWarning($"The GameObject {uiObject.name} does not have an EventTrigger component.");
			}
		}
	}
	void OnClick(GameObject uiObject)
	{
		if (uiObject == Select)
			SelectEvent();
		else if (uiObject == Move​)
			Move​​Event();
		else if (uiObject == Back)
			BackEvent();
		else if (uiObject == Ok)
			Ok​Event();
		else if (uiObject == Action​)
			Attack​Event();
		else if (uiObject == Wait)
			Waitk​Event();
	}
	void SelectEvent()
	{
		last_behavior = P_behavior;
		P_behavior = behaviorMod.Acted;
		sortBattleUi(original_Date);
		//Debug.Log("Select");
	}
	void Move​​Event()
	{
		lastdate = original_Date;
		print(lastdate.loc);
		target = GameObject.Find(cursor_move.instance.FindUnitDate(0).date.name);
		generator.instance.showRange(original_Date);
		last_behavior = P_behavior;
		P_behavior = behaviorMod.OnMoveing_first;
		sortBattleUi(original_Date);
		Routing_Protocol rp = Routing_Protocol.instance;
		rp.FindProtocol(target);
	}
	void BackEvent()
	{
		//P_behavior = last_behavior;
		SetCursorToOrigin();
		switch (P_behavior)
		{
			case behaviorMod.NotActed:
				break;
			case behaviorMod.Acted:
				if (last_behavior == behaviorMod.OnMoveing_first)
				{
					target.transform.position = lastdate.loc;
					cursor_move.instance.SetCusorLoc(lastdate.loc);
					Write_UnitData(lastdate);
				}
				P_behavior = behaviorMod.NotActed;
				break;
			case behaviorMod.Idle:
				break;
			case behaviorMod.OnMove:
				P_behavior = behaviorMod.Acted;
				break;
			case behaviorMod.OnMoveing_first:
				generator.instance.DestroyNode();
				P_behavior = behaviorMod.Acted;
				break;
			case behaviorMod.OnMoveing_second:
				P_behavior = behaviorMod.Acted;
				break;
			case behaviorMod.OnDiscussion:
				break;
			case behaviorMod.OnUesitem:
				break;
			case behaviorMod.choiceSkills_mode:
				break;
			default:
				break;
		}
		sortBattleUi(original_Date);
		Debug.Log("Back​");

	}
	void Ok​Event()
	{
		if (P_behavior == behaviorMod.OnMoveing_first || P_behavior == behaviorMod.OnMoveing_second)
		{
			last_behavior = behaviorMod.OnMoveing_first;

			generator.instance.DestroyNode();
			Routing_Protocol rp = Routing_Protocol.instance;
			Vector3 endloc = cursor_move.instance.GetCursorLoc();
			int Step = rp.FindProtocol(target);
			//資料修改
			Write_Date = original_Date;
			Write_Date.CanMoveCont--;
			Write_Date.MoveStep = Write_Date.MoveStep + Step;
			Write_Date.loc = endloc;
			Write_UnitData(Write_Date);
			original_Date = Write_Date;


		}
		//Debug.Log("Ok​​");
	}
	void Attack​Event()
	{
		Debug.Log("Attack​");
	}
	void Waitk​Event()
	{

		Write_Date = original_Date;
		Write_Date.Idle = true;
		Write_UnitData(Write_Date);
		sortBattleUi(original_Date);
		P_behavior = behaviorMod.NotActed;
		last_behavior = behaviorMod.NotActed;
		if (battleRound.instance.ClickIdle(1))
		{
			//print("Allidle");
			battleRound.instance.SetPhase(2);
		}
		//original_Date = null;

		//Debug.Log("Wait​");
	}
	public void sortBattleUi(Unit_map_Date Date)
	{
		//print(last_behavior);
		original_Date = null;
		original_Date = Date;
		original_Date.loc = Date.loc;
		hideAllUi();
		Vector2 choiceUiLoc = new Vector2(650, -30);
		Vector2 infoUiLoc = new Vector2(-745, 227.5f);
		List<GameObject> choiceList = new List<GameObject>();

		if (P_behavior == behaviorMod.NotActed)
		{
			choiceList.Add(Select);

		}
		if (P_behavior == behaviorMod.OnMoveing_first || P_behavior == behaviorMod.OnMoveing_second)
		{
			choiceList.Add(Ok);

		}
		if (P_behavior != behaviorMod.NotActed && P_behavior != behaviorMod.Idle && P_behavior != behaviorMod.OnMoveing_first)
		{

			if (Date.CanMoveCont >= Date.MoveCont && Date.MoveStep <= Date.date.Mov)
			{
				choiceList.Add(Move);
			}
			if (Date.IsUesItem == false)
			{
				choiceList.Add(Item);
			}
			if (Date.IsAttack == false)
			{
				if (Date.EnemyInRange == true)
				{
					choiceList.Add(Action​);
				}
			}
			if (P_behavior == behaviorMod.OnMove)
			{
				choiceList.Add(Ok);
				choiceList.Add(Back);
			}
			if (P_behavior != behaviorMod.NotActed)
			{
				choiceList.Add(Back);
			}
			if (P_behavior != behaviorMod.Idle)
			{
				choiceList.Add(Wait);
			}
		}
		if (Date.Idle)
		{
			hideAllUi();
			return;
		}
		int num = 0;
		foreach (var list in choiceList)
		{
			RectTransform rt = list.GetComponent<RectTransform>();
			rt.anchoredPosition = choiceUiLoc + new Vector2(0, -90 * num);
			num++;
		}
	}
	public void hideAllUi()
	{
		Vector2 hide_Loc = new Vector2(1587, -30);
		foreach (var item in allUi)
		{
			RectTransform rt = item.GetComponent<RectTransform>();
			rt.anchoredPosition = hide_Loc;
		}
	}
	public void Write_UnitData(Unit_map_Date Date)
	{
		List<Unit_map_Date> Unitlist = Map_Unit_Mgr.instance.Player_Unit;
		Unit_map_Date temp_date = new Unit_map_Date(Vector3.zero, new Unit_Data("", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
		foreach (var list in Unitlist)
		{
			if (Write_Date.date.name == list.date.name && Write_Date.loc == decisionLoc)
			{
				temp_date = list;
				Unitlist.Remove(temp_date);
				Unitlist.Add(Date);
				original_Date = list;
			}
		}
	}
	private void SetCursorToOrigin()
	{
		cursor_move.instance.SetCusorLoc(original_Date.loc);
	}
}
