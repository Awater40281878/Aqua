using System.Threading;
using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    private const int TargetFrameRate = 100; // 目标帧率

    void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }

    void Update()
    {
        // 检查当前帧率
        if (1f / Time.deltaTime > TargetFrameRate)
        {
            // 计算需要等待的时间（以秒为单位）
            float waitTime = 1f / TargetFrameRate - Time.deltaTime;

            // 等待一段时间
            if (waitTime > 0)
            {
                Thread.Sleep((int)(waitTime * 1000));
            }
        }
    }
}
