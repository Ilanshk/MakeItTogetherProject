using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.TableUI;

public class StudentListManager : MonoBehaviour
{
    [SerializeField] private TableUI _studentList;
    [SerializeField] private TextMeshProUGUI _message;

    public void EditList(string productName)
    {
        for(int i = 1; i < _studentList.Rows; i++)
        {
            if(_studentList.GetCell(i,0).text == productName && _studentList.GetCell(i, 0))
            {
                int amount = int.Parse(_studentList.GetCell(i, 1).text);
                if (amount - 1 != 0)
                {
                    _studentList.GetCell(i, 1).text = (amount - 1).ToString();
                }
                else
                {
                    _studentList.GetCell(i, 1).text = (amount - 1).ToString();
                    _studentList.GetCell(i, 0).fontStyle = TMPro.FontStyles.Strikethrough;
                    _studentList.GetCell(i, 1).fontStyle = TMPro.FontStyles.Strikethrough;
                }
                break;
            }
        }
    }
    public void MessageType(int messageType)
    {
        if(messageType == 0)
        {
            _message.text = "המוצר הזה אינו ברשימה. תסתכל/י ברשימה שוב";
            StartCoroutine(ShowMessage(3));
        }
        else if(messageType == 1)
        {
            _message.text = "כבר אספת את כמות הנדרשת של המוצר. תסתכל/י ברשימה שוב";
            StartCoroutine(ShowMessage(3));
        }
        else if(messageType == 3)
        {
            _message.text = "אספת את כל המוצרים ברשימה. תיגש/י לקופה";
            StartCoroutine(ShowMessage(5));
        }
    }
    
    public void TurnOffCanva()
    {
        StartCoroutine(TurnOff());
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSecondsRealtime(6);
        this.gameObject.SetActive(false);
    }

    IEnumerator ShowMessage(int sec)
    {
        _message.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(sec);
        _message.gameObject.SetActive(false);
        
    }
}
