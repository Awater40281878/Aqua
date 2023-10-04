using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class myani : MonoBehaviour
{
	public static myani instance;
	private Coroutine cursorFloatCoroutine;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
	}
	public void Start_Battle()
	{
		GameObject animeUI = UIMgr.instance.GetChild(GameObject.Find("UIanime"), 1);
		animeUI.SetActive(true);
		StartCoroutine(Start_BattleAni(4f));
	}
	//開始戰鬥動畫
	IEnumerator Start_BattleAni(float costTime)
	{
		Image img = GameObject.Find("UIanime").transform.GetChild(0).GetComponent<Image>();

		float targetAlpha = 1.0f; // 最終透明度
		float halfTime = costTime / 4.0f; // 從0到255的一半時間

		// 從0到255的透明度增加速度
		float deltaAlpha = targetAlpha / halfTime;

		// 從0到255的漸顯
		while (img.color.a < targetAlpha)
		{
			Color newColor = img.color;
			newColor.a += deltaAlpha * Time.deltaTime;
			img.color = newColor;
			yield return null;
		}

		// 等待一段時間，讓透明度保持在255
		yield return new WaitForSeconds(halfTime);

		// 從255到0的透明度減小速度
		deltaAlpha = targetAlpha / halfTime;

		// 從255到0的漸隱
		while (img.color.a > 0)
		{
			Color newColor = img.color;
			newColor.a -= deltaAlpha * Time.deltaTime;
			img.color = newColor;
			yield return null;
		}
		switch (battleRound.instance.Phase)
		{
			case 1:

				break;
			case 2:
				battleRound.instance.StartEY();
				break;
			default:
				break;
		}

		// 確保透明度最終為0
		Color finalColor = img.color;
		finalColor.a = 0;
		img.color = finalColor;
		img.gameObject.SetActive(false);
		// 在此之後你可以執行其他操作，或者在淡入淡出完成後繼續邏輯
	}


	public void Start_CursorFloat()
	{

		cursorFloatCoroutine = StartCoroutine(Start_CursorFloatCoroutine(2f));

	}
	public void Stop_CursorFloat()
	{
		if (cursorFloatCoroutine != null) // 確保協程未在運行中時才啟動
		{
			StopCoroutine(cursorFloatCoroutine);
		}
	}
	private IEnumerator Start_CursorFloatCoroutine(float loopTime)
	{

		GameObject cursorObj = GameObject.Find("cursor").transform.GetChild(0).gameObject;
		Vector3 startPosition = cursorObj.transform.position;
		Vector3 aniPosition = startPosition;
		float floatHeight = 2f; // 上下浮動的高度
		float floatSpeed = floatHeight / loopTime; // 根據 loopTime 計算浮動速度

		while (true) // 使用 cursorFloatRunning 控制動畫的運行
		{
			float timer = 0f;
			while (timer < loopTime)
			{
				float yOffset = Mathf.Sin(timer / loopTime * Mathf.PI * 2 * floatSpeed) * floatHeight;
				Vector3 newPosition = aniPosition + Vector3.up * yOffset;

				cursorObj.transform.position = newPosition;

				timer += Time.deltaTime;
				yield return null;
			}

			cursorObj.transform.position = startPosition;
			// 在每次動畫完成後等待一段時間再進行下一次浮動
			yield return new WaitForSeconds(0f); // 此處可以調整等待時間
		}
	}



}
