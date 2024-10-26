using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using UnityEngine.UI;
using Unity.Services.Vivox;

public class StudentCanvaManager : MonoBehaviour
{
    [SerializeField] private Button _addOneButton;
    [SerializeField] private Button _addTwoButton;
    [SerializeField] private Button _passToNextButton;
    [SerializeField] private Canvas _remainCanva;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private TextMeshProUGUI _studentResult;
    [SerializeField] private TextMeshProUGUI _totalSum;
    [SerializeField] private TextMeshProUGUI _paidSum;
    [SerializeField] private Slider _progress;
    private int _progressValue = 0;

    public int ProgressValue
    {
        get => _progressValue;
        set => _progressValue = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        int temp = GameObject.Find("StudentData").GetComponent<StudentScript>().GetTotalSum();
        _totalSum.text = temp.ToString();
        _progress.maxValue = temp;
        _passToNextButton.gameObject.SetActive(false);
        if (this.GetComponent<MakeLevelSimulationMultiplayer>().GetLevelName() == "2TaskCM")
            GameObject.Find("GameManager").GetComponent<GameManager>().SetTotalSumServerRpc(GameObject.Find("StudentData").GetComponent<StudentScript>().GetTotalSum());
        GameObject.Find("GameManager").GetComponent<GameManager>().SetUpUIServerRpc();

    }

    private void OnEnable()
    {
        _passToNextButton.onClick.AddListener(() => 
        { 
            this.GetComponent<MakeLevelSimulationMultiplayer>().MoveToNextLevel();
            GameObject.Find("GameManager").GetComponent<GameManager>().SetUpUIServerRpc();
            StartCoroutine(SetFalseButton(0.1f));

        });
    }

    void Update()
    {
        if(_studentResult.text == "0")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().SetUpUIServerRpc();
        }


    }

    [ClientRpc]
    public void AddNumberClientRpc(int num)
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().GameManagerWorkServerRpc(num);
        int totalSum = GameObject.Find("GameManager").GetComponent<GameManager>().TotalSum.Value;
        int temp = GameObject.Find("GameManager").GetComponent<GameManager>()._currNum.Value;
        if (this.GetComponent<MakeLevelSimulationMultiplayer>().GetLevelName() == "2TaskCM")
        {
            if(this.GetComponentInParent<NetworkObject>().OwnerClientId != 0)
                temp += num;
            if (temp > totalSum)
            {
                _message.text = "תודה רבה, עכשיו צריך לקבל עודף";
                StartCoroutine(ShowMessage(2));
                _passToNextButton.gameObject.SetActive(true);
                CheckConditionsClientRpc(temp);
                _progress.value = totalSum;
            }
            else if (temp == totalSum)
            {
                EndGameSetup();
                _progress.value += num;
            }
            else
                _progress.value += num;
        }
        else
        {
            if (this.GetComponentInParent<NetworkObject>().OwnerClientId != 0)
                temp += num;
            if (temp > totalSum)
            {
                _message.text = "חרגת מהעודף שצריך לקבל. תנסה/י שוב";
                StartCoroutine(ShowMessage(2));
                GameObject.Find("GameManager").GetComponent<GameManager>().SetCurrNumServerRpc(temp - num);
                GameObject.Find("GameManager").GetComponent<GameManager>().SetUpUIServerRpc();
                _progressValue += num;
                int temp1 = GameObject.Find("GameManager").GetComponent<GameManager>()._currNum.Value;
                int temp2 = temp1;
            }
            else if (temp <= totalSum)
            {
                _progressValue += num;
                _progress.value += num;
                if (temp == totalSum)
                    EndGameSetup();
            }
        }
        //GameObject.Find("GameManager").GetComponent<GameManager>().SetUpUIServerRpc();
        


    }


    [ClientRpc]
    public void CheckConditionsClientRpc(int currSum)
    {
        int totalSum = currSum - GameObject.Find("GameManager").GetComponent<GameManager>().TotalSum.Value;
        GameObject.Find("StudentData").GetComponent<StudentScript>().RemainAmount = totalSum; 
        _progress.maxValue = totalSum;
        GameObject.Find("GameManager").GetComponent<GameManager>().MakeZeroCurrentNumServerRpc();
        _paidSum.text = currSum.ToString();
        GameObject.Find("GameManager").GetComponent<GameManager>().SetTotalSumServerRpc(totalSum);
    }

    private void EndTheGame()
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            VivoxService.Instance.LeaveAllChannelsAsync();
        }
        AuthenticationService.Instance.SignOut();
        VivoxService.Instance.LogoutAsync();
        SceneManager.LoadSceneAsync("SampleScene");
    }

    private void EndGameSetup()
    {
        
        _message.color = Color.green;
        _message.text = "סיימת את המשחק. צא/י לתפריט הראשי";
        StartCoroutine(ShowMessage(2));
        _passToNextButton.gameObject.SetActive(true);
        _passToNextButton.GetComponentInChildren<TextMeshProUGUI>().isRightToLeftText = true;
        _passToNextButton.GetComponentInChildren<TextMeshProUGUI>().text = "יציאה";
        _passToNextButton.onClick.RemoveAllListeners();
        _passToNextButton.onClick.AddListener(() => { EndTheGame(); });
    }

    IEnumerator ShowMessage(int sec)
    {
        _message.enabled = true;
        yield return new WaitForSecondsRealtime(sec);
        _message.enabled = false;

    }

    IEnumerator SetFalseButton(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        _passToNextButton.gameObject.SetActive(false);
    }


}
