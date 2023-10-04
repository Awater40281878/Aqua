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
	public int mapW;//�a�ϼe

	public int mapH;//�a�ϰ� 
	private List<AStarNode> openList = new List<AStarNode>();//�}�ҦC��

	private List<AStarNode> cloesList = new List<AStarNode>();//�����C��

	public AStarNode[,] nodes;//�a�ϩҦ���l��������H�e��
	/// <summary>
	/// �Ыتťզa�� w �e�� h ����
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
		//1.�O�_�b�d��
		if (startPos.x < 0 || startPos.x > mapW || startPos.y < 0 || startPos.y > mapH ||
			endPos.x < 0 || endPos.x > mapW || endPos.y < 0 || endPos.y > mapH)
		{
			Debug.Log(mapW + "_" + mapH);
			Debug.Log("�}�l�Ϊ̵����`�I�b�a�ϥ~");
			return null;
		}
		//Debug.Log("mapW"+mapW);
		//Debug.Log("mapH" + mapH);
		//2.�O�_�O����
		AStarNode start = _nodes[(int)startPos.x, (int)startPos.y];
		AStarNode end = _nodes[(int)endPos.x, (int)endPos.y];
		if (start.type == Node_Type.Stop || end.type == Node_Type.Stop)
		{
			//Debug.Log(mapW + "_" + mapH);
			//Debug.Log(end.type);
			Debug.Log("�}�l�Ϊ̵����`�I�������O����");
			Debug.Log(new Vector3(start.x * 10, (start.z - 1) * 4, start.y * 10) + "_" + new Vector3(end.x * 10, (end.z - 1) * 4, end.y * 10));

			return null;
		}
		//3.�M�������M�}�ҦC��
		cloesList.Clear();
		openList.Clear();
		//4.�M�Ÿ`�I�W�@�����ƭ�
		start.father = null;
		start.f = 0;
		start.h = 0;
		start.g = 0;
		cloesList.Add(start);

		//��K�Ӥ�쪺�`�I��J�}�ҦC��
		while (true)
		{


			//���W	-1	-1
			//FindNearlyToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
			//�W	0	-1
			FindNearlyToOpenList(start.x, start.y - 1, 1, start, end, _nodes);
			//�k�W	+1	-1
			//FindNearlyToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
			//��	-1	0	
			FindNearlyToOpenList(start.x - 1, start.y, 1, start, end, _nodes);
			//�k	+1	0
			FindNearlyToOpenList(start.x + 1, start.y, 1, start, end, _nodes);
			//���U	-1	+1
			//FindNearlyToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
			//�U	0	+1
			FindNearlyToOpenList(start.x, start.y + 1, 1, start, end, _nodes);
			//�k�U	+1	+1
			//FindNearlyToOpenList(start.x + 1, start.y + 1 - 1, 1.4f, start, end);



			//�p�G�}�_�C���ų��٨S�����|�A�N�O����
			if (openList.Count == 0)
			{
				Debug.Log("����");
				return null;
			}
			//�ƧǶ}�ҦC���̤p������0��
			openList.Sort(SortOpenList);
			//��J�����C��A�M��q�}�ҦC������
			cloesList.Add(openList[0]);
			//�䪺�o���I�S�ܦ��s���_�I�A�i��U�@���M���p��
			start = openList[0];
			openList.RemoveAt(0);

			if (start == end)
			{
				//�䧹�F
				List<AStarNode> path = new List<AStarNode>();
				path.Add(end);
				while (end.father != null)
				{
					path.Add(end.father);
					end = end.father;
				}
				//����
				path.Reverse();
				return path;
			}
		}
	}

	public List<AStarNode> FindPath_AI_01(Vector2 startPos, Vector2 endPos, int Mov,Vector2 originalEnd)
	{
		//���Z�����t��k�A
		//���I���o�覡 �|���b�ؼЩP�� ���Ҧ��i�઺���I
		//�ҥH�n�ǥѥؼЪ���m�A�PAI�����ӨM�w�M��AI���覡
		//�Ĥ@���� �� �Q�r ����


		AStarNode[,] originalNodes = nodes; // ��l�� nodes �G���}�C
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
			//print("��" + testnum + "��");
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
							//print("�����");
							//print(Mov + "==" + pathnum);
							//print(Path.Count);
							if (pathnum == Mov)
							{
								restart = true;
								//print("���ʪ̬�" + startPos + "�Ӧ�m�����" + paloc + "�Q�]�m�A�����|������" + pathnum + "��");
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
				print("���`��");
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
		//1.�O�_�b�d��
		if (startPos.x < 0 || startPos.x > mapW || startPos.y < 0 || startPos.y > mapH)
		{
			Debug.Log(mapW + "_" + mapH);
			Debug.Log("�}�l�Ϊ̵����`�I�b�a�ϥ~");
			return null;
		}
		//Debug.Log("mapW"+mapW);
		//Debug.Log("mapH" + mapH);
		//2.�O�_�O����
		AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
		if (start.type == Node_Type.Stop)
		{
			//Debug.Log(mapW + "_" + mapH);
			//Debug.Log(end.type);
			Debug.Log("�}�l�Ϊ̵����`�I�������O����");
			return null;
		}
		//3.�M�������M�}�ҦC��
		cloesList.Clear();
		openList.Clear();
		//4.�M�Ÿ`�I�W�@�����ƭ�
		start.father = null;
		start.f = 0;
		start.h = 0;
		start.g = 0;
		cloesList.Add(start);
		int mov = Mov;
		//��K�Ӥ�쪺�`�I��J�}�ҦC��
		while (true)
		{
			mov--;
			foreach (var item in cloesList)
			{
				//�W	0	-1
				FindNearlyToOpenList(item.x, item.y - 1, item, _nodes);
				//��	-1	0	
				FindNearlyToOpenList(item.x - 1, item.y, item, _nodes);
				//�k	+1	0
				FindNearlyToOpenList(item.x + 1, item.y, item, _nodes);
				//�U	0	+1
				FindNearlyToOpenList(item.x, item.y + 1, item, _nodes);
			}

			//�p�G�Ͱ_�C���ų��٨S�����|�A�N�O����
			//if (openList.Count == 0)
			//{
			//	Debug.Log("����");
			//	return null;
			//}
			//�ƧǶ}�ҦC���̤p������0��
			//openList.Sort(SortOpenList);
			//��J�����C��A�M��q�}�ҦC������
			foreach (var item in openList)
			{
				cloesList.Add(item);
			}
			//�䪺�o���I�S�ܦ��s���_�I�A�i��U�@���M���p��
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

	//�ƧǨ��
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

		//�P�_�O�_�O ��� ���� �b�}�� ���� �C�� �����O�~��J�}�ҦC��
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
		//g ���� �����_�I���Z��   =   ����H���_�I���Z�� + ��������H���Z��
		node.g = father.g + g;

		node.h = Mathf.Abs(end.x - node.x) + (Mathf.Abs(end.y - node.y));

		node.f = node.g + node.h;
		openList.Add(node);

	}
	void FindNearlyToOpenList(int x, int y, AStarNode father, AStarNode[,] _nodes)
	{
		//�P�_�O�_�O ��� ���� �b�}�� ���� �C�� �����O�~��J�}�ҦC��
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
		//g ���� �����_�I���Z��   =   ����H���_�I���Z�� + ��������H���Z��
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
					print("���`��");
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
		//�P�_�O�_�O ��� ���� �b�}�� ���� �C�� �����O�~��J�}�ҦC��
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
		//g ���� �����_�I���Z��   =   ����H���_�I���Z�� + ��������H���Z��
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