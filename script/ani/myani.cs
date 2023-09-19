using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class myani : MonoBehaviour
{
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
		GameObject animeUI = UIMgr.instance.GetChild(GameObject.Find("UIanime"), 1);
		animeUI.SetActive(true);
		StartCoroutine(Start_Battle_ani(4f));
    }
	//�}�l�԰��ʵe
	IEnumerator Start_Battle_ani(float costTime)
	{
		Image img = GameObject.Find("UIanime").transform.GetChild(0).GetComponent<Image>();

		float targetAlpha = 1.0f; // �̲׳z����
		float halfTime = costTime / 4.0f; // �q0��255���@�b�ɶ�

		// �q0��255���z���׼W�[�t��
		float deltaAlpha = targetAlpha / halfTime;

		// �q0��255������
		while (img.color.a < targetAlpha)
		{
			Color newColor = img.color;
			newColor.a += deltaAlpha * Time.deltaTime;
			img.color = newColor;
			yield return null;
		}

		// ���ݤ@�q�ɶ��A���z���׫O���b255
		yield return new WaitForSeconds(halfTime);

		// �q255��0���z���״�p�t��
		deltaAlpha = targetAlpha / halfTime;

		// �q255��0������
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
		
		// �T�O�z���׳̲׬�0
		Color finalColor = img.color;
		finalColor.a = 0;
		img.color = finalColor;
		img.gameObject.SetActive(false);
		// �b������A�i�H�����L�ާ@�A�Ϊ̦b�H�J�H�X�������~���޿�
	}
	
}
