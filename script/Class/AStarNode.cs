using System;
using System.Numerics;

[Serializable]
	public class AStarNode
	{
		public float f;
		public float g;
		public float h;
		public AStarNode father;
		public int x;
		public int y;
		public int z;
		public Node_Type type;
		public Buff_Type buff;

	public AStarNode(int _x, int _y, int _z, Node_Type _type, Buff_Type _buff)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
		this.type = _type;
		this.buff = _buff;
	}

	// ���B�ݭn�K�[�@�� DeepClone ��k
	public AStarNode DeepClone()
	{
		// �b�o�̽s�g AStarNode ���`�קJ���޿�
		// �A�ݭn��ʽƻs AStarNode ���Ҧ������ê�^�@�ӷs�����
		// �i��ݭn�`�קJ�� Buff_Type �M Node_Type �T�|�A������M�󥦭̪���{
		// �`�N�G�o�u�O�ܨҡA�A�ݭn�T�O�`�קJ���Ҧ�����
		AStarNode clone = new AStarNode(this.x, this.y, this.z, this.type, this.buff);
		clone.f = this.f;
		clone.g = this.g;
		clone.h = this.h;
		// �~��`�קJ����L����
		// ...
		return clone;
	}
}
