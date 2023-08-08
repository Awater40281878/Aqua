using UnityEngine;
using TMPro;

public class job_menu: MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        // �M���{�����ﶵ
        dropdown.ClearOptions();

        // ���o Job_Type ���Ҧ��T�|��
        Job_Type[] jobTypes = (Job_Type[])System.Enum.GetValues(typeof(Job_Type));

        // �N�T�|���ഫ���r��÷s�W�� Dropdown ���ﶵ��
        foreach (Job_Type jobType in jobTypes)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(jobType.ToString()));
        }

        // ��s Dropdown �����
        dropdown.RefreshShownValue();
    }

    public void OnDropdownValueChanged(int index)
    {
        // ���o��ܪ� Job_Type
        Job_Type selectedJobType = (Job_Type)index;

        // �b���B����������ާ@�A�ھڿ�ܪ� Job_Type
        Debug.Log("Selected Job_Type: " + selectedJobType);
    }
}
