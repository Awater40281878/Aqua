using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldRange : MonoBehaviour
{
    public InputField inputField;
    public int minValue;
    public int maxValue;
    private string previousText;

    private void Start()
    {
        inputField = GetComponent<InputField>();
        // 添加事件侦听器以在文本更改时调用特定方法
        inputField.onValueChanged.AddListener(OnTextChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
        previousText = inputField.text;
    }

    private void OnTextChanged(string newText)
    {
        // 保存当前文本
        previousText = newText;
    }

    private void OnEndEdit(string newText)
    {
        // 尝试将输入的文本转换为整数
        if (int.TryParse(newText, out int value))
        {
            // 如果值超出范围，则限制在范围内
            value = Mathf.Clamp(value, minValue, maxValue);

            // 更新输入字段的文本为限制后的值
            inputField.text = value.ToString();
        }
        else
        {
            // 输入的文本无法转换为整数，恢复为之前的文本
            inputField.text = previousText;
        }
    }

    private void OnDeselect(BaseEventData eventData)
    {
        // 在失去焦点时进行数值限制
        OnEndEdit(inputField.text);
    }
}

