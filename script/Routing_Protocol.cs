using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static UnityEditor.Progress;

public class Routing_Protocol : MonoBehaviour
{
	public static Routing_Protocol instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	public Vector3 starLoc = Vector3.one * -1;
	public Vector3 endLoc = Vector3.one * -1;
	Unit_map_Date Write_Date;

	int XZmultiply = 10;
	int Ymultiply = 4;
	List<AStarNode> Protocol = new List<AStarNode>();
	//private void Update()
	//{

	//	//第一次按下D時，取得起點
	//	if (Input.GetKeyDown(KeyCode.F))
	//	{
	//		cursor_move cm = cursor_move.instance;
	//		ASTarMgr am = ASTarMgr.instance;
	//		Vector3 cursor = cm.gameObject.transform.position;
	//		AStarNode node = am.nodes[(int)cursor.x / 10, (int)cursor.z / 10];
	//		if (starLoc == Vector3.one * -1)
	//		{
	//			starLoc = new Vector3(node.x, node.z, node.y);
	//		}
	//		else
	//		{
	//			endLoc = new Vector3(node.x, node.z, node.y);
	//			if (starLoc != Vector3.one * -1 && endLoc != Vector3.one * -1)
	//			{
	//				Debug.Log(new Vector2(starLoc.x, starLoc.z) + "_____" + new Vector2(endLoc.x, endLoc.z));
	//				Protocol = ASTarMgr.instance.FindPath(new Vector2(starLoc.x, starLoc.z), new Vector2(endLoc.x, endLoc.z));
	//			}
	//		}
	//		if (Protocol.Count != 0)
	//		{
	//			foreach (var nodes in Protocol)
	//			{
	//				// 生成立方体
	//				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

	//				// 设置立方体的位置
	//				cube.transform.position = new Vector3(nodes.x * XZmultiply, nodes.z * Ymultiply, nodes.y * XZmultiply);
	//			}
	//		}


	//	}
	//}
	public int FindProtocol(GameObject target)
	{

		cursor_move cm = cursor_move.instance;
		ASTarMgr am = ASTarMgr.instance;
		Vector3 cursor = cm.gameObject.transform.position;
		AStarNode node = am.nodes[(int)cursor.x / 10, (int)cursor.z / 10];
		if (starLoc == Vector3.one * -1)
		{
			starLoc = new Vector3(node.x, node.z, node.y);
		}
		else
		{
			if (!generator.instance.Range.Contains(node))
			{
				Debug.Log("不在移動和圍內");
				return 0;
			}
			endLoc = new Vector3(node.x, node.z, node.y);
			if (starLoc != Vector3.one * -1 && endLoc != Vector3.one * -1)
			{
				//Debug.Log(new Vector2(starLoc.x, starLoc.z) + "_____" + new Vector2(endLoc.x, endLoc.z));
				Protocol = ASTarMgr.instance.FindPath(new Vector2(starLoc.x, starLoc.z), new Vector2(endLoc.x, endLoc.z));
			}
			if (Protocol.Count != 0)
			{
				StartCoroutine(MoveUnit(Protocol, target));
			}
		}

		return Protocol.Count;
	}
	public int FindProtocol_EY(GameObject target, int Mov)
	{

		cursor_move cm = cursor_move.instance;
		ASTarMgr am = ASTarMgr.instance;
		AStarNode node = am.nodes[(int)target.transform.position.x / 10, (int)target.transform.position.z / 10];
		starLoc = new Vector3(node.x, node.z, node.y);
		List<AStarNode> path = new List<AStarNode>();
		List<AStarNode> TempPath = new List<AStarNode>();
		List<Unit_map_Date> PlayerList = Map_Unit_Mgr.instance.Player_Unit;

		Vector2 endpos = Vector2.one * -1;
		path = ASTarMgr.instance.findend(new Vector2(target.transform.position.x / 10, target.transform.position.z / 10), Mov);

		//print(Protocol.Count);
		if (path==null)
		{
			path = new List<AStarNode>();
			path.Add(node);
		}
		if (path.Count != 0)
		{

			//print(path.Count);
			List<Unit_map_Date> datelist = new List<Unit_map_Date>();
			Unit_map_Date date = null;
			Map_Unit_Mgr mum = Map_Unit_Mgr.instance;
			if (mum != null)
			{
				foreach (var list in mum.Player_Unit)
				{
					datelist.Add(list);
				}
				foreach (var list in mum.Enemy_Unit)
				{
					datelist.Add(list);

				}
				foreach (var list in mum.Ally_Unit)
				{
					datelist.Add(list);
				}
			}

			//錯誤
			//print(datelist.Count);
			foreach (var list in datelist)
			{
				//print(list.loc +"___"+ target.transform.position);
				if (list.loc == target.transform.position)
				{
					date = list;
					//print("123");
				}

			}
			while (path.Count > date.date.Mov + 1)
			{
				int lastIndex = path.Count - 1; // 最後一個元素的索引
				path.RemoveAt(lastIndex); // 移除最後一個元素
			}
			if (path.Count != 1)
			{
				bool onunity = false;
				while (true)
				{
					int num = path.Count - 1;
					onunity = am.TargetLocOnUnit(new Vector2(path[num].x, path[num].y));
					if (onunity == false)
					{
						break;
					}
					path.RemoveAt(num);

				}
			}
			Write_Date = date;
			StartCoroutine(MoveUnit(path, target));

		}



		return Protocol.Count;

	}
	private IEnumerator MoveUnit(List<AStarNode> protocol, GameObject target)
	{
		cursor_move cm = cursor_move.instance;

		GameObject cursor = cm.gameObject;
		GameObject lightbox = cm.Linghtbox;
		MeshRenderer mr = cursor.transform.GetChild(0).GetComponent<MeshRenderer>();
		mr.enabled = false;
		lightbox.SetActive(false);

		float speed = 20f; // 移动速度
		int currentTargetIndex = 0; // 当前目标索引
		battleUIMgr bum = battleUIMgr.instance;
		bum.hideAllUi();
		while (currentTargetIndex < protocol.Count)
		{
			AStarNode targetNode = protocol[currentTargetIndex];
			Vector3 targetPosition = new Vector3(targetNode.x * XZmultiply, (targetNode.z - 1) * Ymultiply, targetNode.y * XZmultiply);
			float distance = Vector3.Distance(target.transform.position, targetPosition);
			while (distance > 0.1f)
			{
				Vector3 moveDirection = (targetPosition - target.transform.position).normalized;
				target.transform.position += moveDirection * speed * Time.deltaTime;
				cursor.transform.position = target.transform.position;
				lightbox.transform.position = target.transform.position;
				distance = Vector3.Distance(target.transform.position, targetPosition);
				yield return null;
			}
			cursor.transform.position = targetPosition;
			lightbox.transform.position = targetPosition;
			target.transform.position = targetPosition;
			currentTargetIndex++;

			yield return null;
		}
		mr.enabled = true;
		lightbox.SetActive(true);
		bum.P_behavior = behaviorMod.Acted;
		bum.sortBattleUi(bum.original_Date);
		switch (battleRound.instance.Phase)
		{
			case 1:

				break;
			case 2:
				Write_Date.loc = target.transform.position;
				battleUIMgr.instance.Write_UnitData(Write_Date);
				battleRound.instance.StartEY();
				break;
			default:
				break;
		}
		Resetprotocol();
		//Debug.Log("Movement completed.");

	}
	public void Resetprotocol()
	{
		starLoc = -Vector3.one;
		endLoc = -Vector3.one;
	}
}
