using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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



			//�p�G�Ͱ_�C���ų��٨S�����|�A�N�O����
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
				//�䧹�F
				//List<AStarNode> path = new List<AStarNode>();
				//path.Add(end);
				//while (end.father != null)
				//{
				//	path.Add(end.father);
				//	end = end.father;
				//}
				//����
				//path.Reverse();
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
		AStarNode node = _nodes[x, y];

		if (node == null || node.type == Node_Type.Stop || cloesList.Contains(node) || openList.Contains(node))
		{

			return;
		}
		node.father = father;
		//g ���� �����_�I���Z��   =   ����H���_�I���Z�� + ��������H���Z��
		node.g = father.g + g;

		node.h = Mathf.Abs(end.x - node.x) + (Mathf.Abs(end.y - node.y));

		node.f = node.g + node.h;
		openList.Add(node);

	}
	public void FindNearlyToOpenList(int x, int y, AStarNode father, AStarNode[,] _nodes)
	{
		//�P�_�O�_�O ��� ���� �b�}�� ���� �C�� �����O�~��J�}�ҦC��
		if (x < 0 || x >= mapW || y < 0 || y >= mapH)
		{
			//Debug.Log(x+"_"+y);
			return;

		}
		AStarNode node = _nodes[x, y];

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
}