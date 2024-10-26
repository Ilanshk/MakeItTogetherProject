using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Composites;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    [SerializeField] private GameObject _calculator;
    [SerializeField] private Texture2D cursor;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        if (button.name == "MoveButton")
        {
            button.onClick.AddListener(GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().MoveToNextLevel);
        }
        else if (button.name == "RecognizeCoins")
        {
            Debug.Log("RecognizeCoins");
            button.onClick.AddListener(async () =>
                await GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().MakeLevelSequence(1));
        }
        else if (button.name == "UnderstandingNumberValue")
        {
            Debug.Log("UnderstandingNumberValue");
            button.onClick.AddListener(async () => await GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().MakeLevelSequence(2));
        }
        else if (button.name == "Cheap/Expensive")
        {
            Debug.Log("Cheap/Expensive");
            button.onClick.AddListener(async () => await GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().MakeLevelSequence(3));
        }
        else if (button.name == "CalcExcess")
        {
            Debug.Log("CalcExcess");
            button.onClick.AddListener(async () => await GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().MakeLevelSequence(4));
        }
        else if (button.name == "ResetButton")
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            button.onClick.AddListener(() => Reset());
        }
        else if (button.name == "BackToMenu")
        {
            button.onClick.AddListener(() => GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().GoBackToMenu());
        }
        else if (button.name == "YesBtn")
        {
            button.onClick.AddListener(() => GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(new PointerEventData(EventSystem.current), null, null, null));
        }
        else if (button.name == "NoBtn")
        {
            button.onClick.AddListener(() => GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(new PointerEventData(EventSystem.current), null, null, null));
        }
        /*else if (button.name == "SubmitButton")
        {

            button.onClick.AddListener(async () =>
            {
                string studentCode = GameObject.Find("StudentCodeInput").GetComponent<TMP_InputField>().text;
                string studentName = GameObject.Find("StudentNameInput").GetComponent<TMP_InputField>().text;

                if (studentName.Split().Length == 3)
                {
                    string firstName = studentName.Split(" ")[0];

                    string lastName = studentName.Split(" ")[1];
                    string groupName = studentName.Split(" ")[2];
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetStudentCode(studentCode);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetStudentFirstName(firstName);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetStudentGroupName(groupName);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetStudentLastName(lastName);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetIsNewStudent(true);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetUnderstandingValue(0);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetIdentifyingCoinsBills(0);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetChange(0);
                    GameObject.Find("StudentData").GetComponent<StudentScript>().SetCheapExpensive(0);
                    Game_Manager gm = GameObject.Find("GameManager").GetComponent<Game_Manager>();
                    //await gm.PresentStudentData();
                    //GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 28;
                    GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 29;

                }
                else
                {
                    GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "  הכנס שם מלא של התלמיד והקבוצה אליה משתייך";

                }
                //GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 26;

            });
        }*/
        else if (button.name == "ContinueToMenuBtn")
        {
            button.onClick.AddListener(() =>
            {
                StudentScript student = GameObject.Find("StudentData").GetComponent<StudentScript>();
                if (student.GetChange() != 0 && student.GetCheapExpensive() != 0 && student.GetIdentifyingCoinsBills() != 0 && student.GetUnderstandingValue() != 0)
                {
                    GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = 27;
                    GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 0;
                    GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 0;
                }
                else
                {
                    GameObject.Find("NewStudentAdded").GetComponent<Canvas>().sortingOrder = 30;
                    GameObject.Find("MessageStudentAdded").GetComponent<TextMeshProUGUI>().text = "נא לבדוק את בחירת הפרמטרים לפני שתמשיכו";
                    GameObject.Find("MessageToTeacher").GetComponent<TextMeshProUGUI>().color = Color.red;
                }


                //GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().LoadExercisesForStudent(studentLearningLevel); 
            });
        }
        else if (button.name == "ContinueToSimulation")
        {
            button.onClick.AddListener(() => {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                SceneManager.LoadSceneAsync("BeforeInit"); 
            });

        }
        else if (button.name == "BackToStartFromMainMenu")
        {
            button.onClick.AddListener(() =>
            {
                GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 28;
                GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 29;
            });

        }
        else if (button.name == "CalculatorButton")
        {
            button.onClick.AddListener(() =>
            {
                _calculator.SetActive(true);//!_calculator.activeInHierarchy
                _calculator.GetComponent<Canvas>().sortingOrder = 7;
            });
        }
        else if (button.name == "Close_Calculator")
        {
            button.onClick.AddListener(() =>
            {
                _calculator.SetActive(false);
                _calculator.GetComponent<Canvas>().sortingOrder = 0;

            });
        }
        else if (button.name == "ShowProducts")
        {
            List<string> btnTexts = new() { "צפה בפריטים", "הסתר פריטים" };
            int currentTextIndex = 0;
            button.onClick.AddListener(() =>
            {
                currentTextIndex = 1 - currentTextIndex;
                button.GetComponentInChildren<TextMeshProUGUI>().text = btnTexts[currentTextIndex];
                GameObject productDes = GameObject.Find("ProductDescription");
                foreach (Transform item in productDes.transform)
                {
                    Color color = item.GetComponentInChildren<Image>().color;
                    color.a = Mathf.Abs(item.GetComponentInChildren<Image>().color.a - 1);
                    item.GetChild(0).GetComponent<Image>().color = color;
                    item.GetComponent<Image>().color = color;
                    item.GetComponentInChildren<TextMeshProUGUI>().enabled = !item.GetComponentInChildren<TextMeshProUGUI>().enabled;
                }

            });
        }
        else if (button.name == "ContinueToQuestionaire")
        {
            button.onClick.AddListener(async () =>
            {
                StudentScript st = GameObject.Find("StudentData").GetComponent<StudentScript>();
                if (st.GetIsNewStudent())
                {
                    //await st.getCurrentDataForQuestionaire();
                    st.SetIsNewStudent(false);
                }
                await st.getCurrentDataForQuestionaire();
                GameObject.Find("StudentIndicator").GetComponent<Canvas>().sortingOrder = 30;

            });

        }
        else if (button.name == "BackFromQuestionnaire")
        {
            button.onClick.AddListener(
                () =>
                {
                    GameObject.Find("StudentIndicator").GetComponent<Canvas>().sortingOrder = 0;
                    GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 28;
                    GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 29;
                }
                );

        }
        else if (button.name == "OKButton") {
            //New student added
            button.onClick.AddListener(() =>
            {
                GameObject.Find("NewStudentAdded").GetComponent<Canvas>().sortingOrder = 0;

            });
        
        }

    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (button.interactable)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }
    
    }
    public void OnPointerExit(PointerEventData eventData) {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    
    }
    
   

    private void Reset()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().enabled = false;
        GameObject.Find("MarkForAnswer").GetComponent<Image>().sprite = null;
        this.button.interactable = false;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
}
