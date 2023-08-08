using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class drawing_tool : MonoBehaviour
{
    public static drawing_tool instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
    }
    int mode = 1;
    Image Walkable;
	Image Stop;
    Image Delete;
    Image SetUp;
    GameObject Cursor;
    GameObject nodeGrp;
    public Material base_mat;
    public Material stop_mat;
    TextMeshProUGUI floorstext;
    public TextMeshProUGUI LocText;
    public TextMeshProUGUI TypeText;
    public TextMeshProUGUI hightText;
    public TextMeshProUGUI BuffText;
    // Start is called before the first frame update    cursor
    void Start()
    {
        GameObject sm = GameObject.Find("setmode");
		if (sm!=null)
		{
            Delete = GetChild(sm, 2).GetComponent<Image>();
            SetUp = GetChild(sm, 1).GetComponent<Image>();
        }

        GameObject du = GameObject.Find("drawUIsss");
		if (du!=null)
		{
            Walkable = GetChild(du, 1, 2, 1).GetComponent<Image>();
            Stop = GetChild(du, 1, 2, 2).GetComponent<Image>();
            floorstext = GetChild(du, 2, 3, 1).GetComponent<TextMeshProUGUI>();
            LocText = GetChild(du, 3, 2, 1, 1).GetComponent<TextMeshProUGUI>();
            TypeText = GetChild(du, 3, 2, 2, 1).GetComponent<TextMeshProUGUI>();
            hightText = GetChild(du, 3, 2, 3, 1).GetComponent<TextMeshProUGUI>();
            BuffText = GetChild(du, 3, 2, 4, 1).GetComponent<TextMeshProUGUI>();
        }
        Cursor = GameObject.Find("cursor");
        nodeGrp = GameObject.Find("Node_Map");
    }

    // Update is called once per frame
    void Update()
    {
		if (UIMgr.instance.mode[1]==true)
		{
            return;
		}
		else
		{
            if (IsButtonPressed(KeyCode.Q))
            {
                // 在此處處理 Q 鍵按下邏輯
                change_draw_mode("Q");
                Debug.Log("Q key pressed!");

            }
            if (IsButtonPressed(KeyCode.E))
            {
                // 在此處處理 E 鍵按下邏輯
                change_draw_mode("E");
                Debug.Log("E key pressed!");
            }
            if (IsButtonPressed(KeyCode.Space))
            {
                switch (Change_Scene.instance.GetMode())
                {
                    case 2:
                        Draw_Node();
                        break;
                    case 3:
                        switch (mode)
                        {
                            case 1:
                                UIMgr.instance.SetUpUnit();
                                break;
                            case 2:
                                UIMgr.instance.Remove_unit();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        return;
                }

                Debug.Log("Space key pressed!");

            }
            if (IsButtonPressed(KeyCode.Alpha1))
            {
                if (Change_Scene.instance.GetMode() != 2)
                {
                    return;
                }
                // 在此處處理 1 鍵按下邏輯
                change_floors("1");
                Debug.Log("1 key pressed!");
            }
            if (IsButtonPressed(KeyCode.Alpha2))
            {
                if (Change_Scene.instance.GetMode() != 2)
                {
                    return;
                }
                // 在此處處理 1 鍵按下邏輯
                change_floors("2");
                Debug.Log("2 key pressed!");
            }
        }
        
    }

    void Draw_Node()
    {
		
        float y = 0;
        float x= -1;
        float z = -1;
        ASTarMgr am = ASTarMgr.instance;
        GameObject target;
        x = Cursor.transform.position.x / 10;
        z = Cursor.transform.position.z / 10;
        target = GameObject.Find("(" + x + "," + z + ")");
        y = Int32.Parse(floorstext.text) - 1;
        AStarNode node = am.nodes[(int)x, (int)z];
        target.transform.position = new Vector3(target.transform.position.x,y*4, target.transform.position.z);
        switch (mode)
		{
            case 1:
                //Debug.Log("設置walk");
                node.type = Node_Type.walk;
                node.z = (int)y+1;
                //Debug.Log(target.name);
                target.GetComponent<MeshRenderer>().material = base_mat;
                break;
            case 2:
                //Debug.Log("設置Stop");
                node.type = Node_Type.Stop;
                node.z = (int)y+1;
                //Debug.Log(target.name);
                target.GetComponent<MeshRenderer>().material = stop_mat;
                break;
            default:
                Debug.Log("設置失敗");
                break;
		}
        cursor_move.instance.GetNodeInfo(node);
        cursor_move.instance.sethight(node);  


    }
    bool IsButtonPressed(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode);
    }

    void change_draw_mode(string keycode)
	{
        Image img1;
        Image img2;
        int weight_max=1;
        int weight_min = 1;
        switch (Change_Scene.instance.GetMode())
        {
            case 2:
                weight_max =  GameObject.Find("draw_mode").transform.childCount;
                img1 = Walkable;
                img2 = Stop;
                break;
            case 3:
                weight_max = GameObject.Find("setmode").transform.childCount;
                img1 = SetUp;
                img2 = Delete;
                break;
            default:
                return;
        }
        
        if (keycode=="Q")
		{
			if (mode== weight_min)
			{
                return;
			}
            mode--;

        }
        if (keycode == "E")
        {
			if (mode== weight_max)
			{
                return;
            }
            mode++;
		}
		
		switch (mode)
		{
            case 1:
                img1.color = Color.green;
                img2.color = Color.white;
                break;
            case 2:
                img1.color = Color.white;
                img2.color = Color.green;
                break;
            default:
				break;
		}
	}
    void change_floors(string keycode)
	{
		switch (Int32.Parse(keycode))
		{
            case 1:
                floorstext.text = (Int32.Parse(floorstext.text) + 1).ToString();
                break;
            case 2:
                floorstext.text = (Int32.Parse(floorstext.text) - 1).ToString();
                break;
			default:
				break;
		}
	}
    GameObject GetChild(GameObject father, int floor_1)
    {
        return father.transform.GetChild(floor_1 - 1).gameObject;
	}

	GameObject GetChild(GameObject father, int floor_1, int floor_2)
	{
		return father.transform.GetChild(floor_1 - 1).transform.GetChild(floor_2 - 1).gameObject;
	}
    GameObject GetChild(GameObject father, int floor_1, int floor_2, int floor_3)
    {
        return father.transform.GetChild(floor_1 - 1).transform.GetChild(floor_2 - 1).transform.GetChild(floor_3 - 1).gameObject;
    }
    GameObject GetChild(GameObject father, int floor_1, int floor_2, int floor_3,int floor_4)
    {
        return father.transform.
            GetChild(floor_1 - 1).transform.
            GetChild(floor_2 - 1).transform.
            GetChild(floor_3 - 1).transform.
            GetChild(floor_4 - 1).transform.gameObject;
    }
}
