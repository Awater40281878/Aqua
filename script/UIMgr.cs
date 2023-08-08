using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class UIMgr : MonoBehaviour
{
	public static UIMgr instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public GameObject Unit_template;
	public GameObject Unit_list_temp;
	/// <summary>
	/// 1編輯模式 2戰鬥模式 3家園模式
	/// </summary>
	public bool[] mode = new bool[3];
	// Start is called before the first frame update
	public void battle_mode()
	{
		GameObject.Find("EditUI").SetActive(false);
		Click_Mode(2);
		GameObject lui = GetChild(GameObject.Find("battleUI"), 1);
		lui.SetActive(true);
		GameObject ld = GetChild(GameObject.Find("battleUI"), 1, 2);
		TMP_Dropdown td = ld.GetComponent<TMP_Dropdown>();

		// 先清空選項列表
		td.ClearOptions();

		// 取得所有的 Episode 物件
		List<Episode> allEpisodes = Episode_Mgr.instance.All_Episode;

		// 建立一個新的選項列表
		List<string> dropdownOptions = new List<string>();

		// 將所有 Episode 的名稱加入選項列表
		foreach (Episode episode in allEpisodes)
		{
			dropdownOptions.Add(episode.name);
		}

		// 設置 TMP_Dropdown 的選項為新的選項列表
		td.AddOptions(dropdownOptions);

		// 可以在這裡選擇預設選項，例如：
		// td.value = 0;

		// 或者也可以在 TMP_Dropdown 的 Inspector 設定中選擇預設選項。

		// 重要：更新 Dropdown 的顯示，以顯示新的選項
		td.RefreshShownValue();
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

	public void save_node()
	{
		AStarNode[,] nodes = ASTarMgr.instance.nodes;
		string fileName = "Nodemap.txt";

		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 確保資料夾存在
		Directory.CreateDirectory(saveFolderPath);

		// 組合完整的檔案路徑
		string filePath = Path.Combine(saveFolderPath, fileName);

		using (StreamWriter writer = new StreamWriter(filePath))
		{
			int width = nodes.GetLength(0);
			int height = nodes.GetLength(1);

			// 寫入陣列大小資訊
			writer.WriteLine($"{width},{height}");

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					AStarNode node = nodes[x, y];

					// 將座標、類型和 Buff_Type 寫入文字檔
					string line = string.Format("{0},{1},{2},{3},{4}",
						node.x, node.y, node.z, node.type.ToString(), node.buff.ToString());
					writer.WriteLine(line);
				}
			}
		}

		Debug.Log("Nodes saved to file: " + filePath);
	}
	public void load_node()
	{
		string fileName = "Nodemap.txt";

		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 組合完整的檔案路徑
		string filePath = Path.Combine(saveFolderPath, fileName);

		// 確保檔案存在
		if (File.Exists(filePath))
		{
			using (StreamReader reader = new StreamReader(filePath))
			{
				// 讀取陣列大小資訊
				string sizeLine = reader.ReadLine();
				string[] sizeValues = sizeLine.Split(',');
				int width = int.Parse(sizeValues[0]);
				int height = int.Parse(sizeValues[1]);

				// 建立新的陣列
				ASTarMgr.instance.mapW = width;
				ASTarMgr.instance.mapH = height;
				AStarNode[,] nodes = new AStarNode[width, height];

				// 讀取座標、類型和 Buff_Type 資料並填入陣列
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						string line = reader.ReadLine();
						string[] values = line.Split(',');

						int nodeX = int.Parse(values[0]);
						int nodeY = int.Parse(values[1]);
						int nodeZ = int.Parse(values[2]);
						Node_Type node_Type = (Node_Type)Enum.Parse(typeof(Node_Type), values[3]);
						Buff_Type buff_Type = (Buff_Type)Enum.Parse(typeof(Buff_Type), values[4]);

						// 建立新的 AStarNode 並填入資料
						AStarNode node = new AStarNode(nodeX, nodeY, nodeZ, node_Type, buff_Type);
						nodes[x, y] = node;
					}
				}

				// 覆蓋 ASTarMgr.instance.nodes
				ASTarMgr.instance.nodes = nodes;

				// 輸出節點資料到 Debug.Log
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						AStarNode node = nodes[x, y];
						//Debug.Log($"Node ({node.x}, {node.y}, {node.z}): Type={node.type}, Buff={node.buff}");
					}
				}
			}

			generator.instance.generate_node(ASTarMgr.instance.nodes);
			//Debug.Log("Nodes loaded from file: " + filePath);
		}
		else
		{
			// 檔案不存在的錯誤處理
			Debug.LogError("File not found: " + filePath);
		}
	}
	public void load_node(TextAsset mapNode)
	{
		// 將 TextAsset 的文字內容轉換成字串陣列
		string[] allLines = mapNode.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		// 讀取陣列大小資訊
		string sizeLine = allLines[0];
		string[] sizeValues = sizeLine.Split(',');
		int width = int.Parse(sizeValues[0]);
		int height = int.Parse(sizeValues[1]);

		// 建立新的陣列
		ASTarMgr.instance.mapW = width;
		ASTarMgr.instance.mapH = height;
		AStarNode[,] nodes = new AStarNode[width, height];

		int lineIndex = 1; // 從第二行開始讀取地圖資料
						   // 讀取座標、類型和 Buff_Type 資料並填入陣列
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				string line = allLines[lineIndex];
				string[] values = line.Split(',');

				int nodeX = int.Parse(values[0]);
				int nodeY = int.Parse(values[1]);
				int nodeZ = int.Parse(values[2]);
				Node_Type node_Type = (Node_Type)Enum.Parse(typeof(Node_Type), values[3]);
				Buff_Type buff_Type = (Buff_Type)Enum.Parse(typeof(Buff_Type), values[4]);

				// 建立新的 AStarNode 並填入資料
				AStarNode node = new AStarNode(nodeX, nodeY, nodeZ, node_Type, buff_Type);
				nodes[x, y] = node;

				lineIndex++; // 讀取下一行地圖資料
			}
		}

		// 覆蓋 ASTarMgr.instance.nodes
		ASTarMgr.instance.nodes = nodes;

		// 輸出節點資料到 Debug.Log
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				AStarNode node = nodes[x, y];
				//Debug.Log($"Node ({node.x}, {node.y}, {node.z}): Type={node.type}, Buff={node.buff}");
			}
		}

		generator.instance.generate_node(ASTarMgr.instance.nodes);
		//Debug.Log("Nodes loaded from file.");
	}

	public void load_Unit()
	{
		string fileName = "UnitData.txt";

		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 組合完整的檔案路徑
		string filePath = Path.Combine(saveFolderPath, fileName);

		if (File.Exists(filePath))
		{
			List<Unit_Data> unitDataList = new List<Unit_Data>();

			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] values = line.Split(',');

					string unitName = values[0];
					Job_Type unitJob = (Job_Type)Enum.Parse(typeof(Job_Type), values[2]);
					int unitHp = int.Parse(values[3]);
					int unitPAttack = int.Parse(values[4]);
					int unitMAttack = int.Parse(values[5]);
					int unitPDef = int.Parse(values[6]);
					int unitMDef = int.Parse(values[7]);
					int unitAgi = int.Parse(values[8]);
					int unitMov = int.Parse(values[9]);
					int unitLuk = int.Parse(values[10]);
					int unitLightning = int.Parse(values[11]);
					int unitIce = int.Parse(values[12]);
					int unitFire = int.Parse(values[13]);
					int unitWind = int.Parse(values[14]);

					Unit_Data unitData = new Unit_Data(unitName, 1, unitJob, unitHp, unitPAttack,
						unitMAttack, unitPDef, unitMDef, unitAgi, unitMov, unitLuk, unitLightning, unitIce, unitFire, unitWind);

					unitDataList.Add(unitData);

					//Debug.Log($"Unit Data: {unitData.Unit_name}, {unitData.Unit_Level}, {unitData.Unit_job}, {unitData.Unit_Hp}, {unitData.Unit_PAttack}, {unitData.Unit_MAttack}, {unitData.Unit_PDef}, {unitData.Unit_MDef}, {unitData.Unit_Agi}, {unitData.Unit_Mov}, {unitData.Unit_Luk}");
				}
			}

			AI_Unit_Manage.instance.Data = unitDataList;

			//Debug.Log("Unit data loaded from file: " + filePath);
		}
		else
		{
			// 檔案不存在的錯誤處理
			Debug.LogError("File not found: " + filePath);
		}
	}
	public void save_Unit()
	{
		List<Unit_Data> unitDataList = AI_Unit_Manage.instance.Data;

		string fileName = "UnitData.txt";

		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 組合完整的檔案路徑
		string filePath = Path.Combine(saveFolderPath, fileName);

		Directory.CreateDirectory(saveFolderPath);

		using (StreamWriter writer = new StreamWriter(filePath))
		{
			foreach (Unit_Data unitData in unitDataList)
			{
				string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
					unitData.name, 1, unitData.job, unitData.Hp,
					unitData.PAttack, unitData.MAttack, unitData.PDef, unitData.MDef,
					unitData.Agi, unitData.Mov, unitData.Luk, unitData.Lightning,
					unitData.Ice, unitData.Fire, unitData.Wind);

				writer.WriteLine(line);
			}
		}

		Debug.Log("Unit data saved to file: " + filePath);
	}
	public void Create_unit()
	{
		GameObject CU = GameObject.Find("CreateUI");



		string name = GetChild(CU, 1, 1).GetComponent<TMP_InputField>().text;
		TMP_Dropdown dropdown = GetChild(CU, 2, 1).GetComponent<TMP_Dropdown>();
		string jobString = dropdown.options[dropdown.value].text;
		Job_Type job = Job_Type.Null;
		if (Enum.IsDefined(typeof(Job_Type), jobString))
		{
			job = (Job_Type)Enum.Parse(typeof(Job_Type), jobString);
		}
		int hp = Int32.Parse(GetChild(CU, 3, 2).GetComponent<TMP_InputField>().text);
		int PAttack = Int32.Parse(GetChild(CU, 4, 2).GetComponent<TMP_InputField>().text);
		int MAttack = Int32.Parse(GetChild(CU, 5, 2).GetComponent<TMP_InputField>().text);
		int PDef = Int32.Parse(GetChild(CU, 6, 2).GetComponent<TMP_InputField>().text);
		int MDef = Int32.Parse(GetChild(CU, 7, 2).GetComponent<TMP_InputField>().text);
		int Agi = Int32.Parse(GetChild(CU, 8, 2).GetComponent<TMP_InputField>().text);
		int Mov = Int32.Parse(GetChild(CU, 9, 2).GetComponent<TMP_InputField>().text);
		int Luk = Int32.Parse(GetChild(CU, 10, 2).GetComponent<TMP_InputField>().text);
		int Lightning = Int32.Parse(GetChild(CU, 11, 2).GetComponent<TMP_InputField>().text);
		int Ice = Int32.Parse(GetChild(CU, 12, 2).GetComponent<TMP_InputField>().text);
		int Fire = Int32.Parse(GetChild(CU, 13, 2).GetComponent<TMP_InputField>().text);
		int Wind = Int32.Parse(GetChild(CU, 14, 2).GetComponent<TMP_InputField>().text);


		if (AI_Unit_Manage.instance.Data.Exists(data => data.name == name))
		{
			Debug.Log("創建失敗:已包含相同的的單位資料");
			return;
		}
		AI_Unit_Manage.instance.Data.Add(new Unit_Data(name, 1, job, hp, PAttack, MAttack, PDef, MDef, Agi, Mov, Luk, Lightning, Ice, Fire, Wind));
		save_Unit();

	}
	public void Remove_unit()
	{
		Vector3 cursorLoc = cursor_move.instance.GetCursorLoc();
		cursor_move cm = cursor_move.instance;
		GameObject mgr = Map_Unit_Mgr.instance.GetMgr();
		for (int i = 0; i < mgr.transform.childCount; i++)
		{

			GameObject dleobj = cm.findUnitOnCursor(mgr.transform.GetChild(i).gameObject);

			if (dleobj != null)
			{
				Debug.Log(dleobj);
				Destroy(dleobj);
				break;
			}

		}
	}
	public void save_map_unit()
	{
		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 確保資料夾存在
		Directory.CreateDirectory(saveFolderPath);

		// 建立單一的資料列表，包含我方、敵軍和友軍的資料
		List<Unit_map_Date> allUnits = new List<Unit_map_Date>();
		allUnits.AddRange(Map_Unit_Mgr.instance.Player_Unit_Loc);
		allUnits.AddRange(Map_Unit_Mgr.instance.Enemy_Unit);
		allUnits.AddRange(Map_Unit_Mgr.instance.Ally_Unit);

		// 將資料轉換成字串陣列
		string[] unitData = ConvertUnitDataToStringArray(allUnits);

		// 將資料寫入檔案
		File.WriteAllLines(Path.Combine(saveFolderPath, "Map_Unit_Data.txt"), unitData);

		Debug.Log("Map unit data saved.");
	}
	private string[] ConvertUnitDataToStringArray(List<Unit_map_Date> units)
	{
		List<string> unitDataList = new List<string>();

		foreach (Unit_map_Date unitMapData in units)
		{
			// 將單位資料轉換為字串，並用逗號分隔每個數值
			string unitData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
				GetCampLabel(unitMapData), // 加入陣營標籤
				unitMapData.loc.x, unitMapData.loc.y, unitMapData.loc.z,
				unitMapData.date.name, unitMapData.date.Level, unitMapData.date.job,
				unitMapData.date.Hp, unitMapData.date.PAttack, unitMapData.date.MAttack,
				unitMapData.date.PDef, unitMapData.date.MDef, unitMapData.date.Agi,
				unitMapData.date.Mov, unitMapData.date.Luk,
				unitMapData.date.Lightning, unitMapData.date.Ice, unitMapData.date.Fire, unitMapData.date.Wind);

			unitDataList.Add(unitData);
		}

		return unitDataList.ToArray();
	}
	private string GetCampLabel(Unit_map_Date unitMapData)
	{
		if (Map_Unit_Mgr.instance.Player_Unit_Loc.Contains(unitMapData))
		{
			return "Player";
		}
		else if (Map_Unit_Mgr.instance.Enemy_Unit.Contains(unitMapData))
		{
			return "Enemy";
		}
		else if (Map_Unit_Mgr.instance.Ally_Unit.Contains(unitMapData))
		{
			return "Ally";
		}

		return "Unknown";
	}
	public void load_map_unit()
	{
		// 取得應用程式的持久資料路徑的父資料夾路徑
		string parentFolder = Path.GetDirectoryName(Application.persistentDataPath);
		string saveFolderPath = Path.Combine(parentFolder, "path/to/save");

		// 確保檔案存在
		string filePath = Path.Combine(saveFolderPath, "Map_Unit_Data.txt");
		if (File.Exists(filePath))
		{
			// 讀取所有行的資料
			string[] allLines = File.ReadAllLines(filePath);

			// 清空目前的單位列表
			Map_Unit_Mgr.instance.Player_Unit_Loc.Clear();
			Map_Unit_Mgr.instance.Enemy_Unit.Clear();
			Map_Unit_Mgr.instance.Ally_Unit.Clear();

			// 將資料重新分配給對應的陣營
			foreach (string line in allLines)
			{
				// 用逗號分隔每筆資料
				string[] dataValues = line.Split(',');

				// 第一個資料是陣營標籤
				string camp = dataValues[0];

				// 從第二個資料開始是單位的資料
				Vector3 loc = new Vector3(int.Parse(dataValues[1]), int.Parse(dataValues[2]), int.Parse(dataValues[3]));
				string name = dataValues[4];
				int level = int.Parse(dataValues[5]);
				Job_Type job = (Job_Type)Enum.Parse(typeof(Job_Type), dataValues[6]);
				int hp = int.Parse(dataValues[7]);
				int pAttack = int.Parse(dataValues[8]);
				int mAttack = int.Parse(dataValues[9]);
				int pDef = int.Parse(dataValues[10]);
				int mDef = int.Parse(dataValues[11]);
				int agi = int.Parse(dataValues[12]);
				int mov = int.Parse(dataValues[13]);
				int luk = int.Parse(dataValues[14]);
				int lightning = int.Parse(dataValues[15]);
				int ice = int.Parse(dataValues[16]);
				int fire = int.Parse(dataValues[17]);
				int wind = int.Parse(dataValues[18]);

				// 根據陣營標籤將資料加入對應的陣營列表
				Unit_Loc unitLoc = new Unit_Loc(loc);
				Unit_Data unitData = new Unit_Data(name, level, job, hp, pAttack, mAttack, pDef, mDef, agi, mov, luk, lightning, ice, fire, wind);
				Unit_map_Date unitMapData = new Unit_map_Date(unitLoc, unitData);

				if (camp == "Player")
				{
					Map_Unit_Mgr.instance.Player_Unit_Loc.Add(unitMapData);
				}
				else if (camp == "Enemy")
				{
					Map_Unit_Mgr.instance.Enemy_Unit.Add(unitMapData);
				}
				else if (camp == "Ally")
				{
					Map_Unit_Mgr.instance.Ally_Unit.Add(unitMapData);
				}
			}

			Debug.Log("Map unit data loaded from file: " + filePath);
		}
		else
		{
			// 檔案不存在的錯誤處理
			Debug.LogError("File not found: " + filePath);
		}
		SetTempUnit();
	}
	public void load_map_unit(TextAsset Unitdate)
	{
		// 將 TextAsset 的文字內容轉換成字串陣列
		string[] allLines = Unitdate.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		// 清空目前的單位列表
		Map_Unit_Mgr.instance.Player_Unit_Loc.Clear();
		Map_Unit_Mgr.instance.Enemy_Unit.Clear();
		Map_Unit_Mgr.instance.Ally_Unit.Clear();

		// 將資料重新分配給對應的陣營
		foreach (string line in allLines)
		{
			// 用逗號分隔每筆資料
			string[] dataValues = line.Split(',');

			// 第一個資料是陣營標籤
			string camp = dataValues[0];

			// 從第二個資料開始是單位的資料
			Vector3 loc = new Vector3(int.Parse(dataValues[1]), int.Parse(dataValues[2]), int.Parse(dataValues[3]));
			string name = dataValues[4];
			int level = int.Parse(dataValues[5]);
			Job_Type job = (Job_Type)Enum.Parse(typeof(Job_Type), dataValues[6]);
			int hp = int.Parse(dataValues[7]);
			int pAttack = int.Parse(dataValues[8]);
			int mAttack = int.Parse(dataValues[9]);
			int pDef = int.Parse(dataValues[10]);
			int mDef = int.Parse(dataValues[11]);
			int agi = int.Parse(dataValues[12]);
			int mov = int.Parse(dataValues[13]);
			int luk = int.Parse(dataValues[14]);
			int lightning = int.Parse(dataValues[15]);
			int ice = int.Parse(dataValues[16]);
			int fire = int.Parse(dataValues[17]);
			int wind = int.Parse(dataValues[18]);

			// 根據陣營標籤將資料加入對應的陣營列表
			Unit_Loc unitLoc = new Unit_Loc(loc);
			Unit_Data unitData = new Unit_Data(name, level, job, hp, pAttack, mAttack, pDef, mDef, agi, mov, luk, lightning, ice, fire, wind);
			Unit_map_Date unitMapData = new Unit_map_Date(unitLoc, unitData);

			if (camp == "Player")
			{
				Map_Unit_Mgr.instance.Player_Unit_Loc.Add(unitMapData);
			}
			else if (camp == "Enemy")
			{
				Map_Unit_Mgr.instance.Enemy_Unit.Add(unitMapData);
			}
			else if (camp == "Ally")
			{
				Map_Unit_Mgr.instance.Ally_Unit.Add(unitMapData);
			}
		}

		Debug.Log("Map unit data loaded from file.");
		SetTempUnit(1);
	}
	public void item_list_update()
	{

		GameObject Content = GetChild(GameObject.Find("Scroll_View_item"), 1, 1);
		if (Content.transform.childCount != 0)
		{
			Destroy(Content.transform.GetChild(0).gameObject);
		}
		//獲取資料數量
		AI_Unit_Manage aum = AI_Unit_Manage.instance;
		int DateNum = aum.Data.Count + 1;
		Vector2 ContentSize = new Vector2(0, 500);
		if (DateNum > 9)
		{
			ContentSize = new Vector2(0, 500 + ((DateNum - 9) * 50));
		}
		//Debug.Log("有"+(DateNum-1)+"筆資料");
		RectTransform rt1 = Content.GetComponent<RectTransform>();
		rt1.sizeDelta = ContentSize;
		GameObject center = new GameObject("center");
		RectTransform rectTransform = center.AddComponent<RectTransform>();
		center.transform.parent = Content.transform;
		RectTransform rt2 = center.GetComponent<RectTransform>();
		rt2.anchoredPosition = new Vector2(0, (rt1.sizeDelta.y / 2) - 25);


		//預設位置
		Vector2 originLoc = new Vector2(0, 0);
		//選項間距
		Vector2 interval = new Vector2(0, -50);
		//先創建 選項：創建新單位
		for (int i = 0; i < 1; i++)
		{
			GameObject newButton = Instantiate(Unit_list_temp);
			RectTransform rt = newButton.GetComponent<RectTransform>();
			newButton.transform.SetParent(center.transform, false);
			rt.anchoredPosition = originLoc;
			TextMeshProUGUI textMesh = newButton.GetComponentInChildren<TextMeshProUGUI>();
			textMesh.text = "new Unit";

			Button button = newButton.GetComponent<Button>();
			button.onClick.AddListener(() => Change_Scene.instance.show_Create());
			originLoc += interval;
		}

		foreach (var date in aum.Data)
		{
			string name = date.name;
			GameObject newButton = Instantiate(Unit_list_temp);
			RectTransform rt = newButton.GetComponent<RectTransform>();
			newButton.transform.SetParent(center.transform, false);
			rt.anchoredPosition = originLoc;
			TextMeshProUGUI textMesh = newButton.GetComponentInChildren<TextMeshProUGUI>();
			textMesh.text = date.name;

			Button button = newButton.GetComponent<Button>();
			button.onClick.AddListener(() => Change_Scene.instance.show_SetAIUnit(date.name));
			button.onClick.AddListener(() => SetUpUI.instance.HandleUnitValueChanged(textMesh.text));

			originLoc += interval;
		}
	}
	public void SetUpUnit()
	{
		//先讀取已配置的屬性
		Map_Unit_Mgr mum = Map_Unit_Mgr.instance;
		SetUpUI suu = SetUpUI.instance;
		string name = suu.name.text;
		int hp = Int32.Parse(suu.hp.text);
		Job_Type job = Job_Type.Null;
		switch (suu.job.text)
		{
			case "Saber":
				job = Job_Type.Saber;
				break;
			case "Archer":
				job = Job_Type.Archer;
				break;
			case "Tank":
				job = Job_Type.Tank;
				break;
			case "Witch":
				job = Job_Type.Witch;
				break;
			case "Scoundrel":
				job = Job_Type.Scoundrel;
				break;
			case "Berserker":
				job = Job_Type.Berserker;
				break;
			case "FireMagic":
				job = Job_Type.FireMagic;
				break;
			case "Sagittarius":
				job = Job_Type.Sagittarius;
				break;
			case "SpawnPoint":
				job = Job_Type.SpawnPoint;
				break;
			default:
				job = Job_Type.Null;
				break;
		}
		int PAttack = Int32.Parse(suu.PAttack.text);
		int MAttack = Int32.Parse(suu.MAttack.text);
		int PDef = Int32.Parse(suu.PDef.text);
		int MDef = Int32.Parse(suu.MDef.text);
		int Agi = Int32.Parse(suu.Agi.text);
		int Mov = Int32.Parse(suu.Mov.text);
		int Luk = Int32.Parse(suu.Luk.text);
		int Lightning = Int32.Parse(suu.Lightning.text);
		int Ice = Int32.Parse(suu.Ice.text);
		int Fire = Int32.Parse(suu.Fire.text);
		int Wind = Int32.Parse(suu.Wind.text);
		//int Lightning ;
		//int Ice ;
		//int Fire ;
		//int Wind ;
		int Level = Int32.Parse(suu.Level.text);
		int faction = suu.dd.value + 1;
		//當我按下 放置按鈕
		print(job);
		//會將 單位白模 放置在游標的位置
		Vector3 cursorLoc = cursor_move.instance.gameObject.transform.position;
		GameObject ut = Instantiate(Unit_template, cursorLoc, Unit_template.transform.rotation);
		ut.name = name;
		//若為敵方單位 會成為Enemy_Unit的子物件
		//且在 Map_Unit_Mgr.instance.Enemy_Unit 加入這份單位資料
		//若為友方單位 會成為Ally_Unit的子物件
		//且在 Map_Unit_Mgr.instance.Ally_Unit 加入這份單位資料
		SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
		SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
		ImgMgr im = ImgMgr.instance;
		
		switch (faction)
		{
			
			case 1:
				ut.transform.parent = suu.Enemy_Unit.transform;
				print(mum.Enemy_Unit);
				mum.Enemy_Unit.Add(new Unit_map_Date(new Unit_Loc(cursorLoc), new Unit_Data(name, Level, job, hp, PAttack,
					MAttack, PDef, MDef, Agi, Mov, Luk, Lightning, Ice, Fire, Wind)));
				break;
			case 2:
				ut.transform.parent = suu.Ally_Unit.transform;
				mum.Ally_Unit.Add(new Unit_map_Date(new Unit_Loc(cursorLoc), new Unit_Data(name, Level, job, hp, PAttack,
					MAttack, PDef, MDef, Agi, Mov, Luk, Lightning, Ice, Fire, Wind)));
				break;
			case 3:
				ut.transform.parent = suu.Player_Unit.transform;
				
				mum.Player_Unit_Loc.Add(new Unit_map_Date(new Unit_Loc(cursorLoc), new Unit_Data(name, Level, job, hp, PAttack,
					MAttack, PDef, MDef, Agi, Mov, Luk, Lightning, Ice, Fire, Wind)));
				break;
		}
		Debug.Log(job.ToString());
		switch (faction)
		{
			case 1:
				jobbg.color = Color.red;
				break;
			case 2:
				jobbg.color = Color.green;
				break;
			case 3:
				jobbg.color = Color.white;
				break;
			default:
				break;
		}
		//改變圖片
		switch (job)
		{
			case Job_Type.Null:
				break;
			case Job_Type.Saber:
				jobimg.sprite = im.job_img.Sword;
				break;
			case Job_Type.Archer:
				jobimg.sprite = im.job_img.Bow;
				break;
			case Job_Type.Tank:
				jobimg.sprite = im.job_img.Shield;
				break;
			case Job_Type.Witch:
				jobimg.sprite = im.job_img.Magic;
				break;
			case Job_Type.Scoundrel:
				jobimg.sprite = im.job_img.Sword;
				break;
			case Job_Type.Berserker:
				jobimg.sprite = im.job_img.Shield;
				break;
			case Job_Type.FireMagic:
				jobimg.sprite = im.job_img.Magic;
				break;
			case Job_Type.Sagittarius:
				jobimg.sprite = im.job_img.Bow;
				break;
			case Job_Type.SpawnPoint:
				jobimg.sprite = im.job_img.Bow;
				Color color = jobimg.color;
				color.a = 0f;
				jobimg.color = color;
				break;
			default:
				break;
		}



	}
	public void SetTempUnit()
	{
		Map_Unit_Mgr mum = Map_Unit_Mgr.instance;
		SetUpUI suu = SetUpUI.instance;
		List<Unit_map_Date> eu = mum.Enemy_Unit;
		List<Unit_map_Date> pu = mum.Player_Unit_Loc;
		List<Unit_map_Date> au = mum.Ally_Unit;
		foreach (var list in eu)
		{
			//座標
			int x = list.loc.x;
			int y = list.loc.y;
			int z = list.loc.z;
			//需要的資料
			string name = list.date.name;
			Job_Type job = list.date.job;
			//在座標上生成單位 模板
			Vector3 unitloc = new Vector3(x, y, z);
			GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
			ut.name = name;
			SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			ImgMgr im = ImgMgr.instance;
			ut.transform.parent = suu.Enemy_Unit.transform;
			jobbg.color = Color.red;
			switch (job)
			{
				case Job_Type.Null:
					break;
				case Job_Type.Saber:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Archer:
					jobimg.sprite = im.job_img.Bow;
					break;
				case Job_Type.Tank:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.Witch:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Scoundrel:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Berserker:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.FireMagic:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Sagittarius:
					jobimg.sprite = im.job_img.Bow;
					break;
				
				default:
					break;
			}
		}
		foreach (var list in au)
		{
			//座標
			int x = list.loc.x;
			int y = list.loc.y;
			int z = list.loc.z;
			//需要的資料
			string name = list.date.name;
			Job_Type job = list.date.job;
			//在座標上生成單位 模板
			Vector3 unitloc = new Vector3(x, y, z);
			GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
			ut.name = name;
			SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			ImgMgr im = ImgMgr.instance;
			ut.transform.parent = suu.Ally_Unit.transform;
			jobbg.color = Color.green;
			switch (job)
			{
				case Job_Type.Null:
					break;
				case Job_Type.Saber:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Archer:
					jobimg.sprite = im.job_img.Bow;
					break;
				case Job_Type.Tank:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.Witch:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Scoundrel:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Berserker:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.FireMagic:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Sagittarius:
					jobimg.sprite = im.job_img.Bow;
					break;
				default:
					break;
			}
		}
		foreach (var list in pu)
		{
			//座標
			int x = list.loc.x;
			int y = list.loc.y;
			int z = list.loc.z;
			//需要的資料
			string name = list.date.name;
			Job_Type job = list.date.job;
			//在座標上生成單位 模板
			Vector3 unitloc = new Vector3(x, y, z);
			GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
			ut.name = name;
			SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			ImgMgr im = ImgMgr.instance;
			ut.transform.parent = suu.Enemy_Unit.transform;
			jobbg.color = Color.white;
			switch (job)
			{
				case Job_Type.SpawnPoint:
					jobimg.sprite = im.job_img.Bow;
					Color color = jobimg.color;
					color.a = 0f;
					jobimg.color = color;
					break;
				default:
					break;
			}
		}

	}
	public void SetTempUnit(int level)
	{
		Map_Unit_Mgr mum = Map_Unit_Mgr.instance;
		SetUpUI suu = SetUpUI.instance;
		List<Unit_map_Date> eu = mum.Enemy_Unit;
		List<Unit_map_Date> pu = mum.Player_Unit_Loc;
		List<Unit_map_Date> au = mum.Ally_Unit;
		foreach (var list in eu)
		{
			//座標
			int x = list.loc.x;
			int y = list.loc.y;
			int z = list.loc.z;
			//需要的資料
			string name = list.date.name;
			Job_Type job = list.date.job;
			//在座標上生成單位 模板
			Vector3 unitloc = new Vector3(x, y, z);
			GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
			ut.name = name;
			SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			ImgMgr im = ImgMgr.instance;
			ut.transform.parent = suu.Enemy_Unit.transform;
			jobbg.color = Color.red;
			switch (job)
			{
				case Job_Type.Null:
					break;
				case Job_Type.Saber:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Archer:
					jobimg.sprite = im.job_img.Bow;
					break;
				case Job_Type.Tank:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.Witch:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Scoundrel:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Berserker:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.FireMagic:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Sagittarius:
					jobimg.sprite = im.job_img.Bow;
					break;

				default:
					break;
			}
		}
		foreach (var list in au)
		{
			//座標
			int x = list.loc.x;
			int y = list.loc.y;
			int z = list.loc.z;
			//需要的資料
			string name = list.date.name;
			Job_Type job = list.date.job;
			//在座標上生成單位 模板
			Vector3 unitloc = new Vector3(x, y, z);
			GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
			ut.name = name;
			SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
			ImgMgr im = ImgMgr.instance;
			ut.transform.parent = suu.Ally_Unit.transform;
			jobbg.color = Color.green;
			switch (job)
			{
				case Job_Type.Null:
					break;
				case Job_Type.Saber:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Archer:
					jobimg.sprite = im.job_img.Bow;
					break;
				case Job_Type.Tank:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.Witch:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Scoundrel:
					jobimg.sprite = im.job_img.Sword;
					break;
				case Job_Type.Berserker:
					jobimg.sprite = im.job_img.Shield;
					break;
				case Job_Type.FireMagic:
					jobimg.sprite = im.job_img.Magic;
					break;
				case Job_Type.Sagittarius:
					jobimg.sprite = im.job_img.Bow;
					break;
				default:
					break;
			}
		}

		if (level ==1)
		{
			int num = 0;
			foreach (var list in pu)
			{
				

				Unit_Data ud= Map_Unit_Mgr.instance.Player_Unit[num];
				num++;
				//座標
				int x = list.loc.x;
				int y = list.loc.y;
				int z = list.loc.z;
				//需要的資料
				string name = ud.name;
				Job_Type job = ud.job;
				//在座標上生成單位 模板
				Vector3 unitloc = new Vector3(x, y, z);
				GameObject ut = Instantiate(Unit_template, unitloc, Unit_template.transform.rotation);
				ut.name = name;
				SpriteRenderer jobbg = ut.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
				SpriteRenderer jobimg = ut.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
				ImgMgr im = ImgMgr.instance;
				ut.transform.parent = suu.Player_Unit.transform;
				jobbg.color = Color.white;
				switch (job)
				{
					case Job_Type.Null:
						break;
					case Job_Type.Saber:
						jobimg.sprite = im.job_img.Sword;
						break;
					case Job_Type.Archer:
						jobimg.sprite = im.job_img.Bow;
						break;
					case Job_Type.Tank:
						jobimg.sprite = im.job_img.Shield;
						break;
					case Job_Type.Witch:
						jobimg.sprite = im.job_img.Magic;
						break;
					case Job_Type.Scoundrel:
						jobimg.sprite = im.job_img.Sword;
						break;
					case Job_Type.Berserker:
						jobimg.sprite = im.job_img.Shield;
						break;
					case Job_Type.FireMagic:
						jobimg.sprite = im.job_img.Magic;
						break;
					case Job_Type.Sagittarius:
						jobimg.sprite = im.job_img.Bow;
						break;
					default:
						break;
				}
			}
		}
		

	}
	GameObject GetChild(GameObject father, int floor_1)
	{
		return father.transform.GetChild(floor_1 - 1).gameObject;
	}
	GameObject GetChild(GameObject father, int floor_1, int floor_2)
	{
		return father.transform.GetChild(floor_1 - 1).transform.GetChild(floor_2 - 1).gameObject;
	}

	
}
