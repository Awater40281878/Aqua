using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Collections.ObjectModel;
using UnityEngine.WSA;

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
	public int FindProtocol_EY(GameObject target)
	{
		cursor_move cm = cursor_move.instance;
		ASTarMgr am = ASTarMgr.instance;
		AStarNode node = am.nodes[(int)target.transform.position.x / 10, (int)target.transform.position.z / 10];
		starLoc = new Vector3(node.x, node.z, node.y);
		List<AStarNode> path = new List<AStarNode>();
		List<AStarNode> TempPath = new List<AStarNode>();
		List<Unit_map_Date> PlayerList = Map_Unit_Mgr.instance.Player_Unit;

		foreach (var Py in PlayerList)
		{
			if (path.Count == 0)
			{
				path = am.FindPath_AI_01(new Vector2(starLoc.x, starLoc.z), new Vector2(Py.loc.x / 10, Py.loc.z / 10));
			}
			else
			{
				TempPath = am.FindPath_AI_01(new Vector2(starLoc.x, starLoc.z), new Vector2(Py.loc.x / 10, Py.loc.z / 10));
			}
			if (TempPath.Count!= 0 && TempPath.Count < path.Count)
			{
				path = TempPath;
			}
			
		}
		//print(Protocol.Count);
		if (path.Count != 0)
		{
			print(path.Count);
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
		switch (battleRound.instance.Phase)
		{ 
			case 1:

				break;
			case 2:
				battleRound.instance.StartEY();
				break;
			default:
				break;
		}
		mr.enabled = true;
		lightbox.SetActive(true);
		bum.P_behavior = behaviorMod.Acted;
		bum.sortBattleUi(bum.original_Date);
		//Debug.Log("Movement completed.");
	}
	public void Resetprotocol()
	{
		starLoc = -Vector3.one;
		endLoc = -Vector3.one;
	}   
}
