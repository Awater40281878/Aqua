using System.Collections;
using System.Collections.Generic;
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
	private GameObject InfoUI;
	private GameObject Select;
	private GameObject Item​;
	private GameObject Move​;
	private GameObject Back;
	private GameObject camara;
	private GameObject Date;
	private GameObject Ok;
	private GameObject Attack;
	private GameObject Wait;
	private List<GameObject> allUi = new List<GameObject>();
	public behaviorMod P_behavior;
	private Unit_map_Date Write_Date;
	public Unit_map_Date original_Date;
	private GameObject target;
	private Vector3 decisionLoc;
	
	private void Start()
	{
		P_behavior = behaviorMod.NotActed;
		InfoUI = GameObject.Find("InfoUI");
		UIMgr Ui = UIMgr.instance;
		Select = Ui.GetChild(InfoUI, 1);
		Item​ = Ui.GetChild(InfoUI, 2);
		Move​ = Ui.GetChild(InfoUI, 3);
		Back = Ui.GetChild(InfoUI, 4);
		camara = Ui.GetChild(InfoUI, 5);
		Date = Ui.GetChild(InfoUI, 6);
		Ok = Ui.GetChild(InfoUI, 7); 
		Attack = Ui.GetChild(InfoUI, 8);
		Wait = Ui.GetChild(InfoUI, 9);
		if (InfoUI!=null)
		{
			allUi.Add(Select);
			allUi.Add(Item​);
			allUi.Add(Move​);
			allUi.Add(Back);
			//allUi.Add(camara);
			//allUi.Add(Date);
			allUi.Add(Ok);
			allUi.Add(Attack);
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
		else if (uiObject == Attack)
			Attack​Event();
		else if (uiObject == Wait)
			Waitk​Event();
	}
	void SelectEvent()
	{
		
		P_behavior = behaviorMod.Acted;
		sortBattleUi(original_Date);
		//Debug.Log("Select");
	}
	void Move​​Event()
	{
		target =GameObject.Find(cursor_move.instance.FindUnitDate(0).date.name);
		
		//if (target)
		//{
		//	print(target.name);
		//}
		generator.instance.showRange(original_Date);
		P_behavior = behaviorMod.OnMoveing_first;
		sortBattleUi(original_Date);
		Routing_Protocol rp = Routing_Protocol.instance;
		rp.FindProtocol(target);
		//Debug.Log("Move​​​");

	}
	void BackEvent()
	{
		Debug.Log("Back​");
	}
	void Ok​Event()
	{
		//print(P_behavior);
		if (P_behavior == behaviorMod.OnMoveing_first|| P_behavior == behaviorMod.OnMoveing_second)
		{
			Routing_Protocol rp = Routing_Protocol.instance;
			
			int Step =  rp.FindProtocol(target);
			//資料修改
			Write_Date = original_Date;
			Write_Date.CanMoveCont--;
			Write_Date.MoveStep= Write_Date.MoveStep+ Step;
			Write_Date.loc = cursor_move.instance.GetCursorLoc();
			Write_UnitData(Write_Date);
			original_Date = Write_Date;

		}
		Debug.Log("Ok​​");
	}
	void Attack​Event()
	{
		Debug.Log("Attack​");
	}
	void Waitk​Event()
	{
		Debug.Log("Wait​");
	}
	public void sortBattleUi(Unit_map_Date Date)
	{
		//print(123);
		original_Date = Date;
		original_Date.loc = Date.loc;
		hideAllUi();
		Vector2 choiceUiLoc = new Vector2(817, -30);
		Vector2 infoUiLoc = new Vector2(-745, 227.5f);
		List<GameObject> choiceList = new List<GameObject>();
		if (P_behavior ==behaviorMod.NotActed)
		{
			choiceList.Add(Select);
		}
		if (P_behavior == behaviorMod.OnMoveing_first)
		{
			choiceList.Add(Ok);
		}
		if (P_behavior != behaviorMod.NotActed && P_behavior != behaviorMod.Idle && P_behavior != behaviorMod.OnMoveing_first)
		{

			//print(Date.MoveCont);
			//print(Date.CanMoveCont);
			if (Date.CanMoveCont>= Date.MoveCont)
			{
				choiceList.Add(Move);
			}
			if (Date.IsUesItem == false)
			{
				choiceList.Add(Item);
			}
			if (Date.IsAttack==false)
			{
				if (Date.EnemyInRange == true)
				{
					choiceList.Add(Attack);
				}
			}
			if (P_behavior == behaviorMod.OnMove)
			{
				choiceList.Add(Ok);
				choiceList.Add(Back);
			}
		}
		int num = 0;
		foreach (var list in choiceList)
		{ 
			RectTransform rt =  list.GetComponent<RectTransform>();
			rt.anchoredPosition = choiceUiLoc + new Vector2(0, -90* num);
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
			print(item.name);
		}
	}

	private void Write_UnitData(Unit_map_Date Date)
	{
		List<Unit_map_Date> Unitlist = Map_Unit_Mgr.instance.Player_Unit;
		Unit_map_Date temp_date = new Unit_map_Date(Vector3.zero,new Unit_Data("",0,0,0,0,0,0,0,0,0,0,0,0,0,0));
		foreach (var list in Unitlist)
		{
			if (Write_Date.date.name == list.date.name && Write_Date.loc == decisionLoc)
			{
				temp_date = list;
				Unitlist.Remove(temp_date);
				Unitlist.Add(Date);
			}
		}
	}
}
