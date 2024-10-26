using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Reflection;
using System.Threading.Tasks;

public class Game_Manager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Slider slider;
    private MakeLevel makeLevel;
    private StudentScript student = null;
    private string currId = null;
    private ItemSlot itemSlot = null;
    private Drag itemDrag = null;
    private Sprite mark = null;
    bool switcher = false;
    private DatabaseReference dbRef;
    private TMP_Dropdown ddIdentify;
    private TMP_Dropdown ddUnderstandingValue;
    private TMP_Dropdown ddCheapExpensive;
    private TMP_Dropdown ddChange;
    private string currIdentifying;
    private string currUnderstandingValues;
    private string currCheapExpensive;
    private string currChange;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void Start()
    {
        student = GameObject.Find("StudentData").GetComponent<StudentScript>();
        makeLevel = GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>();
        message.GetComponent<TextMeshProUGUI>().enabled = false;
        slider.value = 0;
        GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = 20;
        GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
        GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
        GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
        ddIdentify = GameObject.Find("DropdownIdentifyCB").GetComponent<TMP_Dropdown>();
        ddIdentify.onValueChanged.AddListener(delegate { updateStudentDataInDB(); });
        ddUnderstandingValue = GameObject.Find("DropdownUnderstandingNumValue").GetComponent<TMP_Dropdown>();
        ddUnderstandingValue.onValueChanged.AddListener(delegate { updateStudentDataInDB(); });
        ddCheapExpensive = GameObject.Find("DropdownCheapExpensive").GetComponent<TMP_Dropdown>();
        ddCheapExpensive.onValueChanged.AddListener(delegate { updateStudentDataInDB(); });
        ddChange = GameObject.Find("DropdownChange").GetComponent<TMP_Dropdown>();
        ddChange.onValueChanged.AddListener(delegate { updateStudentDataInDB(); });
        
        if (student.GetStudentCode() != null)
        {
            GameObject.Find("StudentNameInput").GetComponent<TMP_InputField>().text = student.GetFirstName() + " " + student.GetLastName();
            GameObject.Find("StudentCodeInput").GetComponent<TMP_InputField>().text = student.GetStudentCode();
            GameObject.Find("DropdownIdentifyCB").GetComponentInChildren<TextMeshProUGUI>().text = student.GetIdentifyingCoinsBills().ToString();
            GameObject.Find("DropdownUnderstandingNumValue").GetComponentInChildren<TextMeshProUGUI>().text = student.GetUnderstandingValue().ToString();
            GameObject.Find("DropdownCheapExpensive").GetComponentInChildren<TextMeshProUGUI>().text = student.GetCheapExpensive().ToString();
            GameObject.Find("DropdownChange").GetComponentInChildren<TextMeshProUGUI>().text = student.GetChange().ToString();
            GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 27;
            foreach (TMP_Dropdown.OptionData value in ddIdentify.options)
            {
                if (value.text == student.GetIdentifyingCoinsBills().ToString())
                {
                    ddIdentify.value = ddIdentify.options.IndexOf(value);
                    break;
                }
            }
            foreach (TMP_Dropdown.OptionData value in ddUnderstandingValue.options)
            {
                if (value.text == student.GetUnderstandingValue().ToString())
                {
                    ddUnderstandingValue.value = ddUnderstandingValue.options.IndexOf(value);
                    break;
                }
            }
            foreach (TMP_Dropdown.OptionData value in ddCheapExpensive.options)
            {
                if (value.text == student.GetCheapExpensive().ToString())
                {
                    ddCheapExpensive.value = ddCheapExpensive.options.IndexOf(value);
                    break;
                }
            }
            foreach (TMP_Dropdown.OptionData value in ddChange.options)
            {
                if (value.text == student.GetChange().ToString())
                {
                    ddChange.value = ddChange.options.IndexOf(value);
                    break;
                }
            }
        }
    }
    public void CheckCondition()
    {
        if (slider.value == slider.maxValue)
        {
            GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().text = "תשובה נכונה";
            GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().color = Color.green;
            message.enabled = true;
            GameObject.Find("MoveButton").GetComponent<Button>().interactable = true;
            GameObject.Find("MoveButton").GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            mark = Resources.Load<Sprite>("V mark");
            GameObject.Find("MarkForAnswer").GetComponent<Image>().sprite = mark;
            GameObject.Find("ResetButton").GetComponent<Button>().interactable = false;
            GameObject.Find("ResetButton").GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
        else if (switcher == true)
        {
            GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().text = "נסו שוב";
            GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().color = Color.red;
            mark = Resources.Load<Sprite>("X mark");
            GameObject.Find("MarkForAnswer").GetComponent<Image>().sprite = mark;
            switcher = false;
            message.enabled = true;
            GameObject.Find("ResetButton").GetComponent<Button>().interactable = true;
            GameObject.Find("ResetButton").GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            GameObject.Find("StudentAnswer").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("StudentWrittenAnswer").GetComponent<TMP_InputField>().text = "";
        }
    }

    public void GameManagerCheck(PointerEventData eventData, ItemSlot iSlot, Drag iDrag, GameObject line)
    {
        if (iSlot != null)
            itemSlot = iSlot;
        if (iDrag != null)
            itemDrag = iDrag;
        if (GameObject.Find("ExerciseCanva") != null && GameObject.Find("ExerciseCanva").activeInHierarchy == true)
        {
            if (makeLevel.GetLevelName() == "1TaskCM" || makeLevel.GetLevelName() == "8TaskCM")
            {
                foreach (int i in makeLevel.GetRightSumsForLevel()) { Debug.Log(i); }
                if (makeLevel.GetRightSumsForLevel().Contains(eventData.pointerEnter.GetComponent<Drag>().value))
                {
                    if (eventData.pointerEnter.GetComponent<Drag>().GetSwitcher() == false)
                    {
                        eventData.pointerEnter.GetComponent<Image>().color = Color.green;
                        slider.value += 1;
                        eventData.pointerEnter.GetComponent<Drag>().SetSwitcher(true);
                        Debug.Log("Coin is collected");
                    }
                }
                else
                    switcher = true;
            }
            else if (makeLevel.GetLevelName() == "2TaskCM" || makeLevel.GetLevelName() == "7TaskCM")
            {
                if (eventData.pointerDrag != null && itemDrag != null)
                {
                    int val = eventData.pointerDrag.GetComponent<Drag>().value;
                    Debug.Log("Coin is collected");
                    if ((slider.value + val <= slider.maxValue && makeLevel.GetLevelName() == "2TaskCM") || (slider.value + val <= slider.maxValue && slider.maxValue == 1 && makeLevel.GetLevelName() == "7TaskCM")
                        || slider.value + val <= slider.maxValue && val != slider.maxValue && makeLevel.GetLevelName() == "7TaskCM")
                            slider.value += val;
                    else
                        switcher = true;
                }
                if (eventData.pointerDrag != null)
                    eventData.pointerDrag.transform.SetParent(GameObject.Find("LeftGrid").transform, true);
            }
            else if (makeLevel.GetLevelName() == "3TaskCM" || makeLevel.GetLevelName() == "5TaskCM" || makeLevel.GetLevelName() == "CTask")
            {
                if (itemDrag != null)
                {
                    if (eventData.pointerDrag != null && itemDrag.value == eventData.pointerDrag.GetComponent<Drag>().value)
                    {
                        Debug.Log("Coin is collected");
                        Destroy(eventData.pointerDrag);
                        slider.value += 1;
                    }
                    else
                    {
                        switcher = true;
                    }
                }

                if (eventData.pointerDrag != null)
                    eventData.pointerDrag.transform.SetParent(GameObject.Find("LeftGrid").transform, true);
            }
            else if (makeLevel.GetLevelName() == "10TaskCM") {
                //check if the dragged object isn't null and its value(Drag Script) is the closest minimum to the object from the right
                if (eventData.pointerDrag != null) {
                    if (makeLevel.GetRightSumsForLevel().Contains(eventData.pointerDrag.GetComponent<Drag>().value)) {
                        slider.value += 1;
                        Destroy(eventData.pointerDrag);
                    }
                    else {
                        switcher = true;
                    }
                }
                //which is in slotItems list
                //add the following if the condition to correct answer is met:
                /*
                     Destroy(eventData.pointerDrag);
                        slider.value += 1;
                    
                    else(condition not met)
                    {
                        switcher = true;
                    }
                 
                 
                 */

            }
            else if (makeLevel.GetLevelName() == "FTask")
            {
                if (eventData.pointerDrag != null && itemDrag != null)
                {
                    if (slider.value + eventData.pointerDrag.GetComponent<Drag>().value <= slider.maxValue)
                    {
                        Debug.Log(itemDrag.ToString());
                        GameObject temp = Instantiate(eventData.pointerDrag, GameObject.Find("GridForCoins").transform);
                        temp.GetComponent<Drag>().interactable = false;
                        slider.value += eventData.pointerDrag.GetComponent<Drag>().value;
                    }
                    else
                        switcher = true;

                }
                eventData.pointerDrag.transform.SetParent(makeLevel.horLayGroup.transform, true);
            }
            else if (makeLevel.GetLevelName() == "ETask")
            {
                if (eventData != null)
                {
                    GameObject temp = eventData.selectedObject;
                    string answer = GameObject.Find("StudentAnswer").GetComponent<TMP_InputField>().text;
                    if (makeLevel.GetRightSumsForLevel()[0] < GameObject.Find("ProductDisplay").GetComponentInChildren<Drag>().value && temp.name == "NoBtn" ||
                       makeLevel.GetRightSumsForLevel()[0] >= GameObject.Find("ProductDisplay").GetComponentInChildren<Drag>().value && temp.name == "YesBtn"
                       )
                    {
                        slider.value++;
                        Debug.Log(slider.value);
                        GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
                    }
                    else
                        switcher = true;



                }


            }
            else if (makeLevel.GetLevelName().Substring(0, 12) == "WordProblems")
            {
                if (eventData.pointerDrag != null && iSlot != null)
                {

                    if (makeLevel.GetRightSumsForLevel().Contains(eventData.pointerDrag.GetComponent<Drag>().value))
                    {
                        slider.value++;
                        Instantiate(eventData.pointerDrag, GameObject.Find("AnswerBox").transform);
                    }
                    else
                    {
                        switcher = true;
                    }

                }

            }
            else if (makeLevel.GetLevelName().Substring(0, 4) == "Comp" || makeLevel.GetLevelName().Substring(0, 5) == "Coins")
            {
                if (eventData.pointerEnter != null)
                {

                    if (makeLevel.GetRightSumsForLevel().Contains(eventData.pointerEnter.GetComponentInParent<Drag>().value))//GetComponent<Drag>().value
                    {
                        slider.value++;

                    }
                    else
                    {
                        switcher = true;
                    }

                }

            }
            else if (makeLevel.GetLevelName() == "MatchingCoins")
            {
                if (iDrag.value == eventData.pointerDrag.GetComponent<Drag>().value && eventData.pointerDrag.GetComponent<Drag>() != iDrag)
                {
                    eventData.pointerDrag.GetComponent<Drag>().UpdateLine(eventData.pointerEnter.transform.position);
                    slider.value++;
                    Instantiate(eventData.pointerDrag.GetComponent<Drag>().GetLine(), GameObject.FindWithTag("Task1Workplace").transform.parent);
                    iDrag.GetComponent<Image>().raycastTarget = false;
                    Color oldColor = iDrag.GetComponent<Image>().color;
                    oldColor.a /= 2;
                    iDrag.GetComponent<Image>().color = oldColor;
                    eventData.pointerDrag.GetComponent<Image>().raycastTarget = false;
                    oldColor = eventData.pointerDrag.GetComponent<Image>().color;
                    oldColor.a /= 2;
                    eventData.pointerDrag.GetComponent<Image>().color = oldColor;
                    /*Destroy(eventData.pointerDrag);
                    Destroy(this);*/
                }
                else
                {
                    if (eventData.pointerDrag.GetComponent<Drag>().GetLine() != null)
                    {
                        Destroy(eventData.pointerDrag.GetComponent<Drag>().GetLine());
                        switcher = true;
                    }
                }

            }
            GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
        }

        CheckCondition();
        itemSlot = null;
        itemDrag = null;
    }

    public void AddPoint()
    {
        slider.value += 1;
        CheckCondition();
    }

    public void CheckWrittenAnswer() {
        string answer = GameObject.Find("StudentWrittenAnswer").GetComponent<TMP_InputField>().text;
        if (makeLevel.GetCorrectAnswersForlevel().Contains(GameObject.Find("StudentWrittenAnswer").GetComponentInChildren<TMP_InputField>().text))
        {
            slider.value += 1;
            GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
            CheckCondition();
        }
        else
        {
            switcher = true;
            CheckCondition();
        }
    }
    public void CheckYesNoQuestionAns()
    {
        string answer = GameObject.Find("StudentAnswer").GetComponent<TMP_InputField>().text;
        if (makeLevel.GetRightSumsForLevel()[0] < GameObject.Find("ProductDisplay").GetComponentInChildren<Drag>().value && answer == "לא" ||
           makeLevel.GetRightSumsForLevel()[0] >= GameObject.Find("ProductDisplay").GetComponentInChildren<Drag>().value && answer == "כן")
        {
            slider.value++;
            GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
            CheckCondition();
        }
        else
        {
            switcher = true;
            CheckCondition();
        }
    }

    public async Task PresentStudentData() {

        if (student.GetStudentCode() != null && student.GetGroupName() != null)
        {
            currId = student.GetStudentCode();
            

            /*if (student.GetIsNewStudent())
            {
                GameObject message = GameObject.Find("NewStudentAdded");
                Component[] compList = message.GetComponents(System.Type.GetType("Canvas"));
            }*/
            GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 26;
            await FirebaseDatabase.DefaultInstance.GetReference($"ComputerParams/{currId}").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        if (data.Key == "_changeCalc")
                        {

                            foreach (TMP_Dropdown.OptionData value in ddChange.options)
                            {
                                if (value.text == data.Value.ToString())
                                {
                                    ddChange.value = ddChange.options.IndexOf(value);
                                    student.SetChange(int.Parse(value.text));
                                    GameObject.Find("DropdownChange").GetComponentInChildren<TextMeshProUGUI>().text = data.Value.ToString();
                                    break;
                                }
                            }
                        }
                        else if (data.Key == "_cheapExpensive")
                        {

                            foreach (TMP_Dropdown.OptionData value in ddCheapExpensive.options)
                            {
                                if (value.text == data.Value.ToString())
                                {
                                    ddCheapExpensive.value = ddCheapExpensive.options.IndexOf(value);
                                    student.SetCheapExpensive(int.Parse(value.text));
                                    GameObject.Find("DropdownCheapExpensive").GetComponentInChildren<TextMeshProUGUI>().text = data.Value.ToString();
                                    break;
                                }
                            }
                        }
                        else if (data.Key == "_identifyingCoinsBills")
                        {
                            foreach (TMP_Dropdown.OptionData value in ddIdentify.options)
                            {
                                if (value.text == data.Value.ToString())
                                {
                                    ddIdentify.value = ddIdentify.options.IndexOf(value);
                                    student.SetIdentifyingCoinsBills(int.Parse(value.text));
                                    GameObject.Find("DropdownIdentifyCB").GetComponentInChildren<TextMeshProUGUI>().text = data.Value.ToString();
                                    break;
                                }
                            }
                        }
                        else if (data.Key == "_understandingValue")
                        {

                            foreach (TMP_Dropdown.OptionData value in ddUnderstandingValue.options)
                            {
                                if (value.text == data.Value.ToString())
                                {
                                    ddUnderstandingValue.value = ddUnderstandingValue.options.IndexOf(value);
                                    student.SetUnderstandingValue(int.Parse(value.text));
                                    GameObject.Find("DropdownUnderstandingNumValue").GetComponentInChildren<TextMeshProUGUI>().text = data.Value.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
            });
        }
        else
        {
            //GameObject.Find("MessageStudentAdded").GetComponent<TextMeshProUGUI>().text = "שם הקבוצה או הקוד תלמיד חסרים. נא מלאו את החסר לפני שתמשיכו";
            //GameObject.Find("NewStudentAdded").GetComponent<Canvas>().sortingOrder = 31;
        }
        
    }
           

    public void updateStudentDataInDB() {

        FirebaseDatabase.DefaultInstance.GetReference($"ComputerParams/{currId}").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null) {
                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        if (data.Key == "_changeCalc")
                        {
                            currChange = ddChange.options[ddChange.value].text;
                            if (currChange != data.Value.ToString()) {
                                dbRef.Child("ComputerParams").Child(currId).Child("_changeCalc").SetValueAsync(int.Parse(currChange));
                                student.SetChange(int.Parse(currChange));
                                GameObject.Find("DropdownChange").GetComponent<TextMeshProUGUI>().text = currChange;
                            }
                        }
                        else if (data.Key == "_cheapExpensive")
                        {
                            currCheapExpensive = ddCheapExpensive.options[ddCheapExpensive.value].text;
                            if (currCheapExpensive != data.Value.ToString()) {
                                dbRef.Child("ComputerParams").Child(currId).Child("_cheapExpensive").SetValueAsync(int.Parse(currCheapExpensive));
                                student.SetCheapExpensive(int.Parse(currCheapExpensive));
                                ddCheapExpensive.options[ddCheapExpensive.value].text = currCheapExpensive;
                            }
                        }
                        else if (data.Key == "_identifyingCoinsBills")
                        {
                            currIdentifying = ddIdentify.options[ddIdentify.value].text;
                            if (currIdentifying != data.Value.ToString()) {
                                dbRef.Child("ComputerParams").Child(currId).Child("_identifyingCoinsBills").SetValueAsync(int.Parse(currIdentifying));
                                student.SetIdentifyingCoinsBills(int.Parse(currIdentifying));
                                ddIdentify.options[ddIdentify.value].text = currIdentifying;
                            }
                        }
                        else if (data.Key == "_understandingValue")
                        {
                            currUnderstandingValues = ddUnderstandingValue.options[ddUnderstandingValue.value].text;
                            if (currUnderstandingValues != data.Value.ToString()) {
                                dbRef.Child("ComputerParams").Child(currId).Child("_understandingValue").SetValueAsync(int.Parse(currUnderstandingValues));
                                student.SetUnderstandingValue(int.Parse(currUnderstandingValues));
                                ddUnderstandingValue.options[ddUnderstandingValue.value].text = currUnderstandingValues;
                            }
                        }

                    }
                }
            }
        });
    }


}
