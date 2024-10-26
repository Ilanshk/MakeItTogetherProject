using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using UnityEngine.UI.TableUI;
using System.Linq;

public class BeforeInitMenuController : MonoBehaviour
{
    [SerializeField] private Button _initButton;
    [SerializeField] private Button _addItemButton;
    [SerializeField] private Button _removeItemButton;
    [SerializeField] private StudentScript _firstPlayerObjectData;
    [SerializeField] private StudentScript _secondPlayerObjectData;
    [SerializeField] private TMP_Dropdown _itemSelection;
    [SerializeField] private TableUI _listOfPurchases;
    [SerializeField] private TextMeshProUGUI _emptyListMessage;
    [SerializeField] private Button _emptyShoppingListButton;
    [SerializeField] private GameObject _emptyShoppingListAnnouncment;

    // Start is called before the first frame update

    private void Start()
    {
        _secondPlayerObjectData = GameObject.Find("StudentData").GetComponent<StudentScript>();//
        _secondPlayerObjectData.ListOfPurchases.Clear();
        _secondPlayerObjectData.ListOfPurchasesTemp.Clear();
        _listOfPurchases.gameObject.SetActive(false);
        //_emptyListMessage = GameObject.Find("EmptyPurchaseListMessage").GetComponent<TextMeshProUGUI>();
        Destroy(GameObject.Find("NetworkManager"));
        Destroy(GameObject.Find("LobbyManager"));
        Destroy(GameObject.Find("RelayManager"));
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("GameLobbyManager"));
        _emptyShoppingListAnnouncment.SetActive(false);
        //DontDestroyOnLoad(GameObject.Find("VivoxVoiceManager"));
    }
    private void OnEnable()
    {
        _initButton.onClick.AddListener(InitGame);
        _addItemButton.onClick.AddListener(AddItem);
        _removeItemButton.onClick.AddListener(RemoveItem);
        _emptyShoppingListButton.onClick.AddListener(HideEmptyShoppingListPanel);
    }

    private void RemoveItem()
    {
        int itemOption = _itemSelection.value;
        string item = _itemSelection.options[itemOption].text;
        if (_secondPlayerObjectData.ListOfPurchases.ContainsKey(item))
        {
            int temp = int.Parse(_secondPlayerObjectData.ListOfPurchases[item]);
            if (temp - 1 == 0)
            {
                _secondPlayerObjectData.ListOfPurchases.Remove(item);
                _secondPlayerObjectData.ListOfPurchasesTemp.Remove(item);
            }
            else
            {
                temp--;
                _secondPlayerObjectData.ListOfPurchases[item] = temp.ToString();
                _secondPlayerObjectData.ListOfPurchasesTemp[item] = temp.ToString();
            }
        }
        UpdateList();
    }

    private void AddItem()
    {
        int itemOption = _itemSelection.value;
        string item = _itemSelection.options[itemOption].text;
        if(_secondPlayerObjectData.ListOfPurchases.ContainsKey(item))
        {
            int temp = int.Parse(_secondPlayerObjectData.ListOfPurchases[item]);
            temp++;
            _secondPlayerObjectData.ListOfPurchases[item] = temp.ToString();
            _secondPlayerObjectData.ListOfPurchasesTemp[item] = temp.ToString();
        }
        else 
        {
            _secondPlayerObjectData.ListOfPurchases.Add(item, "1");
            _secondPlayerObjectData.ListOfPurchasesTemp.Add(item, "1");
        }
        UpdateList();
    }

    private void OnDisable()
    {
        _initButton.onClick?.RemoveListener(InitGame);
    }

    private void InitGame()
    {
        if (_secondPlayerObjectData.ListOfPurchases.Count == 0)
        {
            _emptyShoppingListAnnouncment.SetActive(true);
            _emptyListMessage.text = "עליך לבנות רשימת קניות לפני שתיכנס לסופרמרקט";

        }
        else {
            _emptyListMessage.text = "";
            _firstPlayerObjectData.PlayerName = "Teacher";
            _secondPlayerObjectData.PlayerName = "Student";
            SceneManager.LoadSceneAsync("Init");
        }
        
    }


    private void UpdateList()
    {
        if (_secondPlayerObjectData.ListOfPurchases.Count == 0)
            _listOfPurchases.gameObject.SetActive(false);
        else
            _listOfPurchases.gameObject.SetActive(true);    
        Dictionary<string, string> list = _secondPlayerObjectData.ListOfPurchases;
        var dataForListSuited = list.Select(x => Tuple.Create(x.Key, x.Value)).ToList();
        _listOfPurchases.Rows = list.Count + 1;
        for (int i = 1; i < _listOfPurchases.Rows; i++)
        {
            _listOfPurchases.GetCell(i, 0).GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
            _listOfPurchases.GetCell(i, 0).GetComponent<TextMeshProUGUI>().text = dataForListSuited[i-1].Item1;
            _listOfPurchases.GetCell(i, 1).GetComponent<TextMeshProUGUI>().text = dataForListSuited[i-1].Item2;
        }
    }

    private void HideEmptyShoppingListPanel() {
        _emptyShoppingListAnnouncment.SetActive(false);

    }
}
