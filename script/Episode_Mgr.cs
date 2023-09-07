using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Episode_Mgr : MonoBehaviour
{
    public static Episode_Mgr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
    }
    public GameObject map;
    public GameObject nodemap;
    public GameObject AllUnit;
    public List<Episode> All_Episode;
    public void StartBattle()
    {
        UIMgr Ui = UIMgr.instance;
        GameObject lui = Ui.GetChild(GameObject.Find("battleUI"), 1);
        lui.SetActive(false);
        Battle();

    }


    private void Battle()
	{
        
        //�M�� �a�� ��l ���
        List<GameObject> mapdate = new List<GameObject>();
        mapdate.Add(map);
        mapdate.Add(nodemap);
        mapdate.Add(AllUnit.transform.GetChild(0).gameObject);
        mapdate.Add(AllUnit.transform.GetChild(1).gameObject);
        mapdate.Add(AllUnit.transform.GetChild(2).gameObject);
		foreach (var item in mapdate)
		{
            DestroyAllChild(item);

        }
        UIMgr Ui = UIMgr.instance;
        GameObject ld = Ui.GetChild(GameObject.Find("battleUI"), 1, 2);
        TMP_Dropdown td = ld.GetComponent<TMP_Dropdown>();
        int level = td.value;
        Episode thisEp = All_Episode[level];
        //Ū�� �a�� ��l ���
        UIMgr um = UIMgr.instance;
		GameObject thismap = Instantiate(thisEp.map_obj, Vector3.zero, Quaternion.identity);
        thismap.transform.parent = map.transform;
        thismap.transform.localPosition = Vector3.zero;
        thismap.transform.localRotation = Quaternion.identity;
        thismap.transform.localScale = Vector3.one;
        um.load_node(thisEp.mapNode);
        um.load_map_unit(thisEp.Unitdate);
    }
    public void DestroyAllChild(GameObject father)
    {
        // �T�{�����w������
        if (father == null)
        {
            Debug.LogWarning("�����󬰪šA�L�k���� DestroyAllChild �禡�C");
            return;
        }

        // �M���úR���Ҧ��l����
        foreach (Transform child in father.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
