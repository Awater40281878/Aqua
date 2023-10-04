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
	//�}�l�԰��ʵe
	IEnumerator Start_BattleAni(float costTime)
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


	public void Start_CursorFloat()
	{

		cursorFloatCoroutine = StartCoroutine(Start_CursorFloatCoroutine(2f));

	}
	public void Stop_CursorFloat()
	{
		if (cursorFloatCoroutine != null) // �T�O��{���b�B�椤�ɤ~�Ұ�
		{
			StopCoroutine(cursorFloatCoroutine);
		}
	}
	private IEnumerator Start_CursorFloatCoroutine(float loopTime)
	{

		GameObject cursorObj = GameObject.Find("cursor").transform.GetChild(0).gameObject;
		Vector3 startPosition = cursorObj.transform.position;
		Vector3 aniPosition = startPosition;
		float floatHeight = 2f; // �W�U�B�ʪ�����
		float floatSpeed = floatHeight / loopTime; // �ھ� loopTime �p��B�ʳt��

		while (true) // �ϥ� cursorFloatRunning ����ʵe���B��
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
			// �b�C���ʵe�����ᵥ�ݤ@�q�ɶ��A�i��U�@���B��
			yield return new WaitForSeconds(0f); // ���B�i�H�վ㵥�ݮɶ�
		}
	}



}
