using System;

[Serializable]
public enum polar
{
    /// <summary>
    /// ���y�� �F�_��
    /// </summary>
    Northeast,
    /// <summary>
    /// ���y�� ��_��
    /// </summary>
    Northwest,
    /// <summary>
    /// ���y�� �F�n��
    /// </summary>
    Southeast,
    /// <summary>
    /// ���y�� ��n��
    /// </summary>
    Southwest,
}

public class Example
{
    public polar direction;
}