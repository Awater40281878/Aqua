
using System;
public class AStarNode
{
	public float f;//�M������
	public float g;//���_�I���Z��
	public float h;//�����I���Z��
	public AStarNode father;//����H
	public int x;
	public int y;
	public int z;
	public Node_Type type;//��l����+-
	public Buff_Type buff;
	public AStarNode(int _x, int _y, int _z, Node_Type _type,Buff_Type _buff)//�غc��k �y�лP����
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
		this.type = _type;
		this.buff = _buff;
	}
}
