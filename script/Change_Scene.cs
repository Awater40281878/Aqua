using UnityEngine;

public class Change_Scene : MonoBehaviour
{
	public static Change_Scene instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			
		}
	}
	// Start is called before the first frame update
	GameObject generatorUI;
	GameObject CreateUnitUI;
	GameObject drawUI;	
	GameObject SetPlayerUnitUI;
	GameObject ScrollViewItem;
	//center UI
	Vector2 ChideLoc = new Vector2(-400, 18.25f);
	Vector2 CshowLoc = new Vector2(0, 18.25f);
	//botton UI
	Vector2 BhideLoc = new Vector2(-3400f, -181.76f);
	Vector2 BshowLoc = new Vector2(-1170f, -181.76f);
	/// <summary>
	/// 1.generator 2.draw 3.Unit
	/// </summary>
	public bool[] mode = new bool[3];
	private void Start()
	{
		generatorUI = GameObject.Find("generatorUI");
		drawUI = GameObject.Find("drawUI");
		CreateUnitUI = GameObject.Find("CreateUI");
		SetPlayerUnitUI = GameObject.Find("SetAIUnitUI");
		ScrollViewItem = GameObject.Find("Scroll_View_item");

	}
	void Click_Mode(int modenum)
	{	

		// 设置指定模式编号的bool值为true，其他bool值为false
		for (int i = 0; i < mode.Length; i++)
		{
			mode[i] = (i == (modenum - 1));
		}
		// 在这里使用更新后的mode数组进行后续操作
	}
	public int GetMode()
	{
		for (int i = 0; i < mode.Length; i++)
		{
			if (mode[i])
			{
				return i+1;
			}
		}

		// 如果没有找到等于true的布尔值，可以根据需要返回一个默认值或错误代码
		Debug.Log("沒有找到");
		return -1; // 返回-1表示未找到
	}

	public void generator()
	{
		Click_Mode(1);
		GetWight(1);
	}
	public void draw()
	{
		Click_Mode(2);
		GetWight(2);
	}
	public void Unit()
	{
		Click_Mode(3);
		GetWight(3);
	}
	private void GetWight(int index)
	{
		switch (index)
		{
			case 1:
				show_generator();
				break;
			case 2:
				show_draw();
				break;
			case 3:
				show_SetAIUnitList();
				break;
		}
	}

	public void show_generator()
	{
		GetRect(generatorUI).anchoredPosition = BshowLoc;
		GetRect(drawUI).anchoredPosition = BhideLoc;
		GetRect(CreateUnitUI).anchoredPosition = BhideLoc;
		GetRect(SetPlayerUnitUI).anchoredPosition = BhideLoc;

	}
	public void show_draw()
	{
		GetRect(drawUI).anchoredPosition = BshowLoc;
		GetRect(generatorUI).anchoredPosition = BhideLoc;
		GetRect(CreateUnitUI).anchoredPosition = BhideLoc;
		GetRect(SetPlayerUnitUI).anchoredPosition = BhideLoc;
	}

	public void show_Create()
	{
		GetRect(SetPlayerUnitUI).anchoredPosition = BhideLoc;
		GetRect(CreateUnitUI).anchoredPosition = BshowLoc;
		GetRect(drawUI).anchoredPosition = BhideLoc;
		GetRect(generatorUI).anchoredPosition = BhideLoc;
	}
	public void show_SetAIUnit(string name)
	{
		SetUpUI.instance.unitname = name;
		//Debug.Log(CreateUnitUI.name);
		UIMgr.instance.load_Unit();
		SetUpUI.instance.Level.text = "1"; // 設定初始值為1;
		GetRect(CreateUnitUI).anchoredPosition = BhideLoc;
		GetRect(drawUI).anchoredPosition = BhideLoc;
		GetRect(generatorUI).anchoredPosition = BhideLoc;
		GetRect(SetPlayerUnitUI).anchoredPosition = BshowLoc;
		//Unit_menu.instance.UpdateDropdownOptions();
		//SetUpUI.instance.HandleUnitValueChanged();
	}
	public void show_SetAIUnitList()
	{
		UIMgr.instance.load_Unit();
		GetRect(ScrollViewItem).anchoredPosition = CshowLoc;
		GetRect(drawUI).anchoredPosition = BhideLoc;
		GetRect(generatorUI).anchoredPosition = BhideLoc;
		UIMgr.instance.item_list_update();
	}
	public void hide_SetAIUnitList()
	{
		GetRect(ScrollViewItem).anchoredPosition = ChideLoc;
	}
	private RectTransform GetRect(GameObject target)
	{
		
		return target.GetComponent<RectTransform>();
	}
}
