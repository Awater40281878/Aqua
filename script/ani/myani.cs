using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class myani : MonoBehaviour
{
    public GameObject si;
    public static myani instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
    public void Start_Battle()
	{
        
        StartCoroutine(Start_Battle_ani(1f));
    }
    //開始戰鬥動畫
    IEnumerator Start_Battle_ani(float costTime)
    {
        
        si.SetActive(true);
        Image filledImage = GameObject.Find("battleStartIcon").GetComponent<Image>();
        
        float elapsedTime = 0.0f;
        float startFillAmount = 1.0f;
        float targetFillAmount = 0.0f;

        while (elapsedTime < costTime)
        {
            // 使用插值函式計算當前填充量
            float currentFillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / costTime);

            // 將填充量設置到 Image 組件上
            filledImage.fillAmount = currentFillAmount;

            // 增加經過的時間
            elapsedTime += Time.deltaTime;

            // 等待下一個幀
            yield return null;
        }
        si.SetActive(false);
        // 確保填充量在結束時為目標值
        filledImage.fillAmount = targetFillAmount;
    }

}
