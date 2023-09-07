using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetUpUI : MonoBehaviour
{
	public static SetUpUI instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public TMP_Dropdown Unit;
	public TextMeshProUGUI name;
	public TextMeshProUGUI job;
	public TextMeshProUGUI hp;
	public TextMeshProUGUI PAttack;
	public TextMeshProUGUI MAttack;
	public TextMeshProUGUI PDef;
	public TextMeshProUGUI MDef;
	public TextMeshProUGUI Agi;
	public TextMeshProUGUI Mov;
	public TextMeshProUGUI Luk;
	public TextMeshProUGUI Lightning;
	public TextMeshProUGUI Ice;
	public TextMeshProUGUI Fire;
	public TextMeshProUGUI Wind;
	public TMP_InputField Level;
	public TMP_Dropdown dd;
	public GameObject Player_Unit;
	public GameObject Enemy_Unit;
	public GameObject Ally_Unit;

	public string unitname;
	private void Start()
	{
		UIMgr Ui = UIMgr.instance;
		GameObject au = GameObject.Find("All_Unit");
		if (au == null)
		{
			return;
			Debug.Log("錯誤");
		}
		Player_Unit = Ui.GetChild(au, 1);
		Enemy_Unit = Ui.GetChild(au, 2);
		Ally_Unit = Ui.GetChild(au, 3);

		GameObject suu = GameObject.Find("SetAIUnitUI");
		if (suu == null)
		{
			return;
			Debug.Log("錯誤");
		}
		//dd = GameObject.Find("sauu_faction").GetComponent<TMP_Dropdown>();
		//Unit = GameObject.Find("sauu_Dropdown").GetComponent<TMP_Dropdown>();
		dd = Ui.GetChild(suu,16).GetComponent<TMP_Dropdown>();
		name = Ui.GetChild(suu, 1, 1).GetComponent<TextMeshProUGUI>();
		job = Ui.GetChild(suu, 2, 1).GetComponent<TextMeshProUGUI>();
		hp = Ui.GetChild(suu, 3, 2).GetComponent<TextMeshProUGUI>();
		Level = Ui.GetChild(suu, 4, 2).GetComponent<TMP_InputField>();
		PAttack = Ui.GetChild(suu, 5, 2).GetComponent<TextMeshProUGUI>();
		MAttack = Ui.GetChild(suu, 6, 2).GetComponent<TextMeshProUGUI>();
		Agi = Ui.GetChild(suu, 7, 2).GetComponent<TextMeshProUGUI>();
		PDef = Ui.GetChild(suu, 8, 2).GetComponent<TextMeshProUGUI>();
		MDef = Ui.GetChild(suu, 9, 2).GetComponent<TextMeshProUGUI>();
		Luk = Ui.GetChild(suu, 10, 2).GetComponent<TextMeshProUGUI>();
		Lightning = Ui.GetChild(suu, 11, 2).GetComponent<TextMeshProUGUI>();
		Ice = Ui.GetChild(suu, 12, 2).GetComponent<TextMeshProUGUI>();
		Fire = Ui.GetChild(suu, 13, 2).GetComponent<TextMeshProUGUI>();
		Wind = Ui.GetChild(suu, 14, 2).GetComponent<TextMeshProUGUI>();
		Mov = Ui.GetChild(suu, 15, 1).GetComponent<TextMeshProUGUI>();
		Level.onEndEdit.AddListener(OnLevelEndEdit);
		//HandleUnitValueChanged(Unit.value); 
	}
	
	public void OnLevelEndEdit(string levelText)
	{
		HandleUnitValueChanged(unitname);
	}
	public void HandleUnitValueChanged(string unitname)
	{
		AI_Unit_Manage AIMgr = AI_Unit_Manage.instance;
		List<int> Value = new List<int>(7);
		
		// 尋找與選擇的選項相符的 Unit_Data
		Unit_Data selectedUnitData = AIMgr.Data.Find(unitData => unitData.name == unitname);
		// 更新 Level 文本
		//Debug.Log("測試");
		//Debug.Log(selectedUnitData);

		Value = Growth_formula(selectedUnitData.job, Int32.Parse(Level.text));

		//Debug.Log(Int32.Parse(Level.text));
		if (selectedUnitData != null)
		{
			// 找到匹配的 Unit_Data
			//Debug.Log("找到相符的 Unit_Data，名稱：" + selectedUnitData.Unit_name);
			// 在這裡處理相符的 Unit_Data
		}
		else
		{
			// 找不到相符的 Unit_Data
			//Debug.Log("找不到相符的 Unit_Data");
			return;
		}

		// 更新 UI 文本
		name.text = selectedUnitData.name;
		job.text = selectedUnitData.job.ToString();
		hp.text = (selectedUnitData.Hp + Value[0]).ToString();
		PAttack.text = (selectedUnitData.PAttack + Value[1]).ToString();
		MAttack.text = (selectedUnitData.MAttack + Value[2]).ToString();
		PDef.text = (selectedUnitData.PDef + Value[3]).ToString();
		MDef.text = (selectedUnitData.MDef + Value[4]).ToString();
		Agi.text = (selectedUnitData.Agi + Value[5]).ToString();
		Mov.text = selectedUnitData.Mov.ToString();
		Luk.text = (selectedUnitData.Luk + Value[6]).ToString();
		Lightning.text = selectedUnitData.Lightning.ToString();
		Ice.text = selectedUnitData.Ice.ToString();
		Fire.text = selectedUnitData.Fire.ToString();
		Wind.text = selectedUnitData.Wind.ToString();

		//Range_min.text = selectedUnitData.Range_min.ToString();
		//Range_max.text = selectedUnitData.Range_max.ToString();
	}


	private List<int> Growth_formula(Job_Type job, int level)
	{
		List<int> Value = new List<int>(7);
		List<int> oneList = new List<int>(7);
		List<int> fiveList = new List<int>(7);
		switch (job)
		{
			case Job_Type.Null:
				return null;
			case Job_Type.Saber:
				oneList = GetGrowValue_One(level, 1, 1, 0, 1, 0, 1, 1);
				fiveList = GetGrowValue_Five(level, 2, 2, 2, 0, 3, 0, 0);
				break;
			case Job_Type.Archer:
				oneList = GetGrowValue_One(level, 1, 1, 0, 0, 0, 1, 0);
				fiveList = GetGrowValue_Five(level, 1, 2, 2, 2, 3, 1, 3);
				break;
			case Job_Type.Tank:
				oneList = GetGrowValue_One(level, 1, 1, 0, 1, 0, 0, 0);
				fiveList = GetGrowValue_Five(level, 3, 1, 2, 1, 2, 1, 3);
				break;
			case Job_Type.Witch:
				oneList = GetGrowValue_One(level, 1, 0, 2, 0, 1, 0, 1);
				fiveList = GetGrowValue_Five(level, 1, 0, 1, 1, 1, 2, 0);
				break;
			case Job_Type.Scoundrel:
				oneList = GetGrowValue_One(level, 1, 1, 0, 0, 0, 1, 0);
				fiveList = GetGrowValue_Five(level, 2, 2, 2, 2, 3, 1, 2);
				break;
			case Job_Type.Berserker:
				oneList = GetGrowValue_One(level, 1, 1, 0, 1, 0, 0, 0);
				fiveList = GetGrowValue_Five(level, 3, 1, 2, 1, 2, 1, 2);
				break;
			case Job_Type.FireMagic:
				oneList = GetGrowValue_One(level, 1, 0, 1, 0, 1, 0, 0);
				fiveList = GetGrowValue_Five(level, 1, 2, 3, 1, 1, 2, 2);
				break;
			case Job_Type.Sagittarius:
				oneList = GetGrowValue_One(level, 1, 1, 0, 0, 0, 2, 0);
				fiveList = GetGrowValue_Five(level, 1, 3, 22, 2, 3, 1, 3);
				break;
			case Job_Type.SpawnPoint:
				oneList = GetGrowValue_One(level, 0, 0, 0, 0, 0, 0, 0);	
				fiveList = GetGrowValue_Five(level, 0, 0, 0, 0, 0, 0, 0);
				break;
		}
		Value = CombineLists(oneList, fiveList);
		return Value;
	}
	private List<int> GetGrowValue_One(int _level, int _hp, int _PAttack, int _MAttack, int _PDef, int _MDef, int _Agi, int _Luk)
	{
		List<int> Value = new List<int>(7);
		int num = 0;
		for (int i = 1; i <= _level; i++)
		{
			if (i != 1 && i % 5 != 0)
			{
				num++;
			}
		}
		Value.Add(_hp * num);
		Value.Add(_PAttack * num);
		Value.Add(_MAttack * num);
		Value.Add(_PDef * num);
		Value.Add(_MDef * num);
		Value.Add(_Agi * num);
		Value.Add(_Luk * num);

		// 在這裡可以根據需要添加其他成長數值的計算邏輯
		// 將計算結果添加到 Value 列表中
		//Debug.Log("每等額外加成" + num);
		return Value;
	}
	private List<int> GetGrowValue_Five(int _level, int _hp, int _PAttack, int _MAttack, int _PDef, int _MDef, int _Agi, int _Luk)
	{
		List<int> Value = new List<int>(7);
		int num = 0;
		for (int i = 1; i <= _level; i++)
		{
			if (i % 5 == 0 && i != 0)
			{
				num++;
			}
		}
		Value.Add(_hp * num);
		Value.Add(_PAttack * num);
		Value.Add(_MAttack * num);
		Value.Add(_PDef * num);
		Value.Add(_MDef * num);
		Value.Add(_Agi * num);
		Value.Add(_Luk * num);
		//Debug.Log("每5等額外加成" + num);
		// 在這裡可以根據需要添加其他成長數值的計算邏輯
		// 將計算結果添加到 Value 列表中

		return Value;
	}
	private List<int> CombineLists(List<int> list1, List<int> list2)
	{
		List<int> combinedList = new List<int>(list1.Count);

		for (int i = 0; i < list1.Count; i++)
		{
			combinedList.Add(list1[i] + list2[i]);
		}

		return combinedList;
	}
	
}

