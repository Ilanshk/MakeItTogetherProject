using Firebase.Database;
using Firebase;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.UI;
using Unity.VisualScripting;
using Game.Events;
using UnityEngine.ProBuilder.MeshOperations;
using System.Globalization;

[Serializable]
public class StudentScript : MonoBehaviour {
    private int _remainAmount = 0;
    private string _playerName; //[SerializeField]
    [SerializeField] private string _firstName;
    [SerializeField] private string _lastName;
    [SerializeField] private string _groupName;
     private int _identifyingCoinsBills = 0;
     private int _understandingValue = 0;
     private int _cheapExpensive = 0;
     private int _changeCalc = 0;
    private List<string> categories = new() { "_recognizeNumber", "_recognizeSmallestOrBiggestNumber", "_recognizeCoins", "_matchCoinToValue", "_constructSumWithCoins", "_constructSumWithCoinsAndBills", "_recognizeBills", "_matchBillToValue", "_comparingOwnSumToProductPrice", "_distinguishCheapExpensive", "_chooseProductMatchingToOwnMoney", "_recognizeChangeGet", "_recognizeNoChange", "_calcChange" };

    //הכרת המספרים
    private int _recognizeNumber = 0; //5,10,20,100,1000

    //private Dictionary<int, int> _recognizeNumbers = new() { { 5,0},{10,0},{20,0},{100,0},{ 1000, 0} };
    //השוואת מספרים
    private int _recognizeSmallestOrBiggestNumber = -1; //boolean

    //מטבעות
    private Dictionary<int, int> _recognizeCoins = new() { { 1, 0 }, { 2, 0 }, { 5, 0 }, { 10, 0 } };
    private Dictionary<int, int> _matchCoinToValue = new() { { 1, 0 }, { 2, 0 }, { 5, 0 }, { 10, 0 } };
    private int _constructSumWithCoins = 0; //the value is the sum, it is not boolean!
    private int _constructSumWithCoinsAndBills = 0; //the value is the sum, it is not boolean!

    //שטרות
    private Dictionary<int, int> _recognizeBills = new() { { 20, 0 }, { 50, 0 }, { 100, 0 }, { 200, 0 } };
    private Dictionary<int, int> _matchBillToValue = new() { { 20, 0 }, { 50, 0 }, { 100, 0 }, { 200, 0 } };

    //השוואת מחירים
    private int _comparingOwnSumToProductPrice = -1; //boolean!
    private int _distinguishCheapExpensive = -1; //boolean!
    private int _chooseProductMatchingToOwnMoney = -1; //boolean!
    //עודף
    private int _recognizeChangeGet = -1; // boolean
    private int _recognizeNoChange = -1; //boolean
    private int _calcChange = 0;  //values,not boolean

   
    private string _studentCode; //[SerializeField]
    [SerializeField] private ObjectListSO _listSO;
    private Dictionary<string, string> _listOfPurchasesTemp = new();
    private Dictionary<string, string> _listOfPurchases = new();
    private Dictionary<string,int> _studentParams = new();
    //List<Dictionary<string, int>> studentSessionsInfo = new List<Dictionary<string, int>>();
    private Dictionary<string, int> _lastSessionTime = new();
    private DatabaseReference dbRef;
    private bool isNewStudent = false;

    

    public void SetRecognizeCoins(int key)
    {
        foreach (KeyValuePair<int, int> item in _recognizeCoins.ToList())
        {
            if (item.Key == key)
                _recognizeCoins[key] = 1;
            else if(item.Value != 1)
                _recognizeCoins[item.Key] = 0;
        }
    }

    public void SetRecognizeBills(int key)
    {
        foreach (KeyValuePair<int, int> item in _recognizeBills.ToList())
        {
            if (item.Key == key)
                _recognizeBills[key] = 1;
            else if (item.Value != 1)
                _recognizeBills[item.Key] = 0;
        }
    }

    public void SetMatchCoinToValue(int key)
    {
        foreach (KeyValuePair<int, int> item in _matchCoinToValue.ToList())
        {
            if (item.Key == key)
                _matchCoinToValue[key] = 1;
            else if (item.Value != 1)
                _matchCoinToValue[item.Key] = 0;
        }
    }

    public void SetMatchBillToValue(int key)
    {
        foreach (KeyValuePair<int, int> item in _matchBillToValue.ToList())
        {
            if (item.Key == key)
                _matchBillToValue[key] = 1;
            else if (item.Value != 1)
                _matchBillToValue[item.Key] = 0;
        }
    }

    public void SetchooseProductMatchingToOwnMoney(ToggleController toggle) {
        if(_chooseProductMatchingToOwnMoney == -1 && toggle.GetToggleState())
            _chooseProductMatchingToOwnMoney = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;

    }

