using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class aniContro : MonoBehaviour
{	
	//private string aniname = "soldierFontIlde";
	private string aniname;
	public polar UnitFont;
	public bool doubleSpeed;
	private int GetAniFont()
	{
		int Anifont =  -1;
		polar veiwFont = CameraController.instance.ViewFont;
		switch (veiwFont)
		{
			case polar.Northeast:
				//當我看向東北方
				switch (UnitFont)
				{
					case polar.Northeast:
						//當我看向東北方
						Anifont = 7;
						break;
					case polar.Northwest:
						//當我看向西北方
						Anifont = 9;
						break;
					case polar.Southeast:
						//當我看向東南方
						Anifont = 1;
						break;
					case polar.Southwest:
						//當我看向西南方
						Anifont = 3;
						break;
					default:
						break;
				}
				break;
			case polar.Northwest:
				//當我看向西方
				switch (UnitFont)
				{
					case polar.Northeast:
						//當我看向東北方
						Anifont = 9;
						break;
					case polar.Northwest:
						//當我看向西北方
						Anifont = 7;
						break;
					case polar.Southeast:
						//當我看向東南方
						Anifont = 3;
						break;
					case polar.Southwest:
						//當我看向西南方
						Anifont = 1;
						break;
					default:
						break;
				}
				break;
			case polar.Southeast:
				//當我看向南方
				switch (UnitFont)
				{
					case polar.Northeast:
						//當我看向東北方
						Anifont = 3;
						break;
					case polar.Northwest:
						//當我看向西北方
						Anifont = 1;
						break;
					case polar.Southeast:
						//當我看向東南方
						Anifont = 9;
						break;
					case polar.Southwest:
						//當我看向西南方
						Anifont = 7;
						break;
					default:
						break;
				}
				break;
			case polar.Southwest:
				//當我看向北方
				switch (UnitFont)
				{
					case polar.Northeast:
						//當我看向東北方
						Anifont = 1;
						break;
					case polar.Northwest:
						//當我看向西北方
						Anifont = 3;
						break;
					case polar.Southeast:
						//當我看向東南方
						Anifont = 9;
						break;
					case polar.Southwest:
						//當我看向西南方
						Anifont = 7;
						break;
					default:
						break;
				}
				break;
			default:
				break;
		}
		return Anifont;
	}
	//當我要播放動畫時，需要有個來源訊號。
	//訊號應該會使用 職業來判斷
	//當獲得訊號時，可以知道要播放哪個動畫
	//接下來要確定方向
	//方向確定之後
	//可以透過 動畫類型 和方向 確定要搜索哪個動畫
	//在來使用協程來播放動畫
	//如果動畫是 待機 就會迴圈播放
	//如果動畫是 攻擊 死亡 則會只播放一次
	//若動畫是 移動 則會迴圈播放到移動結束
	/*   以上是完整流程*/

	
	private void Start()
	{
		StartCoroutine(LookAtCamera());
		//StartCoroutine(PlayAnime(ani_mgr.instance.soldierAnimation.Find(a => a.Title == "soldierBackIlde")?.Animation));
	}


	/// <summary>
	/// 動畫播放器
	/// (aniList=要播放的圖片列表)
	/// </summary>
	//private IEnumerator PlayAnime(List<spirt_Date> aniList)
	//{
	//	SpriteRenderer sp = GetComponent<SpriteRenderer>();

	//	while (true)
	//	{
	//		foreach (spirt_Date animData in aniList)
	//		{
	//			sp.sprite = animData.aniSpirt;
	//			int frameCount = Mathf.RoundToInt(animData.DelayKey); // 延迟的帧数
	//			if (doubleSpeed)
	//			{
	//				frameCount /= 4; // 如果速度加倍，则延迟帧数减半
	//			}

	//			for (int i = 0; i < frameCount; i++)
	//			{
	//				yield return new WaitForEndOfFrame(); // 等待当前帧结束
	//			}
	//		}
	//	}
	//}
	private IEnumerator LookAtCamera()
	{
		
		while (true)
		{
			float rotay = CameraController.instance.realRotationY;
			//float rotax = CameraController.instance.rotationX;
				Quaternion targetRotation = Quaternion.Euler(0, rotay, 0f);
			transform.parent.rotation = targetRotation;
			yield return null;
		}
	}
	




	/// </summary>
	/// <param name="type"></param>
	/// <param name="font"></param>
	/// <returns></returns>
	private List<spirt_Date> GetAniList(Ani_Type type,int font)
	{
		ani_mgr animanager = FindObjectOfType<ani_mgr>();

		if (animanager != null)
		{
			// 根据aniname获取目标动画列表
			List<spirt_Date> animationList = animanager.soldierAnimation.Find(a => a.Title == aniname)?.Animation;

			if (animationList != null)
			{
				return animationList;
			}
			else
			{
				Debug.LogError("Animation list not found!");
			}
		}
		else
		{
			Debug.LogError("ani_mgr not found!");
		}
		return null;
	}
}
