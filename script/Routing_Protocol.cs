using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
	Vector3 starLoc = Vector3.one * -1;
	Vector3 endLoc = Vector3.one * -1;

	int XZmultiply = 10;
	int Ymultiply = 4;
	List<AStarNode> Protocol = new List<AStarNode>();
	private void Update()
	{

		//第一次按下D時，取得起點
		if (Input.GetKeyDown(KeyCode.F))
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
				endLoc = new Vector3(node.x, node.z, node.y);
				if (starLoc != Vector3.one * -1 && endLoc != Vector3.one * -1)
				{
					Debug.Log(new Vector2(starLoc.x, starLoc.z) + "_____" + new Vector2(endLoc.x, endLoc.z));
					Protocol = ASTarMgr.instance.FindPath(new Vector2(starLoc.x, starLoc.z), new Vector2(endLoc.x, endLoc.z));
				}
			}
			if (Protocol.Count != 0)
			{
				foreach (var nodes in Protocol)
				{
					// 生成立方体
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

					// 设置立方体的位置
					cube.transform.position = new Vector3(nodes.x * XZmultiply, nodes.z * Ymultiply, nodes.y * XZmultiply);
				}
			}


		}
	}
	public int FindProtocol(GameObject target)
	{
		
		cursor_move cm = cursor_move.instance;
		ASTarMgr am = ASTarMgr.instance;
		Vector3 cursor = cm.gameObject.transform.position;
		AStarNode node = am.nodes[(int)cursor.x / 10, (int)cursor.z / 10];
		if (starLoc == Vector3.one * -1)
		{
			starLoc = new Vector3(node.x, node.z, node.y);
			//print(starLoc);
		}
		else
		{

			if (!generator.instance.Range.Contains(node))
			{
				Debug.Log("不在移動和圍內");
				return 0;
			}
			endLoc = new Vector3(node.x, node.z, node.y);
			//print(endLoc);

			if (starLoc != Vector3.one * -1 && endLoc != Vector3.one * -1)
			{
				//Debug.Log(new Vector2(starLoc.x, starLoc.z) + "_____" + new Vector2(endLoc.x, endLoc.z));
				Protocol = ASTarMgr.instance.FindPath(new Vector2(starLoc.x, starLoc.z), new Vector2(endLoc.x, endLoc.z));
				print(Protocol.Count);

			}
		}
		if (Protocol.Count != 0)
		{
			//print(123);
			//foreach (var nodes in Protocol)
			//{
			//	// 生成立方体
			//	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

			//	// 设置立方体的位置
			//	cube.transform.position = new Vector3(nodes.x * XZmultiply, nodes.z * Ymultiply, nodes.y * XZmultiply);
			//}
			StartCoroutine(MoveUnit(Protocol, target));
			//Debug.LogError("123");
		}

		
		return Protocol.Count;
		//else
		//{
		//	Debug.LogError("null");

		//}
	}

	private IEnumerator MoveUnit(List<AStarNode> protocol, GameObject target)
	{
		float speed = 20f; // 移动速度
		int currentTargetIndex = 0; // 当前目标索引
		battleUIMgr bum = battleUIMgr.instance;
		bum.hideAllUi();
		while (currentTargetIndex < protocol.Count)
		{
			AStarNode targetNode = protocol[currentTargetIndex];
			Vector3 targetPosition = new Vector3(targetNode.x * XZmultiply, (targetNode.z - 1) * Ymultiply, targetNode.y * XZmultiply);

			print(targetPosition);
			float distance = Vector3.Distance(target.transform.position, targetPosition);	

			while (distance > 0.1f)
			{
				Vector3 moveDirection = (targetPosition - target.transform.position).normalized;
				target.transform.position += moveDirection * speed * Time.deltaTime;
				distance = Vector3.Distance(target.transform.position, targetPosition);
				yield return null;
			}

			target.transform.position = targetPosition;
			currentTargetIndex++;
			yield return null;
		}
		bum.sortBattleUi(bum.original_Date);
		Debug.Log("Movement completed.");
	}

}
