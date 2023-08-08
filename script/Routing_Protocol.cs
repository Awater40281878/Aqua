using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routing_Protocol : MonoBehaviour
{
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
			AStarNode node = am.nodes[(int)cursor.x/10, (int)cursor.z/10];
			if (starLoc== Vector3.one * -1)
			{
				starLoc = new Vector3(node.x, node.z , node.y);
			}
			else
			{
				endLoc = new Vector3(node.x, node.z, node.y);
				if (starLoc != Vector3.one * -1  && endLoc != Vector3.one * -1)
				{
					Debug.Log(new Vector2(starLoc.x, starLoc.z) + "_____" + new Vector2(endLoc.x, endLoc.z));
					Protocol = ASTarMgr.instance.FindPath(new Vector2(starLoc.x, starLoc.z), new Vector2(endLoc.x, endLoc.z));
				}
			}
			if (Protocol.Count!=0)
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
}
