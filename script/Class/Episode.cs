
using System;
using UnityEngine;
[Serializable]
public class Episode
{
	//關卡名稱
	public string name;
	//地圖模型
	public GameObject map_obj;
	////地圖偏移
	//public Vector3 mapOffset;
	//地圖資訊
	public TextAsset mapNode;
	//單位資料
	public TextAsset Unitdate;
	//是否可配置隊伍
	public bool canDispose;
	//是否為教學模式
	public bool teachMode;
}
