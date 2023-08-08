using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Unit_Mgr : MonoBehaviour
{
    public static Map_Unit_Mgr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    /// <summary>
	/// 以放置的 我方 單位
	/// </summary>
    public List<Unit_map_Date> Player_Unit_Loc = new List<Unit_map_Date>();
    /// <summary>
    /// 以放置的 敵方 單位
    /// </summary>
    public List<Unit_map_Date> Enemy_Unit = new List<Unit_map_Date>();
    /// <summary>
    /// 以放置的 友方 單位
    /// </summary>
    public List<Unit_map_Date> Ally_Unit = new List<Unit_map_Date>();
    
    public List<Unit_Data> Player_Unit = new List<Unit_Data>(4)
    {
         new Unit_Data("Saber",1,Job_Type.Saber,21,8,6,4,4,6,5,11,0,0,0,0),
         new Unit_Data("Tank",1,Job_Type.Tank,26,9,2,6,3,4,3,9,0,0,0,0),
         new Unit_Data("Witch",1,Job_Type.Witch,21,3,9,2,3,4,3,9,10,10,0,0),
    };
    public GameObject GetMgr()
	{
        return this.gameObject;
	}
    
}
