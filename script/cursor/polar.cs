using System;

[Serializable]
public enum polar
{
    /// <summary>
    /// 極座標 東北方
    /// </summary>
    Northeast,
    /// <summary>
    /// 極座標 西北方
    /// </summary>
    Northwest,
    /// <summary>
    /// 極座標 東南方
    /// </summary>
    Southeast,
    /// <summary>
    /// 極座標 西南方
    /// </summary>
    Southwest,
}

public class Example
{
    public polar direction;
}