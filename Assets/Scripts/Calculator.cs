using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calculator : MonoBehaviour
{
    [SerializeField] private TMP_InputField _display;
    [SerializeField] private List<Button> _numberButtons;
    [SerializeField] private List<Button> _operationsButtons;
    [SerializeField] private Button _resButton;
    [SerializeField] private GameObject _messageText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _display.text = "";
        _display.onValueChanged.AddListener(LengthCheck);
        foreach (var button in _numberButtons)
        {
            button.onClick.AddListener(() => ChangeDisplay(button.GetComponentInChildren<TextMeshProUGUI>().text));
        }
        foreach (var button in _operationsButtons)
        {
            button.onClick.AddListener(() => ChangeDisplay(button.GetComponentInChildren<TextMeshProUGUI>().text));
        }
        _resButton.onClick.AddListener(CalculateResult);
        _messageText.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        _messageText.SetActive(false);
    }

    private void LengthCheck(string expression)
    {
        if(expression.Length > 24)
            _display.text = _display.text.Substring(0, 26);
    }

    private void OnDisable()
    {
        _display.text = "";
        foreach (var button in _numberButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        foreach (var button in _operationsButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        _resButton.onClick.RemoveAllListeners();
    }

    

    private void ChangeDisplay(string b)
    {
        if (b == "AC")
            _display.text = "";
        else if (b == "DEL" && b.Length > 0)
            _display.text = _display.text.Remove(_display.text.Length - 1);
        else
            _display.text += b;
    }
    private void CalculateResult()
    {
        List<string> numbersList;
        List<string> operandsList;
        if (CheckExp())
        {
            (numbersList, operandsList) = GetTokens(_display.text);
            if (numbersList.Count != 0 && operandsList.Count != 0)
            {
                var result = GetResult(numbersList, operandsList);
                if (result != null)
                    _display.text = result;
            }
        }
        
    }
    private bool CheckExp()
    {
        int textLength = _display.text.Length;
        if (_display.text[0] == '+' || _display.text[0] == '-')
        {
            _messageText.GetComponentInChildren<TextMeshProUGUI>().text = "שגיאה - קיימת פעולת חשבון בתחילת התרגיל";
            StartCoroutine(ShowMessage());
            return false;
        }
        else if (_display.text[textLength - 1] == '+' || _display.text[textLength - 1] == '-')
        {
            _messageText.GetComponentInChildren<TextMeshProUGUI>().text = "שגיאה-קיימת פעולת חשבון בסוף התרגיל";
            StartCoroutine(ShowMessage());
            return false;
        }
        for (int i = 0; i < textLength; i++)
        {
            if (i < textLength && (_display.text[i] == '+' || _display.text[i] == '-') && (_display.text[i + 1] == '+' || _display.text[i + 1] == '-'))
            {
                _messageText.GetComponentInChildren<TextMeshProUGUI>().text = "שגיאה-התרגיל שהוכנס לא תקין";
                StartCoroutine(ShowMessage());
                return false;
            }
        }
        return true;
            
    }

    private (List<string>, List<string>) GetTokens(string expression)
    {
        List<string> numbersList = new List<string>();
        List<string> operandsList = new List<string>();
        List<string> allTokens = new List<string>();
        var tempToken = "";
        for (int i = 0; i < expression.Length; i++)
        {
            if (expression[i] == '+' || expression[i] == '-')
            {
                allTokens.Add(tempToken);
                allTokens.Add(expression[i].ToString());
                tempToken = "";
                continue;
            }
            tempToken += expression[i];
            if(i == expression.Length-1)
                allTokens.Add(tempToken);
        }
        foreach (string token in allTokens)
        {
            if(token == "+" || token == "-")
                operandsList.Add(token);
            else
                numbersList.Add(token);
        }
        return (numbersList, operandsList);
    }

    private string GetResult(List<string> numList, List<string> opList)
    {
        string res = "";
        int firstNum, secondNum;
        for (int i = 0; i < opList.Count; i++)
        {
            if (i == 0)
            {
                firstNum = int.Parse(numList[i]);
            }
            else
                firstNum = int.Parse(res);
            secondNum = int.Parse(numList[i+1]);
            if(opList[i] == "+")
                res = (firstNum + secondNum).ToString();
            else
                res = (firstNum - secondNum).ToString();
        }
        return res;
    }

    IEnumerator ShowMessage() 
    {
        _messageText.SetActive(true);
        yield return new WaitForSecondsRealtime(5);
        _messageText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