    public void SetRecognizeSmallestOrBiggestNumber(ToggleController toggle) {
        if(_recognizeSmallestOrBiggestNumber == -1 && toggle.GetToggleState())
            _recognizeSmallestOrBiggestNumber = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;
        
        //toggle.GetComponent<Toggle>().isOn = _recognizeSmallestOrBiggestNumber == 1 ? true : false;

    }

    public void SetComparingOwnSumToProductPrice(ToggleController toggle) {
        if(_comparingOwnSumToProductPrice == -1 && toggle.GetToggleState())
            _comparingOwnSumToProductPrice = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;
    }

    public void SetDistinguishCheapExpensive(ToggleController toggle) {
        if(_distinguishCheapExpensive == -1 && toggle.GetToggleState())
            _distinguishCheapExpensive = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;
    }
    public void SetRecognizeChangeGet(ToggleController toggle) {
        if(_recognizeChangeGet == -1 && toggle.GetToggleState())
            _recognizeChangeGet = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;
    }

    public void SetRecognizeNoChange(ToggleController toggle) {
        if(_recognizeNoChange == -1 && toggle.GetToggleState())
            _recognizeNoChange = toggle.GetComponentInChildren<Text>().text == "ןכ" ? 1 : 0;
    }

    
    public int RemainAmount
    {
        get => _remainAmount;
        set => _remainAmount = value;
    }
    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    public int IdentifyingCoinsBiils
    {
        get => _identifyingCoinsBills;
        set => _identifyingCoinsBills = value;
    }

    public Dictionary<string, string> ListOfPurchasesTemp
    {
        get => _listOfPurchasesTemp;
        set => _listOfPurchasesTemp = value;
    }

    public Dictionary<string, string> ListOfPurchases
    {
        get => _listOfPurchases;
        set => _listOfPurchases = value;
    }

    public Dictionary<string,int> StudentParams {
        get => _studentParams;
        set => _studentParams = value;
    }

    public Dictionary<string,int> LastSessionTime
    {
        get => _lastSessionTime;
        set => _lastSessionTime = value;
    }

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
       

    }

    public int GetTotalSum()
    {
        int result = 0;
        foreach(KeyValuePair<string, string> item in ListOfPurchases)
        {
            int temp = int.Parse(item.Value) * findPrice(item.Key);
            result += temp;
        }
        return result;
    }

    private int findPrice(string name)
    {
        foreach(ObjectSO item in _listSO.objects)
        {
            if (item.objectName == name)
                return item.priceForItem;
        }
        return -1;
    }

    public void SetIsNewStudent(bool val) { isNewStudent = val; }
    public bool GetIsNewStudent() { return isNewStudent; }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StudentParams = new Dictionary<string, int>();
        
    }

    

