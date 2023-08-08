using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
public class generator : MonoBehaviour
{
	public static generator instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public InputField _width;
	public InputField _length;
	public int begiX = -3;
	public int begiY = 5;
	//�C�ӥߤ��餧��������
	public float offsetX = 1;
	public float offsetY = 1;
	public GameObject PNode;
	GameObject nodeGrp;
	public Material bule;
	public Material red;
	// Start is called before the first frame update
	void Start()
	{
		_width = GameObject.Find("widthValue").GetComponent<InputField>();
		_length = GameObject.Find("lengthValue").GetComponent<InputField>();
		nodeGrp = GameObject.Find("Node_Map");
	}

	// Update is called once per frame
	public void generate_node()
	{
		if (nodeGrp.transform.childCount != 0)
		{
			for (int i = nodeGrp.transform.childCount - 1; i >= 0; i--)
			{
				Destroy(nodeGrp.transform.GetChild(i).gameObject);
			}
		}

		ASTarMgr am = ASTarMgr.instance;
		am.InitMapInfo(Int32.Parse(_width.text), Int32.Parse(_length.text));
		for (int i = 0; i < am.nodes.GetLength(0); i++)
		{
			for (int j = 0; j < am.nodes.GetLength(1); j++)
			{
				GameObject cube = Instantiate(PNode, new Vector3((begiX + i) * offsetX, 0, (begiY + j) * offsetY), PNode.transform.rotation);
				cube.name = "(" + i + "," + j + ")";
				cube.transform.parent = nodeGrp.transform;
			}
		}

	}
	public void generate_node(AStarNode[,] nodemap)
	{
		if (nodeGrp.transform.childCount != 0)
		{
			for (int i = nodeGrp.transform.childCount - 1; i >= 0; i--)
			{
				Destroy(nodeGrp.transform.GetChild(i).gameObject);
			}
		}

		

		for (int i = 0; i < nodemap.GetLength(0); i++)
		{
			for (int j = 0; j < nodemap.GetLength(1); j++)
			{
				AStarNode node = nodemap[i, j];
				GameObject cube = Instantiate(PNode, new Vector3((begiX + node.x) * offsetX, (node.z- 1) * 4, (begiY + node.y) * offsetY), PNode.transform.rotation);
				cube.name = "(" + i + "," + j + ")";
				cube.transform.parent = nodeGrp.transform;
				if (node.type == Node_Type.walk)
				{
					
					cube.GetComponent<MeshRenderer>().material = bule;
				}
				else
				{
					if (node.type == Node_Type.Stop)
					{
						cube.GetComponent<MeshRenderer>().material = red;
					}
				}
			}
		}

	}

}
