using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using static UnityEditor.Progress;

public class ASTarMgr : MonoBehaviour
{
	public static ASTarMgr instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public int mapW;//地圖寬

	public int mapH;//地圖高 
	private List<AStarNode> openList = new List<AStarNode>();//開啟列表

	private List<AStarNode> cloesList = new List<AStarNode>();//關閉列表

	public AStarNode[,] nodes;//地圖所有格子的相關對象容器
	/// <summary>
	/// 創建空白地圖 w 寬度 h 高度
	/// </summary>
	public void InitMapInfo(int w, int h)
	{
		this.mapW = w;
		this.mapH = h;
		nodes = new AStarNode[w, h];
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				AStarNode node = new AStarNode(i, j, 1, Node_Type.walk, Buff_Type.Null);
				nodes[i, j] = node;
			}
		}
		//Debug.Log(nodes.GetLength(0) + "_" + nodes.GetLength(1));

	}

	public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
	{
		return FindPath(startPos, endPos, nodes);
	}
	public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos, AStarNode[,] _nodes)
	{
		//1.是否在範圍內
		if (startPos.x < 0 || startPos.x > mapW || startPos.y < 0 || startPos.y > mapH ||
			endPos.x < 0 || endPos.x > mapW || endPos.y < 0 || endPos.y > mapH)
		{
			Debug.Log(mapW + "_" + mapH);
			Debug.Log("開始或者結束節點在地圖外");
			return null;
		}
		//Debug.Log("mapW"+mapW);
		//Debug.Log("mapH" + mapH);
		//2.是否是阻擋
		AStarNode start = _nodes[(int)startPos.x, (int)startPos.y];
		AStarNode end = _nodes[(int)endPos.x, (int)endPos.y];
		if (start.type == Node_Type.Stop || end.type == Node_Type.Stop)
		{
			//Debug.Log(mapW + "_" + mapH);
			//Debug.Log(end.type);
			Debug.Log("開始或者結束節點的類型是阻擋");
			Debug.Log(new Vector3(start.x * 10, (start.z - 1) * 4, start.y * 10) + "_" + new Vector3(end.x * 10, (end.z - 1) * 4, end.y * 10));

			return null;
		}
		//3.清空關閉和開啟列表
		cloesList.Clear();
		openList.Clear();
		//4.清空節點上一次的數值
		start.father = null;
		start.f = 0;
		start.h = 0;
		start.g = 0;
		cloesList.Add(start);

		//把八個方位的節點放入開啟列表
		while (true)
		{


			//左上	-1	-1
			//FindNearlyToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
			//上	0	-1
			FindNearlyToOpenList(start.x, start.y - 1, 1, start, end, _nodes);
			//右上	+1	-1
			//FindNearlyToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
			//左	-1	0	
			FindNearlyToOpenList(start.x - 1, start.y, 1, start, end, _nodes);
			//右	+1	0
			FindNearlyToOpenList(start.x + 1, start.y, 1, start, end, _nodes);
			//左下	-1	+1
			//FindNearlyToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
			//下	0	+1
			FindNearlyToOpenList(start.x, start.y + 1, 1, start, end, _nodes);
			//右下	+1	+1
			//FindNearlyToOpenList(start.x + 1, start.y + 1 - 1, 1.4f, start, end);



			//如果開起列表為空都還沒找到路徑，就是死路
			if (openList.Count == 0)
			{
				Debug.Log("死路");
				return null;
			}
			//排序開啟列表把最小的放到第0個
			openList.Sort(SortOpenList);
			//放入關閉列表，然後從開啟列表中移除
			cloesList.Add(openList[0]);
			//找的這個點又變成新的起點，進行下一次尋路計算
			start = openList[0];
			openList.RemoveAt(0);

			if (start == end)
			{
				//找完了
				List<AStarNode> path = new List<AStarNode>();
				path.Add(end);
				while (end.father != null)
				{
					path.Add(end.father);
					end = end.father;
				}
				//反轉
				path.Reverse();
				return path;
			}
		}
	}

	public List<AStarNode> FindPath_AI_01(Vector2 startPos, Vector2 endPos, int Mov,Vector2 originalEnd)
	{
		//遠距離單位演算法，
		//終點取得方式 會先在目標周圍 找到所有可能的終點
		//所以要藉由目標的位置，與AI類型來決定尋找AI的方式
		//第一類型 為 十字 類型


		AStarNode[,] originalNodes = nodes; // 原始的 nodes 二維陣列
		AStarNode[,] TempNodes = DeepCloneAStarNodeArray(originalNodes);
		List<AStarNode> Path = new List<AStarNode>();
		List<AStarNode> endlist = new List<AStarNode>();
		List<AStarNode> FailPath = new List<AStarNode>();
		////
		int unitnum = -1;
		int testnum = 0;
		bool restart = false;
		foreach (var item in Map_Unit_Mgr.instance.Player_Unit)
		{
			TempNodes[(int)(item.loc.x / 10), (int)(item.loc.z / 10)].type = Node_Type.Stop;
		}
		generator.instance.generate_node_game(TempNodes);
		while (true)
		{

			restart = false;
			testnum++;
			//print("第" + testnum + "圈");
			Map_Unit_Mgr mun = Map_Unit_Mgr.instance;
			unitnum = 0;
			//print(TempNodes.GetLength(0) + "_" + TempNodes.GetLength(1));

			Path = FindPath(startPos, endPos, TempNodes);
			if (Path.Count != 0)
			{
				int pathnum = 1;

				foreach (var pa in Path)
				{

					Vector3 paloc = new Vector3(pa.x * 10, (pa.z - 1) * 4, pa.y * 10);


					foreach (var unit in mun.Enemy_Unit)
					{
						if (unit.loc == paloc)
						{
							unitnum++;
							//print("找到單位");
							//print(Mov + "==" + pathnum);
							//print(Path.Count);
							if (pathnum == Mov)
							{
								restart = true;
								//print("移動者為" + startPos + "該位置的單位" + paloc + "被設置，為路徑中的第" + pathnum + "項");
								TempNodes[pa.x, pa.y].type = Node_Type.Stop;
								//print(TempNodes[5, 5].type);
								//print(nodes[5, 5].type);
							}
						}
					}
					pathnum++;
				}
			}
			if (restart == false)
			{
				break;
			}
			if (testnum == 1000)
			{
				print("死循環");
				break;
			}
		}
		//print(123);
		generator.instance.generate_node_game(TempNodes);

		int num = Path.Count - 1;
		print(new Vector2(Path[0].x, Path[0].y) + "___"+ originalEnd+"___"+ Vector2.Distance(new Vector2(Path[num].x, Path[num].y), originalEnd));
		if (Vector2.Distance(new Vector2(Path[0].x, Path[0].y),originalEnd)==1)
		{
			while (true)
			{
				if (Path.Count!=1)
				{
					Path.RemoveAt(num);
					num--;
				}
				else
				{
					break;
				}
				

			}
		}

		return Path;

	}
	public List<AStarNode> FindMoveRange(Vector2 startPos, AStarNode[,] _nodes, int Mov)
	{
		//1.是否在範圍內
		if (startPos.x < 0 || startPos.x > mapW || startPos.y < 0 || startPos.y > mapH)
		{
			Debug.Log(mapW + "_" + mapH);
			Debug.Log("開始或者結束節點在地圖外");
			return null;
		}
		//Debug.Log("mapW"+mapW);
		//Debug.Log("mapH" + mapH);
		//2.是否是阻擋
		AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
		if (start.type == Node_Type.Stop)
		{
			//Debug.Log(mapW + "_" + mapH);
			//Debug.Log(end.type);
			Debug.Log("開始或者結束節點的類型是阻擋");
			return null;
		}
		//3.清空關閉和開啟列表
		cloesList.Clear();
		openList.Clear();
		//4.清空節點上一次的數值
		start.father = null;
		start.f = 0;
		start.h = 0;
		start.g = 0;
		cloesList.Add(start);
		int mov = Mov;
		//把八個方位的節點放入開啟列表
		while (true)
		{
			mov--;
			foreach (var item in cloesList)
			{
				//上	0	-1
				FindNearlyToOpenList(item.x, item.y - 1, item, _nodes);
				//左	-1	0	
				FindNearlyToOpenList(item.x - 1, item.y, item, _nodes);
				//右	+1	0
				FindNearlyToOpenList(item.x + 1, item.y, item, _nodes);
				//下	0	+1
				FindNearlyToOpenList(item.x, item.y + 1, item, _nodes);
			}

			//如果凱起列表為空都還沒找到路徑，就是死路
			//if (openList.Count == 0)
			//{
			//	Debug.Log("死路");
			//	return null;
			//}
			//排序開啟列表把最小的放到第0個
			//openList.Sort(SortOpenList);
			//放入關閉列表，然後從開啟列表中移除
			foreach (var item in openList)
			{
				cloesList.Add(item);
			}
			//找的這個點又變成新的起點，進行下一次尋路計算
			//start = openList[0];
			List<AStarNode> newOpenList = new List<AStarNode>(cloesList);

			//openList.RemoveAt(0);
			openList.RemoveAll(item => cloesList.Contains(item));
			if (mov == 0)
			{
				List<Unit_map_Date> pu = Map_Unit_Mgr.instance.Player_Unit;
				List<Unit_map_Date> au = Map_Unit_Mgr.instance.Ally_Unit;
				List<Vector2> AllyLoc = new List<Vector2>();
				List<Vector2> RemoveLoc = new List<Vector2>();
				foreach (Unit_map_Date u in pu)
				{
					AllyLoc.Add(new Vector2(u.loc.x / 10, u.loc.z / 10));
				}
				foreach (var u in au)
				{
					AllyLoc.Add(new Vector2(u.loc.x / 10, u.loc.z / 10));
				}
				foreach (Vector2 al in AllyLoc)
				{
					foreach (var op in cloesList)
					{
						//print(op.x +"_" + al.x+"   "+ op.z + "_" + al.y);
						if (op.x == al.x && op.z == al.y)
						{
							RemoveLoc.Add(new Vector2(op.x, op.z));
						}
					}
				}

				if (RemoveLoc.Count != 0)
				{
					cloesList.RemoveAll(node => RemoveLoc.Any(vec => vec.x == node.x && vec.y == node.y));

				}
				return cloesList;
			}

		}
	}

	//排序函數
	private int SortOpenList(AStarNode a, AStarNode b)
	{
		if (a.f > b.f)
			return 1;
		else if (a.f == b.f)
			return 1;
		else
			return -1;
	}
	public void FindNearlyToOpenList(int x, int y, float g, AStarNode father, AStarNode end, AStarNode[,] _nodes)
	{

		//判斷是否是 邊界 阻擋 在開啟 關閉 列表中 都不是才放入開啟列表
		if (x < 0 || x >= mapW || y < 0 || y >= mapH)
		{
			//Debug.Log(x+"_"+y);
			return;

		}
		////print(1);
		AStarNode node = _nodes[x, y];
		if (node == null || node.type == Node_Type.Stop || cloesList.Contains(node) || openList.Contains(node))
		{
			//Debug.Log("3");
			return;
		}
		node.father = father;
		//g 公式 我離起點的距離   =   父對象離起點的距離 + 我離父對象的距離
		node.g = father.g + g;

		node.h = Mathf.Abs(end.x - node.x) + (Mathf.Abs(end.y - node.y));

		node.f = node.g + node.h;
		openList.Add(node);

	}
	void FindNearlyToOpenList(int x, int y, AStarNode father, AStarNode[,] _nodes)
	{
		//判斷是否是 邊界 阻擋 在開啟 關閉 列表中 都不是才放入開啟列表
		if (x < 0 || x >= mapW || y < 0 || y >= mapH)
		{
			//Debug.Log(x+"_"+y);
			return;

		}
		AStarNode node = _nodes[x, y];
		//List<Unit_map_Date> eu = Map_Unit_Mgr.instance.Enemy_Unit;
		//List<Vector2> EnemyLoc = new List<Vector2>();
		//foreach (Unit_map_Date u in eu)
		//{
		//	EnemyLoc.Add(new Vector2(u.loc.x / 10, u.loc.z / 10));
		//}
		//foreach (Vector2 el in EnemyLoc)
		//{
		//	if (x == el.x && y == el.y)
		//	{
		//		return;
		//	}
		//}
		if (node == null || node.type == Node_Type.Stop || cloesList.Contains(node) || openList.Contains(node))
		{

			return;
		}
		if (TargetLocOnUnit(new Vector2(node.x, node.y)) == true)
		{
			return;
		}
		node.father = father;
		//g 公式 我離起點的距離   =   父對象離起點的距離 + 我離父對象的距離
		//node.g = father.g + g;

		//node.h = Mathf.Abs(end.x - node.x) + (Mathf.Abs(end.y - node.y));

		//node.f = node.g + node.h;

		openList.Add(node);

	}
	public List<AStarNode> findend(Vector2 target,int Mov)
	{
		List<Unit_map_Date> unitlist = new List<Unit_map_Date>();
		switch (battleRound.instance.Phase)
		{
			case 1:
				unitlist = Map_Unit_Mgr.instance.Enemy_Unit;
				break;
			case 2:
				unitlist = Map_Unit_Mgr.instance.Player_Unit;
				break;
			case 3:
				unitlist = Map_Unit_Mgr.instance.Ally_Unit;
				break;
			default:
				break;
		}
		List<AStarNode> Path = new List<AStarNode>();
		List<AStarNode> tempPath = new List<AStarNode>();
		foreach (var item in unitlist)
		{
			tempPath = FindPath(target, new Vector2(item.loc.x / 10, item.loc.z / 10));
			if (Path.Count == 0)
			{
				Path = tempPath;
			}
			if (tempPath.Count < Path.Count && tempPath.Count != 0)
			{
				Path = tempPath;
			}
		}
		int num = Path.Count - 1;
		Vector2Int end = new Vector2Int((int)Path[num].x, (int)Path[num].y);
		Path.Clear();
		cloesList.Clear();
		openList.Clear();
		cloesList.Add(nodes[end.x, end.y]);
		List<AStarNode> newopenList = new List<AStarNode>();
		List<AStarNode> newcloesList = new List<AStarNode>();
		int test = 0;
		while (true)
		{
			foreach (var item in cloesList)
			{
				test++;
				if (test==100)
				{
					print("死循環");
					return null;
				}
				FindNearlyToOpenList(item.x, item.y - 1, item,nodes);

				FindNearlyToOpenList(item.x - 1, item.y, item, nodes);

				FindNearlyToOpenList(item.x + 1, item.y, item, nodes);

				FindNearlyToOpenList(item.x, item.y + 1, item, nodes);
			}
			//print("openList.Count=" + openList.Count);
			foreach (var item in openList)
			{
					newopenList.Add(item);
					//print(item.x + "_" + item.y);
			}
			
			
			//print("newopenList.Count=" + newopenList.Count);

			foreach (var item in cloesList)
			{
				//print("newcloesList loc =" + new Vector2(item.x, item.y));
				newcloesList.Add(item);
			}

			
			foreach (var item in newopenList)
			{
				//print(new Vector2(item.x, item.y));
				tempPath = FindPath_AI_01(target, new Vector2(item.x , item.y ),Mov,end);
				
				if (Path.Count == 0)
				{
					Path = tempPath;
				}
				if (tempPath.Count < Path.Count && tempPath.Count != 0)
				{
					Path = tempPath;
				}
			}
			if (Path.Count!=0)
			{
				//print(Path.Count);
				return Path;
			}
			cloesList.Clear();
			foreach (var item in newcloesList)
			{
				cloesList.Add(item);
			}
			foreach (var item in newopenList)
			{
				cloesList.Add(item);
			}
		}
		

	}
	AStarNode FindNearlyToOpenList(int x, int y, AStarNode father)
	{
		//判斷是否是 邊界 阻擋 在開啟 關閉 列表中 都不是才放入開啟列表
		if (x < 0 || x >= mapW || y < 0 || y >= mapH)
		{
			//Debug.Log(x+"_"+y);
			return null;

		}
		AStarNode node = nodes[x, y];
		List<Unit_map_Date> eu = Map_Unit_Mgr.instance.Enemy_Unit;
		List<Vector2> EnemyLoc = new List<Vector2>();
		foreach (Unit_map_Date u in eu)
		{
			EnemyLoc.Add(new Vector2(u.loc.x / 10, u.loc.z / 10));
		}
		foreach (Vector2 el in EnemyLoc)
		{
			if (x == el.x && y == el.y)
			{
				return null;
			}
		}
		if (node == null || node.type == Node_Type.Stop || cloesList.Contains(node) || openList.Contains(node))
		{

			return null;
		}
		node.father = father;
		//g 公式 我離起點的距離   =   父對象離起點的距離 + 我離父對象的距離
		//node.g = father.g + g;

		//node.h = Mathf.Abs(end.x - node.x) + (Mathf.Abs(end.y - node.y));

		//node.f = node.g + node.h;
		return node;
		//openList.Add(node);

	}
	private List<AStarNode> FindNearlyEnd(AStarNode _end, List<AStarNode> _FailPath)
	{
		List<AStarNode> endlist = new List<AStarNode>();
		AStarNode nodeToAdd;
		Vector2Int end = new Vector2Int(_end.x, _end.y);

		nodeToAdd = FindNearlyToOpenList(end.x, end.y - 1, _end);
		if (!_FailPath.Contains(nodeToAdd))
			endlist.Add(nodeToAdd);

		nodeToAdd = FindNearlyToOpenList(end.x - 1, end.y, _end);
		if (!_FailPath.Contains(nodeToAdd))
			endlist.Add(nodeToAdd);

		nodeToAdd = FindNearlyToOpenList(end.x + 1, end.y, _end);
		if (!_FailPath.Contains(nodeToAdd))
			endlist.Add(nodeToAdd);

		nodeToAdd = FindNearlyToOpenList(end.x, end.y + 1, _end);
		if (!_FailPath.Contains(nodeToAdd))
			endlist.Add(nodeToAdd);
		return endlist;



	}
	public static AStarNode[,] DeepCloneAStarNodeArray(AStarNode[,] originalArray)
	{
		int width = originalArray.GetLength(0);
		int height = originalArray.GetLength(1);

		AStarNode[,] clonedArray = new AStarNode[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				AStarNode originalNode = originalArray[x, y];
				if (originalNode != null)
				{
					clonedArray[x, y] = originalNode.DeepClone();
				}
			}
		}

		return clonedArray;
	}
	public bool TargetLocOnUnit(Vector2 node)
	{
		List<Vector2> unitloc = new List<Vector2>();
		foreach (var item in Map_Unit_Mgr.instance.Enemy_Unit)
		{
			unitloc.Add(new Vector2(item.loc.x / 10, item.loc.z / 10));
		}
		foreach (var item in Map_Unit_Mgr.instance.Player_Unit)
		{
			unitloc.Add(new Vector2(item.loc.x / 10, item.loc.z / 10));
		}
		foreach (var item in Map_Unit_Mgr.instance.Ally_Unit)
		{
			unitloc.Add(new Vector2(item.loc.x / 10, item.loc.z / 10));
		}
		foreach (var item in unitloc)
		{
			if (item.x == node.x && item.y == node.y)
			{
				return true;
			}
		}
		return false;
	}
}