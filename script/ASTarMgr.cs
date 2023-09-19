using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
				AStarNode node = new AStarNode(i, j,1, Node_Type.walk,Buff_Type.Null);
				nodes[i, j] = node;
			}
		}
		Debug.Log(nodes.GetLength(0) + "_" + nodes.GetLength(1));

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
		AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
		AStarNode end = nodes[(int)endPos.x, (int)endPos.y];
		if (start.type == Node_Type.Stop || end.type == Node_Type.Stop)
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

	public List<AStarNode> FindPath_AI_01(Vector2 startPos, Vector2 endPos)
	{
		//���Z�����t��k�A
		//���I���o�覡 �|���b�ؼЩP�� ���Ҧ��i�઺���I
		//�ҥH�n�ǥѥؼЪ���m�A�PAI�����ӨM�w�M��AI���覡
		//�Ĥ@���� �� �Q�r ����
		List<AStarNode> Path = new List<AStarNode>();
		List<AStarNode> endlist = new List<AStarNode>();
		List<AStarNode> FailPath = new List<AStarNode>();
		Path = FindPath(startPos, endPos);
		//print(Path.Count);
		if (Path.Count == 0)
		{
			print("�A���P�_");
			//�Q�P�w�����������I
			FailPath.Add(nodes[(int)endPos.x, (int)endPos.y]);
			//�Ҧ����|
			List<List<AStarNode>> AllPath = new List<List<AStarNode>>();
			//�������ɡA�ݭn��AI�a��ؼ�
			while (true)
			{
				//�o��s��endlist
				foreach (var node in FailPath)
				{
					endlist = FindNearlyEnd(node, FailPath);
				}
				foreach (var end in endlist)
				{
					//�����|�æs�� ����Ϊ��C��
					Vector2 endvec = new Vector2(end.x, end.y);
					List<AStarNode> Temppath =  FindPath(startPos, endvec);
					if (Temppath.Count!= 0)
					{
						AllPath.Add(Temppath);
					}
					else
					{
						FailPath.Add(end);
					}
				}
				if (AllPath.Count != 0)
				{
					AllPath.Sort((list1, list2) => list1.Count.CompareTo(list2.Count));
					return AllPath[0];
				}
			}
		}
		return Path;

	}
	public List<AStarNode> FindMoveRange(Vector2 startPos, AStarNode[,] _nodes, int Mov)
	{
		//1.�O�_�b�d��
		if (startPos.x < 0 || startPos.x > mapW || startPos.y < 0 || startPos.y > mapH )
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
					AllyLoc.Add(new Vector2(u.loc.x/10, u.loc.z/10));
				}
				foreach (var u in au)
				{
					AllyLoc.Add(new Vector2(u.loc.x/10, u.loc.z/10));
				}
				foreach (Vector2 al in AllyLoc)
				{
                    foreach (var op in cloesList)
                    {
						//print(op.x +"_" + al.x+"   "+ op.z + "_" + al.y);
						if (op.x==al.x &&op.z == al.y)
						{
							RemoveLoc.Add(new Vector2(op.x, op.z));
						}
                    }
				}
				
				if (RemoveLoc.Count!=0)
				{
					cloesList.RemoveAll(node => RemoveLoc.Any(vec => vec.x == node.x && vec.y == node.y));
					
				}
				return cloesList;
			}
			
		}
	}

	//public List<ASTarMgr> FindSkillRange()
	//{

	//}
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
	void FindNearlyToOpenList(int x, int y, float g, AStarNode father, AStarNode end, AStarNode[,] _nodes)
	{
		
		//�P�_�O�_�O ��� ���� �b�}�� ���� �C�� �����O�~��J�}�ҦC��
		if (x < 0 || x >= mapW || y < 0 || y >= mapH)
		{
			//Debug.Log(x+"_"+y);
			return;

		}
		////print(1);
		AStarNode node = _nodes[x, y];
		//switch (battleRound.instance.Phase)
		//{
		//	case 1:
		//		List<Unit_map_Date> eu = Map_Unit_Mgr.instance.Enemy_Unit;
		//		List<Vector2> EnemyLoc = new List<Vector2>();
		//		foreach (Unit_map_Date u in eu)
		//		{
		//			EnemyLoc.Add(new Vector2(u.loc.x / 10, u.loc.z / 10));
		//		}
		//		foreach (Vector2 el in EnemyLoc)
		//		{
		//			if (x == el.x && y == el.y)
		//			{
		//				Debug.Log("1");
		//				return;
		//			}
		//		}
		//		break;
		//	case 2:
		//		List<Unit_map_Date> pu = Map_Unit_Mgr.instance.Player_Unit;
		//		List<Vector2> PlayerLoc = new List<Vector2>();
		//		foreach (Unit_map_Date p in pu)
		//		{
					
		//			PlayerLoc.Add(new Vector2(p.loc.x / 10, p.loc.z / 10));
		//		}
		//		foreach (Vector2 pl in PlayerLoc)
		//		{
		//			if (x == pl.x && y == pl.y)
		//			{
		//				print(1);
		//				return;
		//			}
		//		}
		//		break;
		//	default:
		//		break;
		//}
		
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
		List<Unit_map_Date> eu = Map_Unit_Mgr.instance.Enemy_Unit;
		List<Vector2> EnemyLoc = new List<Vector2>();
		foreach (Unit_map_Date u in eu)
		{
			EnemyLoc.Add(new Vector2(u.loc.x/10, u.loc.z/10));
		}
		foreach (Vector2 el in EnemyLoc)
		{
			if (x==el.x && y == el.y)
			{
				return;
			}
		}
		if (node == null || node.type == Node_Type.Stop || cloesList.Contains(node) || openList.Contains(node))
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
				return null ;
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
}