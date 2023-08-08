using UnityEngine;
using System;
using System.Collections;
public class cursor_move : MonoBehaviour
{
	public static cursor_move instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public float moveDistance = 1f;
	public float moveSpeed = 1f;

	private Vector3 targetPosition;
	private bool isMoving;
	private GameObject UnitGrp;
	GameObject center_camera;
	GameObject Linghtbox;
	public Vector3 targetLoc = Vector3.one*-1;
	private void Start()
	{
		//Time.timeScale = 0.1f;
		  center_camera = GameObject.Find("center_camera");
		UnitGrp = GameObject.Find("All_Unit");
		Linghtbox = GameObject.Find("lightbox");
		

	}
	private void Update()
	{
		if (isMoving==false)
		{
			if (Input.GetKey(KeyCode.W))
			{
				StartMove(direction_correction(8));
			}
			else if (Input.GetKey(KeyCode.S))
			{
				StartMove(direction_correction(2));
			}
			else if (Input.GetKey(KeyCode.A))
			{
				StartMove(direction_correction(4));
			}
			else if (Input.GetKey(KeyCode.D))
			{
				StartMove(direction_correction(6));
			}
		}
		
	}

	private void StartMove(Vector3 direction)
	{

		ASTarMgr am = ASTarMgr.instance;
		float x = (transform.position.x + direction.x) / 10;
		float z = (transform.position.z + direction.z) / 10;
		//Debug.Log(x + "-" + z);

		if (am == null || am.nodes == null)
		{
			// 如果 am 或 am.nodes 是空值，直接返回
			return;
		}

		int width = am.nodes.GetLength(0);
		int height = am.nodes.GetLength(1);

		if (x < 0 || x >= width || z < 0 || z >= height)
		{
			// 如果 (int)x 或 (int)z 超出範圍，直接返回
			return;
		}

		if (am.nodes[(int)x, (int)z] == null)
		{
			// 如果節點不存在，直接返回
			return;
		}
		targetLoc = new Vector3(x * 10, 0, z * 10);
		if (!isMoving)
		{
			targetPosition = transform.position + direction;
			StartCoroutine(MoveCoroutine());
		}
	}
	private IEnumerator MoveCoroutine()
	{
		isMoving = true;

		AStarNode node = ASTarMgr.instance.nodes[(int)(targetLoc.x / 10), (int)(targetLoc.z / 10)];
		GetNodeInfo(node);
		Vector3 hight = new Vector3(0, sethight(node), 0);
		float startTime = Time.time;
		Vector3 startPosition = transform.position;
		Vector3 targetPosition = targetLoc + hight;

		while (transform.position!= targetPosition)
		{
			if (Vector2.Distance(new Vector2(startPosition.x,startPosition.z),new Vector2 (transform.position.x,transform.position.z)) > 5f)
			{
				Linghtbox.transform.position = targetLoc+new Vector3(0,0.3f)+hight;
			}
			float elapsedTime = Time.time - startTime;
			float t = Mathf.Clamp01(elapsedTime / (1 / moveSpeed));
			transform.position = Vector3.Lerp(startPosition, targetPosition, t);
			yield return null;
		}
		
		isMoving = false;
	}
	Vector3 direction_correction(int direction)
	{
		CameraController cc = CameraController.instance;
		float Rot = cc.realRotationY;

		switch (direction)
		{
			case 2:
				if (Rot > 45.1 && Rot < 134.9)
				{
					return new Vector3(-moveDistance, 0f, 0f);
				}
				else
				{
					if (Rot > 135.1 && Rot < 224.9)
					{
						return new Vector3(0f, 0f, moveDistance);
					}
					else
					{
						if (Rot > 225.1 && Rot < 314.9)
						{
							return new Vector3(moveDistance, 0f, 0f);


						}
						else
						{
							if (Rot > 315.1 || Rot < 44.9)
							{
								return new Vector3(0f, 0f, -moveDistance);

							}
						}
					}
				}
				break;
			case 4:
				if (Rot > 45.1 && Rot < 134.9)
				{

					return new Vector3(0f, 0f, moveDistance);
				}
				else
				{
					if (Rot > 135.1 && Rot < 224.9)
					{
						return new Vector3(moveDistance, 0f, 0f);
					}
					else
					{
						if (Rot > 225.1 && Rot < 314.9)
						{
							return new Vector3(0f, 0f, -moveDistance);


						}
						else
						{
							if (Rot > 315.1 || Rot < 44.9)
							{
								return new Vector3(-moveDistance, 0f, 0f);

							}
						}
					}
				}
				break;
			case 6:
				if (Rot > 45.1 && Rot < 134.9)
				{

					return new Vector3(0f, 0f, -moveDistance);
				}
				else
				{
					if (Rot > 135.1 && Rot < 224.9)
					{
						return new Vector3(-moveDistance, 0f, 0f);
					}
					else
					{
						if (Rot > 225.1 && Rot < 314.9)
						{
							return new Vector3(0f, 0f, moveDistance);



						}
						else
						{
							if (Rot > 315.1 || Rot < 44.9)
							{
								return new Vector3(moveDistance, 0f, 0f);


							}
						}
					}
				}
				break;
			case 8:
				if (Rot > 45.1 && Rot < 134.9)
				{
					return new Vector3(moveDistance, 0f, 0f);
				}
				else
				{
					if (Rot > 135.1 && Rot < 224.9)
					{
						return new Vector3(0f, 0f, -moveDistance);
					}
					else
					{
						if (Rot > 225.1 && Rot < 314.9)
						{
							return new Vector3(-moveDistance, 0f, 0f);

						}
						else
						{
							if (Rot > 315.1 || Rot < 44.9)
							{
								return new Vector3(0f, 0f, moveDistance);
							}
						}
					}
				}
				break;
			default:
				break;
		}
		return Vector3.zero;
	}
	public void GetNodeInfo(AStarNode node)
	{
		drawing_tool dt = drawing_tool.instance;
		//Debug.Log("(" + node.x + "," + node.y + ")");
		dt.LocText.text = "(" + node.x + "," + node.y + ")";
		dt.TypeText.text = node.type.ToString();
		dt.hightText.text = node.z.ToString();
		
	}
	public int sethight(AStarNode node)
	{
		//bool haveUnit = false;
		int y = node.z-1;
		//Debug.Log("高度="+ node.z);
		return (y * 4);
		//float father_y = 2.75f;
		//for (int i = 0; i < 3; i++)
		//{
		//	haveUnit = findUnitOnCursor(UnitGrp.transform.GetChild(i).gameObject);
		//	if (haveUnit)
		//	{
		//		father_y += 12f;
		//		break;
		//	}
		//}


		//transform.GetChild(0).localPosition = new Vector3(0, father_y,0);

		//if (targetLoc == Vector3.one * -1)
		//{
		//	transform.position = new Vector3(targetLoc.x, , targetLoc.z);
		//}
		//transform.position = new Vector3(targetLoc.x, y * 4, targetLoc.z);
		//Debug.Log(new Vector3(targetLoc.x, y * 4, targetLoc.z));




	}
	public GameObject findUnitOnCursor(GameObject fatherObj)
	{
		foreach (Transform child in fatherObj.transform)
		{
			if (child.position.x == transform.position.x && child.position.z == transform.position.z)
			{
				return child.gameObject;
			}
		}
		return null;
	}

	public Vector3 GetCursorLoc()
	{
		return transform.position;
	}

}
