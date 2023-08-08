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
    //�}�l�԰��ʵe
    IEnumerator Start_Battle_ani(float costTime)
    {
        
        si.SetActive(true);
        Image filledImage = GameObject.Find("battleStartIcon").GetComponent<Image>();
        
        float elapsedTime = 0.0f;
        float startFillAmount = 1.0f;
        float targetFillAmount = 0.0f;

        while (elapsedTime < costTime)
        {
            // �ϥδ��Ȩ禡�p���e��R�q
            float currentFillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / costTime);

            // �N��R�q�]�m�� Image �ե�W
            filledImage.fillAmount = currentFillAmount;

            // �W�[�g�L���ɶ�
            elapsedTime += Time.deltaTime;

            // ���ݤU�@�ӴV
            yield return null;
        }
        si.SetActive(false);
        // �T�O��R�q�b�����ɬ��ؼЭ�
        filledImage.fillAmount = targetFillAmount;
    }

}
