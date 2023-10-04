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

	// 此處需要添加一個 DeepClone 方法
	public AStarNode DeepClone()
	{
		// 在這裡編寫 AStarNode 的深度克隆邏輯
		// 你需要手動複製 AStarNode 的所有成員並返回一個新的實例
		// 可能需要深度克隆 Buff_Type 和 Node_Type 枚舉，具體取決於它們的實現
		// 注意：這只是示例，你需要確保深度克隆所有成員
		AStarNode clone = new AStarNode(this.x, this.y, this.z, this.type, this.buff);
		clone.f = this.f;
		clone.g = this.g;
		clone.h = this.h;
		// 繼續深度克隆其他成員
		// ...
		return clone;
	}
}