public string GetStudentFirstName() {return this._firstName;}

    public string GetStudentLastName() { return this._lastName;}
    
    public int GetIdentifyingCoinsBills() { return this._identifyingCoinsBills;}
    public void SetIdentifyingCoinsBills(int num) { this._identifyingCoinsBills = num; }
    
    public int GetUnderstandingValue() { return this._understandingValue;}
    public void SetUnderstandingValue(int num) { this._understandingValue = num; }

    public int GetCheapExpensive() { return this._cheapExpensive; }
    public void SetCheapExpensive(int num) { this._cheapExpensive = num; }

    public int GetChange() { return this._changeCalc; }
    public void SetChange(int num) {this._changeCalc = num; }
    public string GetStudentCode() { return this._studentCode; }
    public void SetStudentCode(string code) { this._studentCode = code; }

    public string GetFirstName() { return this._firstName; }
    public void SetStudentFirstName(string firstName) { this._firstName = firstName; }

    public string GetLastName() { return this._lastName; }
    public void SetStudentLastName(string lastName) { this._lastName = lastName; }

    public string GetGroupName() { return this._groupName; }
    public void SetStudentGroupName(string groupName) { this._groupName = groupName; }


    public async Task<bool> getCurrentDataForQuestionaire() {
        await FirebaseDatabase.DefaultInstance.GetReference($"StudentQuestionnaire/{_studentCode}").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted) {
                DataSnapshot snapShot = task.Result;
                if (snapShot != null)
                {
                    foreach (DataSnapshot child in snapShot.Children)
                    {
                        if (child.Key != "date") {
                            if (child.Key.Contains('1') || child.Key.Contains('2') || child.Key.Contains('5'))
                            {
                                string value = new(child.Key.SkipWhile((ch) => !Char.IsNumber(ch)).TakeWhile((ch) => Char.IsNumber(ch)).ToArray());
                                string category = child.Key.Substring(0, child.Key.Length - value.Length);
                                ToggleEvents.setToggle(category, int.Parse(value), int.Parse(child.Value.ToString()));
                            }
                            else
                            {
                                ToggleEvents.setToggle(child.Key, int.Parse(child.Value.ToString()), int.Parse(child.Value.ToString()));
                            }
                        }
                        
                    }
                }
            } 
        });
        return true;
    }
    public async Task<bool> HandleApplicationQuit()
    {
        Debug.Log("Application stopped");
        //currentDate.ToOffset(TimeSpan.FromHours(3));
        /*LastSessionTime["Day"] = currentDate.Day;
        LastSessionTime["Month"] = currentDate.Month;
        LastSessionTime["Year"] = currentDate.Year;
        LastSessionTime["Hour"] = currentDate.Hour;
        LastSessionTime["Minute"] = currentDate.Minute;
        LastSessionTime["Second"] = currentDate.Second;
        LastSessionTime["Milliseconds"] = currentDate.Millisecond;*/
       
        
        if (StudentParams.Count != 0)
        {
            //await dbRef.Child("StudentQuestionnaire").Child(_studentCode).SetValueAsync(StudentParams);
            //await FirebaseDatabase.DefaultInstance.GetReference("StudentQuestionnaire").Child($"{_studentCode}").GetValueAsync().ContinueWithOnMainThread(async task =>
            //{
                
             Dictionary<string, dynamic> sessionData = new Dictionary<string, dynamic>();
            // DataSnapshot snapshot = task.Result;
            Dictionary<string, Dictionary<int,int>> categoryToDict = new() { { "_matchCoinToValue", _matchCoinToValue }, { "_matchBillToValue", _matchBillToValue }, { "_recognizeCoins", _recognizeCoins }, { "_recognizeBills", _recognizeBills} };
            Dictionary<string, int> categoryToValue = new() { { "_recognizeSmallestOrBiggestNumber", _recognizeSmallestOrBiggestNumber }, { "_comparingOwnSumToProductPrice", _comparingOwnSumToProductPrice }, { "_distinguishCheapExpensive", _distinguishCheapExpensive }, { "_chooseProductMatchingToOwnMoney", _chooseProductMatchingToOwnMoney }, { "_recognizeChangeGet", _recognizeChangeGet }, { "_recognizeNoChange", _recognizeNoChange } };
                    foreach (string category in categories)//KeyValuePair<string, int> item in StudentParams
                    { //loop through the categories, if it is one of 4: matchBillsTovalue,matchCoinsToValues,recognizeBills,recognizeCoins
                      // else we put in key: category name , value: StudentParams[category name]
                        if (category == "_matchCoinToValue" || category == "_matchBillToValue" || category == "_recognizeCoins" || category == "_recognizeBills")
                        {
                            foreach (KeyValuePair<int, int> pair in categoryToDict[category])
                            {
                                sessionData.Add(category + pair.Key.ToString(), pair.Value);
                            }
                        }
                        
                        else if (category == "_recognizeSmallestOrBiggestNumber" || category == "_comparingOwnSumToProductPrice" || category == "_distinguishCheapExpensive" || category == "_chooseProductMatchingToOwnMoney" || category == "_recognizeChangeGet" || category == "_recognizeNoChange") {
                            sessionData.Add(category, categoryToValue[category]);
                        }
                        else
                        {
                            if (!StudentParams.ContainsKey(category))
                            {
                                sessionData.Add(category, 0);
                            }
                            else
                            {
                                sessionData.Add(category, StudentParams[category]);
                            }

                        }
                    }
                    string currentDate = DateTimeOffset.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture).Replace("+00:00", "Z");

                    sessionData.Add("date", currentDate);
                    await dbRef.Child("StudentQuestionnaire").Child(_studentCode).SetValueAsync(sessionData);
                    foreach (KeyValuePair<string, int> item in LastSessionTime)
                    {
                        sessionData.Add(item.Key, item.Value);
                    }
                    //string sessionDataKey = dbRef.Child("Sessions").Child(_studentCode).Child("Info").Push().Key;
                    //await dbRef.Child("Sessions").Child(_studentCode).Child("Info").Child(sessionDataKey).SetValueAsync(sessionData);
                    /* foreach (DataSnapshot data in snapshot.Children)
                     {
                         if (!StudentParams.Keys.Contains(data.Key))
                         {
                             sessionData.Add(data.Key, int.Parse(data.Value.ToString()));
                         }
                     }*/

            // }
            //});
        }
        
        return true;
       
    }

    public async void OnApplicationQuit()
    {
        bool response = await HandleApplicationQuit();
        if (response)
            Application.Quit();
    }
}
