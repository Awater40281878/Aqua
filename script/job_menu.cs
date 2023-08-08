using UnityEngine;
using TMPro;

public class job_menu: MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        // 清除現有的選項
        dropdown.ClearOptions();

        // 取得 Job_Type 的所有枚舉值
        Job_Type[] jobTypes = (Job_Type[])System.Enum.GetValues(typeof(Job_Type));

        // 將枚舉值轉換為字串並新增到 Dropdown 的選項中
        foreach (Job_Type jobType in jobTypes)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(jobType.ToString()));
        }

        // 更新 Dropdown 的顯示
        dropdown.RefreshShownValue();
    }

    public void OnDropdownValueChanged(int index)
    {
        // 取得選擇的 Job_Type
        Job_Type selectedJobType = (Job_Type)index;

        // 在此處執行相應的操作，根據選擇的 Job_Type
        Debug.Log("Selected Job_Type: " + selectedJobType);
    }
}
