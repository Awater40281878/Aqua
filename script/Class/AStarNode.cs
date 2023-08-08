
using System;
public class AStarNode
{
	public float f;//尋路消耗
	public float g;//離起點的距離
	public float h;//離終點的距離
	public AStarNode father;//父對象
	public int x;
	public int y;
	public int z;
	public Node_Type type;//格子類型+-
	public Buff_Type buff;
	public AStarNode(int _x, int _y, int _z, Node_Type _type,Buff_Type _buff)//建構方法 座標與類型
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
		this.type = _type;
		this.buff = _buff;
	}
}
