using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Unit_Manage : MonoBehaviour
{
    public static AI_Unit_Manage instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public List<Unit_Data> Data = new List<Unit_Data>();
}
