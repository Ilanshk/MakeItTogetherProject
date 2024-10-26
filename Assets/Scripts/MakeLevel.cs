using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Linq;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI.TableUI;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
//using static System.Net.Mime.MediaTypeNames;

public class MakeLevel : MonoBehaviour {
    private List<string> levelNames = new() { };
    public List<List<GameObject>> dragItems = new() { };
    public List<List<GameObject>> itemSlots = new() { };
    public List<List<GameObject>> items = new() { };
    public List<int> maxValues = new() { };
    public List<List<string>> questions = new() { };
    public GridLayoutGroup leftGrid;
    public GridLayoutGroup rightGrid;
    public HorizontalLayoutGroup horLayGroup;
    public GameObject answerBox;
    public GameObject productsToDrag;
    private Slider slider;
    public List<List<int>> rightsum = new() { };
    private List<int> rightSumsForLevel = new() { };
    private List<List<string>> correctAnswers = new() { };
    private int index = -1;
    //private int index2 = 0;
    private int indexWordProblem = 1;
    //private GameObject line;
    public GameObject backBtn;
    private DatabaseReference dbRef;
    [SerializeField] private StudentScript currentStudent;
    List<List<string>> identifying_dragItems;
    List<List<string> >identifying_slotItems;
    List<List<string>> productsToPurchase;
    List<List<string>> coinsCollection;
    
    // Start is called before the first frame update

    private void Awake()
    {
        Debug.Log("Awake!");

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
    }
    void Start()
    {
        GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 25;
        GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 0;
        slider = GameObject.Find("progressBar").GetComponent<Slider>();
        backBtn = GameObject.Find("BackToMenu");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeNewLevel(string levelName)
    {
       
        slider.maxValue = maxValues[index];
        rightSumsForLevel = rightsum[index];
        changeCellSize(dragItems, leftGrid);
        changeCellSize(itemSlots, rightGrid);
        if (string.Equals(levelName, "2TaskCM") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "  גרור מטבעות משמאל עד שתגיע למחיר המוצר מימין";

            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
                Instantiate(itemSlots[index][i], rightGrid.transform);
        }
        else if (string.Equals(levelName, "7TaskCM"))
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = ".התבוננו במטבע בצד ימין. בצעו פריטה של המטבע בעזרת גרירת מטבעות מצד שמאל";
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
                Instantiate(itemSlots[index][i], rightGrid.transform);

        }
        else if (string.Equals(levelName, "1TaskCM") == true || string.Equals(levelName, "8TaskCM") == true)
        {
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
            {
                Instantiate(itemSlots[index][i], rightGrid.transform);

            }
            if (string.Equals(levelName, "1TaskCM") == true)
            {
                if (itemSlots[index][0].GetComponent<Drag>().value <= 10)
                {
                    GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "לחצו על מטבעות משמאל עם ערך זהה לערך המטבע מימין";
                }
                else if (itemSlots[index][0].GetComponent<Drag>().value >= 20) {
                    GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "לחצו על שטרות משמאל עם ערך זהה לערך השטר מימין";
                }
            }
            else if (string.Equals(levelName, "8TaskCM") == true)
                GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "לחצו על מטבעות בצד שמאל עם ערך זהה למופיע בימין";
        }
        else if (string.Equals(levelName, "3TaskCM") == true)
        {
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
                Instantiate(itemSlots[index][i], rightGrid.transform);
        }
        else if (string.Equals(levelName, "5TaskCM") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גרור מטבע משמאל לסכום מטבעות הזהה לו בימין";
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
                Instantiate(itemSlots[index][i], rightGrid.transform);
        }

        else if (string.Equals(levelName, "CTask") == true || string.Equals(levelName, "10TaskCM") == true)
        {
            if (string.Equals(levelName, "CTask") == true)
            {
                if (itemSlots[index][0].GetComponent<Drag>().value <= 10 )
                {
                    if (itemSlots[index][0].GetComponent<Drag>().IsCoin)
                    {
                        GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גררו מטבעות משמאל עם ערך זהה לערך המטבע מימין";
                    }
                    else if (!itemSlots[index][0].GetComponent<Drag>().IsCoin) {
                        GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גררו מטבעות משמאל שמתאימים למחיר מוצר בימין.ייתכן ויש יותר ממטבע אחד";
                    }
                }
                else if (itemSlots[index][0].GetComponent<Drag>().value >= 20)
                {
                    if (itemSlots[index][0].GetComponent<Drag>().IsCoin)
                    {
                        GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גררו שטרות משמאל עם ערך זהה לערך השטר מימין";
                    }
                    else if (!itemSlots[index][0].GetComponent<Drag>().IsCoin) {
                        GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = ".גררו שטרות משמאל שמתאימים למחיר מוצר בימין.ייתכן ויש יותר משטר אחד";
                    }
                }
            }
            else if (string.Equals(levelName, "10TaskCM") == true)
            {
                GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גרור מטבע עם ערך מינימלי משמאל, שאיתו אפשר יהיה לשלם עבור המוצר בצד ימין";
            }
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;

            }
            for (int i = 0; i < itemSlots[index].Count; i++)
            {
                GameObject temp = Instantiate(itemSlots[index][i], rightGrid.transform);
                //if (temp.GetComponent<Drag>().IsCoin == true)
                    //GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גרור מטבעות משמאל שזהים למטבע בימין";

            }
        }
        else if (string.Equals(levelName, "ETask") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "התבוננו במוצר משמאל וענו על השאלה הבאה";
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], GameObject.Find("ProductDisplay").transform);
                temp.GetComponent<Drag>().interactable = false;
            }
            GameObject.Find("QuestionDescription").GetComponent<TextMeshProUGUI>().text = questions[index][0];

        }
        else if (string.Equals(levelName, "FTask") == true)
        {
            //GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "גררו את המטבעות עבור "+"<link>"+"עודף"+"</link>";
            for (int i = 0; i < dragItems[index % levelNames.Count].Count; i++)
            {
                GameObject temp1 = Instantiate(dragItems[index % levelNames.Count][i], horLayGroup.transform);
                temp1.GetComponent<Drag>().interactable = true;
            }
            System.Random rand1 = new System.Random();
            int maxPrice = currentStudent.GetCheapExpensive();
            int howMuchDidIPay = 0;
            int productPrice = 0;
            productPrice = rand1.Next(1, maxPrice + 1);
            howMuchDidIPay = rand1.Next(productPrice + 1, productPrice + 1 + maxPrice + 1);

            TextMeshProUGUI cellForPrice = GameObject.Find("TableExcess").GetComponent<TableUI>().GetCell(1, 2);
            TextMeshProUGUI cellForHowMuchDidIPay = GameObject.Find("TableExcess").GetComponent<TableUI>().GetCell(1, 1);
            cellForPrice.text = productPrice.ToString() + "₪";
            cellForHowMuchDidIPay.text = howMuchDidIPay.ToString() + "₪";
            slider.maxValue = howMuchDidIPay - productPrice;

        }
        else if (string.Equals(levelName.Substring(0, 12), "WordProblems") == true) //else if (string.Equals(levelName.Substring(0, 12), "WordProblems") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "שאלות - הקלד או גרור";
            TableUI productsTable = GameObject.Find("TableOfProducts").GetComponent<TableUI>();
            GameObject productDes = GameObject.Find("ProductDescription");
            for (int row = 1; row < dragItems[index].Count + 1; row++)
            {
                GameObject rowProduct = dragItems[index][row - 1];
                GameObject item = productDes.transform.GetChild(row - 1).gameObject;
                if (item.activeInHierarchy == false)
                    item.SetActive(true);
                Image[] images = rowProduct.GetComponentsInChildren<Image>();
                foreach (Image image in images)
                {
                    if (image.name.Contains("Visuals"))
                    {
                        GameObject.Find("Image_p" + row.ToString()).GetComponent<Image>().sprite = image.sprite;


                    }
                }
                //item.transform.GetChild(0).GetComponent<Image>().sprite = rowProduct.GetComponentInChildren<Image>().sprite;
                item.GetComponentInChildren<TextMeshProUGUI>().text = rowProduct.GetComponent<Drag>().Name;
                item.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                
                productsTable.GetCell(row, 0).gameObject.GetComponent<TextMeshProUGUI>().text = rowProduct.GetComponent<Drag>().value.ToString() + "₪";
                Debug.Log(productsTable.GetCellObject(row, 1));
                Debug.Log(rowProduct);
                Image[] images2 = rowProduct.GetComponentsInChildren<Image>();
                foreach (Image image in images2) {
                    if (image.name.Contains("Visuals")) {
                        GameObject.Find("ImageProduct" + row.ToString()).GetComponentInChildren<Image>().sprite = image.sprite;


                    } 
                }
                //productsTable.GetCellObject(row, 1).GetComponent<Image>().sprite = Resources.Load<GameObject>($"Prefabs/{rowProduct.name}").GetComponent<Image>().sprite;
                //GameObject.Find("ImageProduct" + row.ToString()).GetComponentInChildren<Image>().sprite = rowProduct.GetComponentInChildren<Image>().sprite;//Resources.Load<GameObject>($"Prefabs/{rowProduct.name}").GetComponent<Image>().sprite;
                GameObject.Find("ImageProduct" + row.ToString()).GetComponentInChildren<TextMeshProUGUI>().text = rowProduct.GetComponent<Drag>().Name;
                /*productsTable.GetCell(row, 2).isRightToLeftText = true;
                productsTable.GetCell(row, 2).gameObject.GetComponent<TextMeshProUGUI>().text = rowProduct.GetComponent<Drag>().Name;*/
            }

            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], GameObject.Find("ProductsToDrag").transform);
                temp.GetComponent<Drag>().interactable = true;
            }

            if (levelName.Substring(12) == "1")
            {
                //GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
                GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().text = " התבונן במחירון משמאל. " + "\n" + "מה המוצר "+"<link><b>"+"היקר "+ "</b></link>" + "ביותר?";

            }
            else if (levelName.Substring(12) == "2")
            {
                //GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().isRightToLeftText=true;
                GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().text = " התבונן במחירון משמאל. " + "\n" + "מה המוצר " + "<link>" + "<b>" + "הזול " + "</b>" + "</link>" + "ביותר?";

            }
            else if (levelName.Substring(12) == "3")
            {

            }

        }
        else if (string.Equals(levelName.Substring(0, 4), "Comp") == true)
        {

            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 2;

            if (string.Equals(levelName.Substring(15, 6), "Lowest") == true)
            {
                Debug.Log("Lowest");
                //GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().isRightToLeftText = true;
                GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().text = "לחצו על המוצר" + "\n" + "<link>"+"<b>" + "הזול " + "</b>" + "</link>" + "ביותר";
            }
            else if (string.Equals(levelName.Substring(15, 7), "Highest") == true)
            {
                Debug.Log("Highest");
                //GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().isRightToLeftText = true;
                GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().text = "לחצו על המוצר"+ "\n" + "<link><b>" + "היקר "+ "</b></link>" + "ביותר";

            }

            TableUI pairTable = GameObject.Find("TableComp").GetComponent<TableUI>();
            pairTable.GetCellObject(0, 0).GetComponent<TextMeshProUGUI>().text = "מוצר שני";
            pairTable.GetCellObject(0, 1).GetComponent<TextMeshProUGUI>().text = "מוצר ראשון";
            GameObject tableCell1 = pairTable.GetCellObject(1, 0);
            GameObject tableCell2 = pairTable.GetCellObject(1, 1);
            tableCell1.transform.parent.transform.GetComponent<Canvas>().sortingOrder = 3;

            GameObject leftProduct = Resources.Load<GameObject>($"Prefabs/{dragItems[index][0].name}");
            GameObject rightProduct = Resources.Load<GameObject>($"Prefabs/{dragItems[index][1].name}");
            Instantiate(rightProduct, tableCell2.GetComponent<RectTransform>().parent);
            Instantiate(leftProduct, tableCell1.GetComponent<RectTransform>().parent);
            leftProduct.transform.SetSiblingIndex(2);
            rightProduct.transform.SetSiblingIndex(3);
            tableCell1.transform.SetSiblingIndex(0);
            tableCell2.transform.SetSiblingIndex(1);

        }
        else if (string.Equals(levelName.Substring(0, 5), "Coins") == true) {
            if (levelName[14] == 'L') {
                GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().text = "לחצו על אוסף המטבעות עם הסכום "+"<b>"+"הנמוך "+"</b>"+"ביותר";
            }
            else if (levelName[14] == 'H')
            {
                GameObject.Find("ExerciseInstructions").GetComponentInChildren<TextMeshProUGUI>().text = "לחצו על אוסף המטבעות עם הסכום " + "<b>" + "הגבוה " + "</b>" + "ביותר";
            }
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 3;
            TableUI tableOfCoinsComp = GameObject.Find("TableComp").GetComponent<TableUI>();
            tableOfCoinsComp.GetCellObject(0, 0).GetComponent<TextMeshProUGUI>().text = "קבוצת מטבעות 2";
            tableOfCoinsComp.GetCellObject(0, 1).GetComponent<TextMeshProUGUI>().text = "קבוצת מטבעות 1";
            GameObject tableCell1 = tableOfCoinsComp.GetCellObject(1, 0);
            GameObject tableCell2 = tableOfCoinsComp.GetCellObject(1, 1);
            tableCell1.transform.parent.transform.GetComponent<Canvas>().sortingOrder = 4;

            GameObject leftCoins = Resources.Load<GameObject>($"Prefabs/{dragItems[index][0].name}");
            GameObject rightCoins = Resources.Load<GameObject>($"Prefabs/{dragItems[index][1].name}");
            Instantiate(rightCoins, tableCell2.GetComponent<RectTransform>().parent);
            Instantiate(leftCoins, tableCell1.GetComponent<RectTransform>().parent);
            leftCoins.transform.SetSiblingIndex(2);
            rightCoins.transform.SetSiblingIndex(3);
            tableCell1.transform.SetSiblingIndex(0);
            tableCell2.transform.SetSiblingIndex(1);


        }
        else if (string.Equals(levelName, "MatchingCoins") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "מתחו קו בין סכומי מטבעות זהים מצד שמאל לימין";
            for (int i = 0; i < dragItems[index].Count; i++)
            {
                GameObject temp = Instantiate(dragItems[index][i], leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            for (int i = 0; i < itemSlots[index].Count; i++)
                Instantiate(itemSlots[index][i], rightGrid.transform);
            Color tempColor = GameObject.Find("SepLine").GetComponent<UnityEngine.UI.Image>().color;
            tempColor.a = 0;
            GameObject.Find("SepLine").GetComponent<UnityEngine.UI.Image>().color = tempColor;

        }
        

    }

    public int  getIndexOfWOrdProblem() {
        return indexWordProblem;
    }

    public string GetLevelName()
    {
        try
        {
            return levelNames[index % levelNames.Count];
        }
        catch { return "0000"; }
    }

    public void MoveToNextLevel()
    {
        foreach(Transform itemD in leftGrid.transform)
        {
            Destroy(itemD.gameObject);
        }
        foreach (Transform itemS in rightGrid.transform)
        {
            Destroy(itemS.gameObject);
        }
        foreach (Transform itemS in GameObject.Find("Task1").transform)
        {
            if(itemS.name != "GridMiddle" && itemS.name != "ExerciseInstructions")
                Destroy(itemS.gameObject);
        }
        foreach (Transform boxItem in answerBox.transform)
        {
            Destroy (boxItem.gameObject);
        }
        foreach (Transform boxItem in productsToDrag.transform)
        {
            Destroy(boxItem.gameObject);
        }
        GameObject productDes = GameObject.Find("ProductDescription");
        foreach (Transform child in productDes.transform)
            child.gameObject.SetActive(false);

        //Delete Table when returning back -WP - Finish...
        GameObject tableWordProblem = GameObject.Find("LeftSide1");
        Transform tr = tableWordProblem.transform.GetChild(0);
        

        if (levelNames[(index+1) % levelNames.Count].Substring(0, 4).Equals("Comp") && index!=-1){
            if (levelNames[(index) % levelNames.Count].Substring(0, 12) != "WordProblems") { //+1
                GameObject tableCells = GameObject.Find("TableComp").transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
                if (tableCells.transform.GetChild(2).gameObject != null && tableCells.transform.GetChild(3).gameObject!=null || index+1>=levelNames.Count)
                {
                    Destroy(tableCells.transform.GetChild(2).gameObject);
                    Destroy(tableCells.transform.GetChild(3).gameObject);
                }
                

            }

        }
        if (!levelNames[(index + 1) % levelNames.Count].Substring(0, 5).Equals("ETask") && !(levelNames[(index + 1) % levelNames.Count].Substring(0, 5).Equals("FTask"))){
            if (levelNames[(index + 1) % levelNames.Count].Substring(0, 4).Equals("Word") && index != -1) { //
                if (levelNames[index % levelNames.Count].Substring(0, 4).Equals("Comp")) {
                    GameObject tableCells = GameObject.Find("TableComp").transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
                    if (tableCells.transform.GetChild(2).gameObject != null && tableCells.transform.GetChild(3).gameObject != null || index + 1 >= levelNames.Count)
                    {
                        Destroy(tableCells.transform.GetChild(2).gameObject);
                        Destroy(tableCells.transform.GetChild(3).gameObject);
                    }
                }
            }
        }

        foreach(Transform coin in GameObject.Find("GridForCoins").transform) 
        { 
            Destroy(coin.gameObject); 
        }
        foreach (Transform coin in horLayGroup.transform)
        {
            Destroy(coin.gameObject);
        }
        foreach (Transform coin in GameObject.Find("price").transform)
        { 
            Destroy(coin.gameObject); 
        }
        foreach (Transform coin in GameObject.Find("product").transform)
        {
            Destroy(coin.gameObject);
        }
        foreach (Transform product in GameObject.Find("ProductDisplay").transform)
        {
            Destroy(product.gameObject);
        }
        TableUI tableComparison = GameObject.Find("TableComp").GetComponent<TableUI>();
        GameObject cells = tableComparison.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        foreach (Transform productComp in cells.transform) {
            if (productComp.gameObject.activeInHierarchy)
            {
                Destroy(productComp.gameObject);
            }
        }


        slider.value = 0;
        GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
        GameObject.Find("MoveButton").GetComponent<Button>().interactable = false;
        GameObject.Find("MoveButton").GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().enabled = false;
        GameObject.Find("ResetButton").GetComponent<Button>().interactable = false;
        GameObject.Find("ResetButton").GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        GameObject.Find("MarkForAnswer").GetComponent<UnityEngine.UI.Image>().sprite = null;
        GameObject.Find("StudentWrittenAnswer").GetComponent<TMP_InputField>().text = null;
        GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = null;
        GameObject.Find("StudentAnswer").GetComponent<TMP_InputField>().text = null;
        GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 0;
        Color tempColor = GameObject.Find("SepLine").GetComponent<UnityEngine.UI.Image>().color;
        tempColor.a = 1;
        GameObject.Find("SepLine").GetComponent<UnityEngine.UI.Image>().color = tempColor;
        if (index + 1 >= levelNames.Count)
        {
            if (levelNames[index % levelNames.Count].Substring(0, 4).Equals("Comp")) {
                GameObject left = GameObject.Find("TableComp").transform.GetChild(0).GetChild(1).GetChild(1).GetChild(2).gameObject;
                GameObject right = GameObject.Find("TableComp").transform.GetChild(0).GetChild(1).GetChild(1).GetChild(3).gameObject;

                Destroy(right.gameObject);
                Destroy(left.gameObject);
            }
            //clear all lists for this level sequence and give high sort number to main menu canva, the rest are 1(sorting order)}
            GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("GridBelow").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("MainMenu").GetComponent <Canvas>().sortingOrder = 2;
            GameObject.Find("ExcessTask").GetComponent <Canvas>().sortingOrder = 1;

            dragItems.Clear();
            items.Clear();
            itemSlots.Clear();
            levelNames.Clear();
            rightsum.Clear();
            correctAnswers.Clear();
            maxValues.Clear();
            rightSumsForLevel.Clear();
            questions.Clear();
            return;
        }

        if (string.Equals(levelNames[(index + 1) % levelNames.Count], "FTask") == true)
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 1;
        }
        else if (string.Equals(levelNames[(index + 1) % levelNames.Count], "FTask") == false && string.Equals(levelNames[(index + 1) % levelNames.Count].Substring(0, 4), "Word"))
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 2;
        }
        else if (string.Equals(levelNames[(index + 1) % levelNames.Count].Substring(0, 4), "Comp"))
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 2;
            //right.GetComponent<Canvas>().sortingOrder = 3;
            //left.GetComponent<Canvas>().sortingOrder = 3;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
        }
        else if (string.Equals(levelNames[(index + 1) % levelNames.Count], "CTask")
                  || string.Equals(levelNames[(index + 1) % levelNames.Count], "MatchingCoins"))
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            //GameObject.Find("FirstProductText").GetComponent<Canvas>().sortingOrder = 1;
            //GameObject.Find("SecondProductText").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;

        }
        else if (string.Equals(levelNames[(index + 1) % levelNames.Count], "ETask")) {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 2;

        }

        else
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = 1;
        }
        index++;
        MakeNewLevel(levelNames[index % levelNames.Count]);
    }

    public void SetIndexOfWordProblems(int index) {
        indexWordProblem = index;
    }

    public List<int> GetRightSumsForLevel() { return rightSumsForLevel; }
    public List<string> GetCorrectAnswersForlevel() { return correctAnswers[index]; }

    public void GoBackToMenu() {
        index = levelNames.Count;
        MoveToNextLevel();
    }
    

    public async Task MakeLevelSequence(int choice)
    {
        GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = 0;
        GameObject.Find("StudentLearningData").GetComponent<Canvas>().sortingOrder = 0;
        GameObject.Find("EntrancePage").GetComponent<Canvas>().sortingOrder = 0;
        GameObject.Find("GridBelow").GetComponent<Canvas>().sortingOrder = 5;
        string studentCode = GameObject.Find("StudentCodeInput").GetComponent<TMP_InputField>().text;
        IDictionary<string,object> studentLearningLevel = new Dictionary<string, object>();
        await FirebaseDatabase.DefaultInstance.GetReference($"ComputerParams/{studentCode}").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                
                DataSnapshot studentDataSnapShot = task.Result;
                if (studentDataSnapShot != null)
                {
                    foreach (DataSnapshot data in studentDataSnapShot.Children)
                    {
                        studentLearningLevel.Add(new KeyValuePair<string, object>(data.Key, data.Value));
                    }
                }
            }
        });

        if (choice == 1)
        { //Recognize coins
            Debug.Log("choice: " + choice);
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 1;
            
             

        }
        else if (choice == 2)
        {
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 1;
            
            Debug.Log("choice= " + choice);
            
        }
        else if (choice == 3)
        {
            Debug.Log("choice= " + choice);
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 1;
            
        }
            
        else if (choice == 4) {
            Debug.Log("choice= " + choice);
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("ExcessTask").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("WordProblems").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("ComparingPrices").GetComponent<Canvas>().sortingOrder = 1;

            

        }

        index = -1;
        if (await LoadExercisesForStudent(choice, studentLearningLevel))
        {
            MoveToNextLevel();
        }
            
    }


    public async Task<bool> LoadExercisesForStudent(int choice, IDictionary<string, object> studentDict)
    {
        currentStudent.SetCheapExpensive(int.Parse(studentDict["_cheapExpensive"].ToString()));
        currentStudent.SetIdentifyingCoinsBills(int.Parse(studentDict["_identifyingCoinsBills"].ToString()));
        currentStudent.SetChange(int.Parse(studentDict["_changeCalc"].ToString()));
        currentStudent.SetUnderstandingValue(int.Parse(studentDict["_understandingValue"].ToString()));
        //currentStudent.StudentParams.Add("_identifyingCoinsBills", int.Parse(studentDict["_identifyingCoinsBills"].ToString()));
        //currentStudent.StudentParams.Add("_cheapExpensive", int.Parse(studentDict["_cheapExpensive"].ToString()));
        //currentStudent.StudentParams.Add("_changeCalc", int.Parse(studentDict["_changeCalc"].ToString()));
        //currentStudent.StudentParams.Add("_understandingValue", int.Parse(studentDict["_understandingValue"].ToString()));
        //currentStudent.SetStudentGroupName(studentDict["_groupName"].ToString());
        Debug.Log("LoadExercisesForStudent");
        List<string> categories = new List<string>();
        List<object> values = new List<object>();
        List<string> dragItemsOptions = new List<string>();
        List<string> slotItemsOptions = new List<string>();
        List<string> productsOptions = new List<string>();
        List<string> coinCollectionsOptions = new List<string>();

        foreach (KeyValuePair<string, object> kv in studentDict)
        {
            categories.Add(kv.Key);
            values.Add(kv.Value);
        }
        await FirebaseDatabase.DefaultInstance.GetReference($"identifyingCoinsBills/{studentDict["_identifyingCoinsBills"]}/dragItems").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot draggable in snapshot.Children)
                {
                    dragItemsOptions.Add(draggable.Value.ToString());
                }
            }
        });
        await FirebaseDatabase.DefaultInstance.GetReference($"identifyingCoinsBills/{studentDict["_identifyingCoinsBills"]}/slotItems").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot slotItem in snapshot.Children)
                {
                    slotItemsOptions.Add(slotItem.Value.ToString());

                }
            }
        });
        await FirebaseDatabase.DefaultInstance.GetReference($"ProductsToPurchase/{studentDict["_identifyingCoinsBills"]}").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot product in snapshot.Children)
                {
                    slotItemsOptions.Add(product.Value.ToString());//changed from productsOptions to slotItemsOptions

                }
            }
        });
        await FirebaseDatabase.DefaultInstance.GetReference($"coinsCollections/{studentDict["_understandingValue"]}").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot product in snapshot.Children)
                {
                    coinCollectionsOptions.Add(product.Value.ToString());

                }
            }
        });
        await InitDataLists();

        List<GameObject> tempDragItems = new List<GameObject>(); //for each level
        List<GameObject> tempSlotItems = new List<GameObject>(); //for each level
        System.Random rand1 = new();
        int sliderMaxValue = 0;
        int coinOrProduct;
        if (choice == 1)
        {
            //List<string> allowedLevelNames = new List<string>() { "1TaskCM", "CTask" };
            for (int i = 0; i < 18; i++)
            {
                int coinFlip = rand1.Next(3);
                if (coinFlip == 0)
                {
                    levelNames.Add("1TaskCM");
                }

                else if (coinFlip == 1)
                {
                    levelNames.Add("CTask");
                }

                //else if (coinFlip == 2)
                //{
                    //levelNames.Add("7TaskCM");
                //}
            }
            foreach (string name in levelNames)
            {

                /*if (name == "7TaskCM")
                {
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    for (int j = 0; j < dragItemsOptions.Count; j++)
                    {
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[j]}");
                        tempDragItems.Add(gobj);
                    }
                    
                    bool didFindRightSideItem = false;
                    do
                    {
                        int coinIndex = rand1.Next(tempDragItems.Count);//sum of coins from right
                        GameObject coinsRight = tempDragItems[coinIndex];
                        if (coinsRight.GetComponent<Drag>().value <= int.Parse(studentDict["identifyingCoinsBills"].ToString()))
                        {

                            tempSlotItems.Add(coinsRight);
                            maxValues.Add(coinsRight.GetComponent<Drag>().value);
                            rightsum.Add(new List<int>() { coinsRight.GetComponent<Drag>().value });
                            itemSlots.Add(new List<GameObject>(tempSlotItems));
                            dragItems.Add(new List<GameObject>(tempDragItems));
                            correctAnswers.Add(new List<string>() { });
                            didFindRightSideItem = true;
                            break;
                        }
                        
                    } while (!didFindRightSideItem);
                }*/


                //יוצרים מופע של פריפאב
                //(מגרילים מספר עבור הערך של הטקסט בפריפאב(1,2,5,10,20,50,100,200
                //מצמידים לטקסט את המספר המוגרל בשלב הקודם
                //attach the chosen number to the value property of drag component
                /*
                 * adjust the following:
                maxValues.Add(sliderMaxValue);
                rightsum.Add(new List<int>() { sliderMaxValue });
                itemSlots.Add(new List<GameObject>(tempSlotItems));
                dragItems.Add(new List<GameObject>(tempDragItems));
                if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                 }
                else {rest of the code from here to end of this choice}
                */
                //else
                //{
                List<int> slotItemValues = new List<int>() { 1, 2, 5, 10, 20, 50, 100, 200 };
                do
                {
                    int dragItemsNumRandom = rand1.Next(1, 11); //How many items appear on the left side
                    sliderMaxValue = 0;
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    for (int j = 0; j < dragItemsNumRandom; j++)
                    {
                        int dragItemsIndex = rand1.Next(dragItemsOptions.Count); //choose item from options
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[dragItemsIndex]}");
                        tempDragItems.Add(gobj);
                    }
                    int slotItemsIndex = rand1.Next(slotItemsOptions.Count);
                    coinOrProduct = rand1.Next(2); //0=coin,1=product
                    string item = slotItemsOptions[slotItemsIndex];
                    tempSlotItems.Add(Resources.Load<GameObject>($"Prefabs/{item}"));
                    if (coinOrProduct == 0 && tempSlotItems[0].GetComponent<Drag>().IsCoin == true)//item.Substring(item.Length - 4, 4) == "Drag"
                    {
                        for (int iterDragItems = 0; iterDragItems < tempDragItems.Count; iterDragItems++)
                        {
                            if (tempDragItems[iterDragItems].name == tempSlotItems[0].name)
                            {
                                sliderMaxValue++;
                            }
                        }
                    }
                    else if (coinOrProduct == 1 && slotItemValues.Contains(tempSlotItems[0].GetComponent<Drag>().value) && (tempSlotItems[0].GetComponent<Drag>().IsCoin == false && name == "CTask") || (tempSlotItems[0].GetComponent<Drag>().IsCoin == true && name == "CTask"))//product on the right side
                    {
                        for (int iterDragItems = 0; iterDragItems < tempDragItems.Count; iterDragItems++)
                        {
                            if (tempDragItems[iterDragItems].GetComponent<Drag>().value == tempSlotItems[0].GetComponent<Drag>().value)
                            {
                                sliderMaxValue++;
                            }
                        }
                    }
                    
                } while (sliderMaxValue == 0);
                    //int temp = sliderMaxValue;
                    maxValues.Add(sliderMaxValue);
                    rightsum.Add(new List<int>() { tempSlotItems[0].GetComponent<Drag>().value });
                    dragItems.Add(new List<GameObject>(tempDragItems));
                    itemSlots.Add(new List<GameObject>(tempSlotItems));
                    correctAnswers.Add(new List<string>() { });
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                //}
                
            }
        }
        else if (choice == 2)
        {
            List<string> allowedLevelNames = new List<string>() { "2TaskCM", "MatchingCoins" };

            for (int i = 0; i < 12; i++)
            {
                int CoinFlip2 = rand1.Next(4);
                if (CoinFlip2 == 0)
                {
                    levelNames.Add("2TaskCM");
                }

                else if (CoinFlip2 == 1)
                {
                    levelNames.Add("MatchingCoins");
                }
                else if (CoinFlip2 == 2)
                {
                    levelNames.Add("8TaskCM");
                }
                else if (CoinFlip2 == 3)
                {
                    levelNames.Add("10TaskCM");
                }
                else if (CoinFlip2 == 4)
                {
                    levelNames.Add("7TaskCM");
                }
            }
            foreach (string name in levelNames)
            {
                Debug.Log(name);
                if (name == "2TaskCM")
                {
                    bool didFindProduct = false;
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    for (int j = 0; j < dragItemsOptions.Count; j++)
                    {
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[j]}");
                        tempDragItems.Add(gobj);
                    }
                    do
                    {
                        tempSlotItems.Clear();
                        int slotItemIndex = rand1.Next(slotItemsOptions.Count);
                        coinOrProduct = rand1.Next(2);
                        string item2 = slotItemsOptions[slotItemIndex];
                        tempSlotItems.Add(Resources.Load<GameObject>($"Prefabs/{item2}"));
                        if (coinOrProduct == 1 && tempSlotItems[0].GetComponent<Drag>().IsCoin == false)//product on the right side, 2TaskCM
                        {
                            int counter = 0;
                            for (int iterDragItems = 0; iterDragItems < tempDragItems.Count; iterDragItems++)
                            {
                                counter += tempDragItems[iterDragItems].GetComponent<Drag>().value;
                                if (counter >= tempSlotItems[0].GetComponent<Drag>().value)
                                {
                                    sliderMaxValue = tempSlotItems[0].GetComponent<Drag>().value;
                                    didFindProduct = true;
                                    break;
                                }
                            }
                        }
                    } while (!didFindProduct);
                    maxValues.Add(sliderMaxValue);
                    rightsum.Add(new List<int>() { sliderMaxValue });
                    itemSlots.Add(new List<GameObject>(tempSlotItems));
                    dragItems.Add(new List<GameObject>(tempDragItems));
                    correctAnswers.Add(new List<string>() { });
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                }
                else if (name == "MatchingCoins") //MatchingCoinsExercise
                {
                    int numOfItems = rand1.Next(2, 3); //How many items appear on the left side and the right side
                    int numOfMatching = 0;
                    int counterLeft = 0;
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    do
                    {
                        int leftcoinsIndex = rand1.Next(coinCollectionsOptions.Count); //choose item from options
                        string selectedSlot = coinCollectionsOptions[leftcoinsIndex];
                        if ((selectedSlot.EndsWith(")")))
                        {
                            GameObject gobj = Resources.Load<GameObject>($"Prefabs/{selectedSlot}");
                            if (!tempDragItems.Contains(gobj))
                            {
                                tempDragItems.Add(gobj);
                                counterLeft++;
                            }

                        }
                    } while (counterLeft != numOfItems);
                    List<int> valuesNum = new List<int>();
                    for (int i = 0; i < tempDragItems.Count; i++)
                    {
                        valuesNum.Add(tempDragItems[i].GetComponent<Drag>().value);
                    }
                    do
                    {
                        int rightcoinsCollectionIndex = rand1.Next(coinCollectionsOptions.Count);

                        string selectedRight = coinCollectionsOptions[rightcoinsCollectionIndex];
                        GameObject coinsCollectionRight = Resources.Load<GameObject>($"Prefabs/{selectedRight}");
                        if (coinsCollectionRight.GetComponent<Drag>().IsCoin)
                        {
                            foreach (GameObject coinsLeft in tempDragItems)
                            {
                                if (coinsLeft.GetComponent<Drag>().value == coinsCollectionRight.GetComponent<Drag>().value && !tempSlotItems.Contains(coinsCollectionRight) && valuesNum.Contains(coinsCollectionRight.GetComponent<Drag>().value))
                                {
                                    valuesNum.Remove(coinsCollectionRight.GetComponent<Drag>().value);
                                    tempSlotItems.Add(coinsCollectionRight);
                                    numOfMatching++;
                                    break;
                                }
                            }
                        }
                    } while (valuesNum.Count != 0);
                    maxValues.Add(numOfMatching);
                    rightsum.Add(new List<int>() { });
                    itemSlots.Add(new List<GameObject>(tempSlotItems));
                    dragItems.Add(new List<GameObject>(tempDragItems)); 
                    correctAnswers.Add(new List<string>() { });
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                }
                else if (name == "7TaskCM")
                {
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    for (int j = 0; j < dragItemsOptions.Count; j++)
                    {
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[j]}");
                        tempDragItems.Add(gobj);
                    }
                    List<int> coinsIndexes = new();
                    for (int i = 0; i < dragItemsOptions.Count; i++)
                    {
                        //GameObject coinPrefab = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[i]}");
                        //if (coinPrefab.GetComponent<Drag>().value != 1)
                        //{
                        coinsIndexes.Add(i);
                        //}
                    }
                    bool didFindRightSideItem = false;
                    do
                    {
                        int coinIndex = rand1.Next(coinsIndexes.Count);
                        GameObject coinRightSide = tempDragItems[coinsIndexes[coinIndex]];
                        if (coinRightSide.GetComponent<Drag>().value <= int.Parse(studentDict["_identifyingCoinsBills"].ToString()))
                        {

                            tempSlotItems.Add(coinRightSide);
                            maxValues.Add(coinRightSide.GetComponent<Drag>().value);
                            rightsum.Add(new List<int>() { coinRightSide.GetComponent<Drag>().value });
                            itemSlots.Add(new List<GameObject>(tempSlotItems));
                            dragItems.Add(new List<GameObject>(tempDragItems));
                            correctAnswers.Add(new List<string>() { });
                            didFindRightSideItem = true;
                            break;
                        }
                        else
                        {
                            if (coinsIndexes.Count != 0)
                            {
                                coinsIndexes.Remove(coinsIndexes[coinIndex]);

                            }
                            else
                            {
                                for (int i = 0; i < coinCollectionsOptions.Count; i++)
                                {
                                    coinsIndexes.Add(i);
                                }
                                coinsIndexes.Remove(coinsIndexes[coinIndex]);

                            }
                        }
                    } while (!didFindRightSideItem);
                }
                else if (name == "8TaskCM")
                {
                    int sMaxValue = 0;
                    List<int> possibleValues = new List<int>() { 1, 2, 5, 10, 20, 50, 100 };
                    int studentLevel = int.Parse(studentDict["_identifyingCoinsBills"].ToString());
                    int listIndex = possibleValues.IndexOf(studentLevel);
                    int randIndex = rand1.Next(0, listIndex+1);


                    for (int j = 0; j < slotItemsOptions.Count; j++)
                    {
                        if (slotItemsOptions[j].Contains("Drag"))
                        {
                            GameObject gobj = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[j]}");
                            tempDragItems.Add(gobj);
                        }
                    }

                    //יוצרים מופע של פריפאב(new prefab)
                    List<GameObject> valueRightSide = new();
                    GameObject valueObject = Resources.Load<GameObject>("Prefabs/NumericValue");

                    //choose random number in range of student possibilities
                    //rand1.Next(1, int.Parse(studentDict["identifyingCoinsBills"].ToString()) + 1)
                    int randValueRight = possibleValues[randIndex];
                    //מצמידים לטקסט את המספר המוגרל בשלב הקודם
                    valueObject.GetComponentInChildren<TextMeshProUGUI>().text = randValueRight.ToString() + "₪";
                    //attach the chosen number to the value property of drag component
                    valueObject.GetComponent<Drag>().value = randValueRight;
                    valueRightSide.Add(valueObject);
                    for (int i = 0; i < tempDragItems.Count; i++)
                    {
                        if (tempDragItems[i].GetComponent<Drag>().value == randValueRight)
                        {
                            sMaxValue++;
                        }
                    }
                    maxValues.Add(sMaxValue);
                    rightsum.Add(new List<int>() { randValueRight });
                    itemSlots.Add(new List<GameObject>(valueRightSide));
                    dragItems.Add(new List<GameObject>(tempDragItems));
                    correctAnswers.Add(new List<string>() { });
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    /*
                     * adjust the following:
                    maxValues.Add(sliderMaxValue);
                    rightsum.Add(new List<int>() { sliderMaxValue });
                    itemSlots.Add(new List<GameObject>(tempSlotItems));//list of one item - the coin/bill
                    dragItems.Add(new List<GameObject>(tempDragItems));
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                     */
                }
                else if (name == "10TaskCM") {
                    sliderMaxValue = 0;
                    
                    for (int i = 0; i < dragItemsOptions.Count; i++) {
                        GameObject coin = Resources.Load<GameObject>($"Prefabs/{dragItemsOptions[i]}");
                        if (coin.GetComponent<Drag>().value <= int.Parse(studentDict["_identifyingCoinsBills"].ToString())) {
                            tempDragItems.Add(coin);
                        }
                    }
                    List<int> productIndexes = new();
                    for (int i = 0; i < slotItemsOptions.Count; i++) {
                        if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot")) {
                            GameObject product = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[i]}");
                            if (product.GetComponent<Drag>().value <= int.Parse(studentDict["_identifyingCoinsBills"].ToString())) {
                                productIndexes.Add(i);
                            }
                        }
                    }
                    int randProductIndex = rand1.Next(productIndexes.Count);
                    GameObject chosenProduct = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[productIndexes[randProductIndex]]}");
                    itemSlots.Add(new List<GameObject>() { chosenProduct });
                    dragItems.Add(new List<GameObject>(tempDragItems));
                    correctAnswers.Add(new List<string>() { });
                    maxValues.Add(1);
                    List<List<int>> tempRightSum = new();
                    bool didBreak = false;
                    for (int i = 0; i < tempDragItems.Count; i++) {
                        GameObject draggableCoin = tempDragItems[i];
                        if (draggableCoin.GetComponent<Drag>().value >= chosenProduct.GetComponent<Drag>().value) {
                            tempRightSum.Add(new List<int>() { draggableCoin.GetComponent<Drag>().value, chosenProduct.GetComponent<Drag>().value });
                        }
                    }
                    int difference = -10000;
                    for (int i = 0; i < tempRightSum.Count; i++) {
                        difference = tempRightSum[i][1] - tempRightSum[i][0];
                        if (difference == 0) {
                            rightsum.Add(new List<int>() { tempRightSum[i][0] });
                            didBreak = true;
                            break;
                        }
                        if (difference < tempRightSum[i][1] - tempRightSum[i][0]) {
                            difference = tempRightSum[i][1] - tempRightSum[i][0];
                        }
                    }
                    if (!didBreak) {
                        rightsum.Add(new List<int>() { Math.Abs(difference - chosenProduct.GetComponent<Drag>().value) });
                    
                    }
                    if (tempDragItems.Count > 0) { tempDragItems.Clear(); }
                    if (tempSlotItems.Count > 0) { tempSlotItems.Clear(); }
                    //rightsum.Add(new List<int>() { chosenProduct.GetComponent<Drag>().value });
                }

            }
        }

        else if (choice == 3)
        {
            List<GameObject> tableProducts = new List<GameObject>();
            int maxPrice = int.Parse(studentDict["_cheapExpensive"].ToString());
            List<int> productsPrices = new List<int>();
            int productsCounter = 0;
            if (maxPrice == 1) { productsCounter = 1; }
            else if (maxPrice == 2) { productsCounter = 2; }
            else if (maxPrice >= 5) { productsCounter = 5; }

            for (int i = 0; i < 12; i++)
            {
                int exerciseType = rand1.Next(7);
                if (exerciseType == 0) { levelNames.Add("WordProblems1"); }
                else if (exerciseType == 1) { levelNames.Add("WordProblems2"); }
                else if (exerciseType == 2) { levelNames.Add("WordProblems3"); }
                else if (exerciseType == 3) { levelNames.Add("ComparingPricesHighest"); }
                else if (exerciseType == 4) { levelNames.Add("ComparingPricesLowest"); }
                else if (exerciseType == 5) { levelNames.Add("CoinsComparingHighest"); }
                else if (exerciseType == 6) { levelNames.Add("CoinsComparingLowest"); }



            }

            List<int> productIndexes = new List<int>();

            foreach (string levelName in levelNames)
            {
                if (levelName != "CoinsComparingHighest" && levelName != "CoinsComparingLowest")
                {
                    Debug.Log(levelName);
                    productIndexes.Clear();
                    for (int i = 0; i < slotItemsOptions.Count; i++)
                    {
                        if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot"))
                            productIndexes.Add(i);
                    }
                }
                if (levelName.Substring(0, 12).Equals("WordProblems"))
                {
                    productsPrices.Clear();
                    tableProducts.Clear();
                    do
                    {
                        int randomProductIndex = rand1.Next(productIndexes.Count);

                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[productIndexes[randomProductIndex]]}");
                        if (gobj.GetComponent<Drag>().value <= maxPrice && !tableProducts.Contains(gobj) && !productsPrices.Contains(gobj.GetComponent<Drag>().value))
                        {
                            tableProducts.Add(gobj);
                            productsPrices.Add(gobj.GetComponent<Drag>().value);
                        }

                        if (productIndexes.Count != 0)
                        {
                            productIndexes.Remove(productIndexes[randomProductIndex]);
                            if (productIndexes.Count == 0)
                            {
                                for (int i = 0; i < slotItemsOptions.Count; i++)
                                {
                                    if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot"))
                                        productIndexes.Add(i);
                                }
                            }

                        }

                    } while (tableProducts.Count < productsCounter);
                    TableUI productsTable = GameObject.Find("TableOfProducts").GetComponent<TableUI>();
                    productsTable.Rows = tableProducts.Count + 1; // 1 for header of table


                    if (productsTable.Rows == 3)
                    {
                        productsTable.UpdateRowHeight(110, 1);
                        productsTable.GetCellObject(1, 1).GetComponent<RectTransform>().sizeDelta = new(200, 110f);
                        productsTable.GetCellObject(1, 1).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(200, 80);
                        productsTable.UpdateRowHeight(110, 2);
                        productsTable.GetCellObject(1, 2).GetComponent<RectTransform>().sizeDelta = new(200, 110f);
                        productsTable.GetCellObject(1, 2).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(200, 80);
                    }
                    else if (productsTable.Rows == 2)
                    {
                        productsTable.UpdateRowHeight(130, 1);
                        productsTable.GetCellObject(1, 1).GetComponent<RectTransform>().sizeDelta = new(200, 130);
                        productsTable.GetCellObject(1, 1).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(200, 100);
                    }
                    else if (productsTable.Rows == 6)
                    {
                        for (int i = 1; i < productsTable.Rows; i++)
                        {
                            productsTable.UpdateRowHeight(120f, i);//30
                            productsTable.GetCellObject(i, 1).GetComponent<RectTransform>().sizeDelta = new(200, 100f);
                            productsTable.GetCellObject(i, 1).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(200, 70);
                        }
                    }

                    dragItems.Add(new List<GameObject>(tableProducts));
                    itemSlots.Add(new List<GameObject>() { });
                    maxValues.Add(1);
                    int priceOfProductMax = tableProducts[0].GetComponent<Drag>().value;
                    int priceOfProductMin = tableProducts[0].GetComponent<Drag>().value;
                    string correctWrittenAnswerCheap = "";
                    string correctWrittenAnswerExpensive = "";
                    foreach (GameObject tableProduct in tableProducts)
                    {
                        if (priceOfProductMax <= tableProduct.GetComponent<Drag>().value)
                        {
                            priceOfProductMax = tableProduct.GetComponent<Drag>().value;
                        }
                        else if (priceOfProductMin >= tableProduct.GetComponent<Drag>().value)
                        {
                            priceOfProductMin = tableProduct.GetComponent<Drag>().value;
                        }
                    }
                    foreach (GameObject tableProductGameObject in tableProducts)
                    {
                        if (tableProductGameObject.GetComponent<Drag>().value == priceOfProductMax)
                        {
                            correctWrittenAnswerExpensive = tableProductGameObject.GetComponent<Drag>().Name;
                        }
                        else if (tableProductGameObject.GetComponent<Drag>().value == priceOfProductMin)
                        {
                            correctWrittenAnswerCheap = tableProductGameObject.GetComponent<Drag>().Name;
                        }
                    }
                    if (levelName[12].Equals('1'))
                    {

                        rightsum.Add(new List<int>() { priceOfProductMax });
                        correctAnswers.Add(new List<string>() { correctWrittenAnswerExpensive });


                    }
                    else if (levelName[12].Equals('2'))
                    {
                        rightsum.Add(new List<int>() { priceOfProductMin });
                        correctAnswers.Add(new List<string>() { correctWrittenAnswerCheap });
                    }
                    else if (levelName[12].Equals('3'))
                    {
                        System.Random rand = new System.Random();
                        int randPrice = rand.Next(priceOfProductMin, priceOfProductMax + 1);
                        string questionStart = "יש לך ";
                        string questionEnd = ". מה אפשר לקנות?";
                        List<string> answers = new List<string>();
                        List<int> rightSums = new List<int>();
                        //GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
                        GameObject.Find("QuestionContent1").GetComponent<TextMeshProUGUI>().text = questionStart + " " + randPrice.ToString() + "₪ " + questionEnd;
                        foreach (GameObject tableProduct in tableProducts)// in dragItems[index]
                        {
                            if (tableProduct.GetComponent<Drag>().value <= randPrice)
                            {
                                answers.Add(tableProduct.GetComponent<Drag>().Name);
                                rightSums.Add(tableProduct.GetComponent<Drag>().value);
                            }
                        }
                        correctAnswers.Add(new List<string>(answers));
                        rightsum.Add(new List<int>(rightSums));
                    }
                }
                else if (levelName.Substring(0, 4).Equals("Comp"))
                {

                    TableUI pairTable = GameObject.Find("TableComp").GetComponent<TableUI>();
                    List<GameObject> tableProductsComp = new List<GameObject>();
                    List<int> pricesOfProducts = new List<int>();
                    do
                    {
                        int productIndex = rand1.Next(productIndexes.Count);

                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[productIndexes[productIndex]]}");
                        if (gobj.GetComponent<Drag>().value <= maxPrice && !tableProductsComp.Contains(gobj) && !pricesOfProducts.Contains(gobj.GetComponent<Drag>().value)) //erased : gobj.GetComponent<Drag>().IsCoin == false
                        {
                            tableProductsComp.Add(gobj);
                            pricesOfProducts.Add(gobj.GetComponent<Drag>().value);
                            gobj.GetComponent<Drag>().interactable = true;

                        }
                        if (productIndexes.Count != 0)
                        {
                            productIndexes.Remove(productIndexes[productIndex]);
                        }
                        else if (productIndexes.Count == 0)
                        {
                            for (int i = 0; i < slotItemsOptions.Count; i++)
                            {
                                if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot"))
                                    productIndexes.Add(i);
                            }
                        }
                    } while (tableProductsComp.Count < 2);
                    pairTable.Rows = tableProductsComp.Count;
                    pairTable.UpdateRowHeight(250f, 1);

                    dragItems.Add(new List<GameObject>(tableProductsComp));
                    correctAnswers.Add(new List<string>() { });
                    maxValues.Add(1);
                    itemSlots.Add(new List<GameObject>() { });
                    if (levelName[15].Equals('H'))
                    {
                        if (tableProductsComp[0].GetComponent<Drag>().value > tableProductsComp[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableProductsComp[0].GetComponent<Drag>().value });
                        }
                        else if (tableProductsComp[0].GetComponent<Drag>().value < tableProductsComp[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableProductsComp[1].GetComponent<Drag>().value });

                        }

                    }
                    else if (levelName[15].Equals('L'))
                    {

                        if (tableProductsComp[0].GetComponent<Drag>().value < tableProductsComp[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableProductsComp[0].GetComponent<Drag>().value });
                        }
                        else if (tableProductsComp[0].GetComponent<Drag>().value > tableProductsComp[1].GetComponent<Drag>().value)
                        {

                            rightsum.Add(new List<int>() { tableProductsComp[1].GetComponent<Drag>().value });

                        }

                    }
                }
                else if (levelName.Substring(0,5).Equals("Coins")) {
                    TableUI coinsTable = GameObject.Find("TableComp").GetComponent<TableUI>();
                    List<GameObject> tableCoins = new();
                    List<int> coinsValues = new ();
                    List<int> coinsIndexes = new();
                    for (int i = 0; i < coinCollectionsOptions.Count; i++) {
                        Debug.Log(coinCollectionsOptions[i]);
                        GameObject coins = Resources.Load<GameObject>($"Prefabs/{coinCollectionsOptions[i]}");
                        Debug.Log(coins);
                        if (coins.GetComponent<Drag>().value <= maxPrice)
                        {
                            coinsIndexes.Add(i);
                        }
                    }
                    do
                    {
                        int coinsIndex = rand1.Next(coinsIndexes.Count);
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{coinCollectionsOptions[coinsIndexes[coinsIndex]]}");
                        if (!tableCoins.Contains(gobj) && !coinsValues.Contains(gobj.GetComponent<Drag>().value)) //erased : gobj.GetComponent<Drag>().IsCoin == false, gobj.GetComponent<Drag>().value <= maxPrice
                        {
                            tableCoins.Add(gobj);
                            coinsValues.Add(gobj.GetComponent<Drag>().value);
                            gobj.GetComponent<Drag>().interactable = true;

                        }
                        else {
                            if (coinsIndexes.Count != 0)
                            {
                                coinsIndexes.Remove(coinsIndexes[coinsIndex]);
                                if (coinsIndexes.Count == 0) {
                                    for (int i = 0; i < coinCollectionsOptions.Count; i++)
                                    {
                                        GameObject coinsObj = Resources.Load<GameObject>($"Prefans/{coinCollectionsOptions[i]}");
                                        if (coinsObj.GetComponent<Drag>().value <= maxPrice)
                                        {
                                            coinsIndexes.Add(i);
                                        }
                                    }
                                }
                            }
                            //else {
                              //  for (int i = 0; i < coinCollectionsOptions.Count; i++)
                                //{
                                  //  coinsIndexes.Add(i);
                                //}

                            //}
                        }
                        
                          
                        
                    } while (tableCoins.Count < 2);
                    coinsTable.Rows = tableCoins.Count;
                    coinsTable.UpdateRowHeight(250f, 1);

                    dragItems.Add(new List<GameObject>(tableCoins));
                    correctAnswers.Add(new List<string>() { });
                    maxValues.Add(1);
                    itemSlots.Add(new List<GameObject>() { });
                    if (levelName[14].Equals('H'))
                    {
                        if (tableCoins[0].GetComponent<Drag>().value > tableCoins[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableCoins[0].GetComponent<Drag>().value });
                        }
                        else if (tableCoins[0].GetComponent<Drag>().value < tableCoins[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableCoins[1].GetComponent<Drag>().value });

                        }

                    }
                    else if (levelName[14].Equals('L'))
                    {

                        if (tableCoins[0].GetComponent<Drag>().value < tableCoins[1].GetComponent<Drag>().value)
                        {
                            rightsum.Add(new List<int>() { tableCoins[0].GetComponent<Drag>().value });
                        }
                        else if (tableCoins[0].GetComponent<Drag>().value > tableCoins[1].GetComponent<Drag>().value)
                        {

                            rightsum.Add(new List<int>() { tableCoins[1].GetComponent<Drag>().value });

                        }

                    }

                }
            }
        }
        else if (choice == 4)
        {
            List<GameObject> leftProduct = new List<GameObject>(); //ETask
            List<int> productIndexes = new List<int>();
            int maxPrice = int.Parse(studentDict["_changeCalc"].ToString());

            for (int i = 0; i < 12; i++)
            {
                int exerciseType = rand1.Next(2);
                if (exerciseType == 0) { levelNames.Add("ETask"); }
                else if (exerciseType == 1) { levelNames.Add("FTask"); }
            }



            foreach (string levelName in levelNames)
            {
                if (levelName == "ETask")
                {
                    leftProduct.Clear();
                    productIndexes.Clear();

                    for (int i = 0; i < slotItemsOptions.Count; i++)
                    {
                        if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot"))
                            productIndexes.Add(i);
                    }

                    do
                    {
                        int randomProductIndex = rand1.Next(productIndexes.Count);
                        GameObject gobj = Resources.Load<GameObject>($"Prefabs/{slotItemsOptions[productIndexes[randomProductIndex]]}");
                        if (gobj.GetComponent<Drag>().value <= maxPrice)
                        {
                            leftProduct.Add(gobj);
                        }

                        if (productIndexes.Count != 0)
                        {
                            productIndexes.Remove(productIndexes[randomProductIndex]);
                            if (productIndexes.Count == 0)
                            {
                                for (int i = 0; i < slotItemsOptions.Count; i++)
                                {
                                    if (!slotItemsOptions[i].Contains("Drag") && !slotItemsOptions[i].Contains("Slot"))
                                        productIndexes.Add(i);
                                }
                            }

                        }

                    } while (leftProduct.Count != 1);

                    dragItems.Add(new List<GameObject>(leftProduct));
                    correctAnswers.Add(new List<string>() { });
                    int randOwnPrice = rand1.Next(1, leftProduct[0].GetComponent<Drag>().value + 1);
                    rightsum.Add(new List<int>() { randOwnPrice });
                    maxValues.Add(1);
                    itemSlots.Add(new List<GameObject>() { });
                    string startQuestion = "יש לך ";
                    string endQuestion = "האם יש לך מספיק כסף לקנות את המוצר?";
                    questions.Add(new List<string>() { startQuestion + randOwnPrice.ToString() + "₪ " + endQuestion });


                }
                else if (levelName == "FTask")
                {
                    List<string> coinsBills = new() { "1nisDrag", "2nisDrag", "5nisDrag", "10nisDrag", "20nisDrag", "50nisDrag", "100nisDrag", "200nisDrag" };
                    
                    maxValues.Add(0);
                    rightsum.Add(new() { });
                    List<GameObject> temp = new();
                    for (int i = 0; i <coinsBills.Count ; i++)//currentStudent.GetIdentifyingCoinsBills()
                    {

                        if (coinsBills[i][1] == 'n')
                        {
                            int character = coinsBills[i][0] - '0';
                            if (character <= currentStudent.GetChange())
                            {
                                GameObject coin = Resources.Load<GameObject>($"Prefabs/{coinsBills[i]}");
                                temp.Add(coin);
                            }
                        }
                        else if (coinsBills[i][2] == 'n')
                        {
                            int value = int.Parse(coinsBills[i].Substring(0, 2));
                            if (value <= currentStudent.GetChange())
                            {
                                GameObject coin = Resources.Load<GameObject>($"Prefabs/{coinsBills[i]}");
                                temp.Add(coin);

                            }
                        }
                        else if (coinsBills[i][3] == 'n') {
                            int value = int.Parse(coinsBills[i].Substring(0, 3));
                            if (value <= currentStudent.GetChange()) {
                                GameObject coin = Resources.Load<GameObject>($"Prefabs/{coinsBills[i]}");
                                temp.Add(coin);
                            }
                        }



                        /*else if (coinsBills[i][2] == 'n' && int.Parse(coinsBills[i].Substring(0, 2)) <= currentStudent.GetIdentifyingCoinsBills())
                        {
                            GameObject coin = Resources.Load<GameObject>($"Prefabs/{coinsBills[i]}");
                            temp.Add(coin);
                        }
                        else if (coinsBills[i][3] == 'n' && int.Parse(coinsBills[i].Substring(0, 2)) <= currentStudent.GetIdentifyingCoinsBills())
                        {
                            GameObject coin = Resources.Load<GameObject>($"Prefabs/{coinsBills[i]}");
                            temp.Add(coin);

                        }*/
                    }
                    dragItems.Add(new List<GameObject>(temp));
                    itemSlots.Add(new List<GameObject>() { });
                    questions.Add(new List<string>() { });
                    correctAnswers.Add(new List<string>() { });

                }
            }
        }
        
        return true;
    }
    

    public async Task<bool> InitDataLists()
    {
        Debug.Log("InitDataLists");

        identifying_dragItems = new List<List<string>>() {
                                                          new() { "1nisDrag", "2nisDrag" }, //1
                                                          new() { "1nisDrag", "2nisDrag","5nisDrag" }, //2
                                                          new () { "1nisDrag", "2nisDrag", "5nisDrag","10nisDrag" }, //5,10
                                                          new () { "1nisDrag", "2nisDrag", "5nisDrag", "10nisDrag","20nisDrag","50nisDrag" },//20
                                                          new () { "1nisDrag", "2nisDrag", "5nisDrag", "10nisDrag","20nisDrag","50nisDrag","100nisDrag" }, //50
                                                          new () { "1nisDrag", "2nisDrag", "5nisDrag", "10nisDrag","20nisDrag","50nisDrag","100nisDrag","200nisDrag" } //100,200

        };
        identifying_slotItems = new List<List<string>>() {
                                                           new(){"1nisDrag"},
                                                           new() {"1nisDrag","2nisDrag" },
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" },
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" ,"10nisDrag"},
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" ,"10nisDrag","20nisDrag"},
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" ,"10nisDrag","20nisDrag","50nisDrag"},
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" ,"10nisDrag","20nisDrag","50nisDrag","100nisDrag"},
                                                           new() {"1nisDrag","2nisDrag","5nisDrag" ,"10nisDrag","20nisDrag","50nisDrag","100nisDrag","200nisDrag"},


        };
        productsToPurchase = new List<List<string>>() {
                                                            new() {"Mehaded","Eraser","LolliPop","Glue" }, //1
                                                            new() { "Mehaded", "Eraser", "LolliPop", "Glue","Cucumber","EshelLeben"},//2
                                                            new() { "Mehaded", "Eraser", "LolliPop", "Glue", "Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","Tomato","WhiteCheese","orange"},//5
                                                            new() {"Mehaded", "Eraser", "LolliPop", "Glue","Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","tomato","whiteCheese","Baflot","Apple","Bread","Hat","Coffee","KeyChain","Maadan","PetiBer","Plate","Shoko","SliceBoard","WaterBottle","WaterMelon" },//10
                                                            new() {"Mehaded", "Eraser", "LolliPop", "Glue","Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","tomato","whiteCheese","Baflot","Apple","Bread","Hat","Coffee","KeyChain","Maadan","PetiBer","Plate","Shoko","SliceBoard","WaterBottle","WaterMelon","T-Shirt","Scarf","Eggs" },//20
                                                            new() {"Mehaded", "Eraser", "LolliPop", "Glue", "Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","tomato","whiteCheese","Baflot","Apple","Bread","Hat","Coffee","KeyChain","Maadan","PetiBer","Plate","Shoko","SliceBoard","WaterBottle","WaterMelon","T-Shirt","Scarf","Eggs","Jacket","WaterPackage" },//50
                                                            new() {"Mehaded", "Eraser", "LolliPop", "Glue", "Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","tomato","whiteCheese","Baflot","Apple","Bread","Hat","Coffee","KeyChain","Maadan","PetiBer","Plate","Shoko","SliceBoard","WaterBottle","WaterMelon","T-Shirt","Scarf","Eggs","Jacket","WaterPackage","SchoolBag","Coat" },//100
                                                            new() {"Mehaded", "Eraser", "LolliPop", "Glue", "Cucumber", "EshelLeben", "bamba","bisli","chocolateTable","HairBrush","Key","KifKef","ShkedeiMarak","tomato","whiteCheese","Baflot","Apple","Bread","Hat","Coffee","KeyChain","Maadan","PetiBer","Plate","Shoko","SliceBoard","WaterBottle","WaterMelon","T-Shirt","Scarf","Eggs","Jacket","WaterPackage","SchoolBag","Coat","HairDry","FlowerPot" },//200
        };
        coinsCollection = new List<List<string>>() {
                                                            new(){"1nisDrag","2nisSlot(1,1)","2nisDrag" }, //2
                                                            new() { "1nisDrag","2nisSlot(1,1)", "2nisDrag","3nisSlot(2,1)","3nisDrag(1,1,1)","4nisDrag(2,2)","4nisSlot(2,1,1)","4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)","5nisDrag","5nisSlot(1,1,1,1,1)" },//5
                                                            new(){ "1nisDrag", "2nisSlot(1,1)", "2nisDrag", "3nisSlot(2,1)", "3nisDrag(1,1,1)", "4nisDrag(2,2)", "4nisSlot(2,1,1)", "4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)", "5nisDrag", "5nisSlot(1,1,1,1,1)","6nisSlot(1,1,1,1,1,1)","6nisSlot(5,1)","6nisDrag(2,2,2)","7nisDrag(2,5)","7nisSlot(2,2,2,1)","7nisSlot(5,1,1)","8nisSlot(2,2,2,2)","8nisDrag(5,1,1,1)","8nisSlot(2,5,1)","9nisSlot(5,2,1,1)","9nisSlot(5,2,2)","10nisDrag(5,5)","10nisDrag","10nisSlot(5,2,2,1)","10nisSlot(2,2,2,2,2)" },//10
                                                            new(){ "1nisDrag", "2nisSlot(1,1)", "2nisDrag", "3nisSlot(2,1)", "3nisDrag(1,1,1)", "4nisDrag(2,2)", "4nisSlot(2,1,1)", "4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)", "5nisDrag", "5nisSlot(1,1,1,1,1)","6nisSlot(1,1,1,1,1,1)","6nisSlot(5,1)","6nisDrag(2,2,2)","7nisDrag(2,5)","7nisSlot(2,2,2,1)","7nisSlot(5,1,1)","8nisSlot(2,2,2,2)","8nisDrag(5,1,1,1)","8nisSlot(2,5,1)","9nisSlot(5,2,1,1)","9nisSlot(5,2,2)","10nisDrag(5,5)","10nisDrag","10nisSlot(5,2,2,1)","10nisSlot(2,2,2,2,2)","10nisSlot(5,2,2,1)","12nisSlot(2,5,2,1,1,1)", "12nisSlot(10,2)","18nisSlot(10,2,2,2,2)","18nisSlot(10,5,1,1,1)","20nisSlot(10,10)","20nisSlot"},//20
                                                            new(){ "1nisDrag","2nisSlot(1,1)", "2nisDrag", "3nisSlot(2,1)", "3nisDrag(1,1,1)", "4nisDrag(2,2)", "4nisSlot(2,1,1)", "4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)", "5nisDrag", "5nisSlot(1,1,1,1,1)","6nisSlot(1,1,1,1,1,1)","6nisSlot(5,1)","6nisDrag(2,2,2)","7nisDrag(2,5)","7nisSlot(2,2,2,1)","7nisSlot(5,1,1)","8nisSlot(2,2,2,2)","8nisDrag(5,1,1,1)","8nisSlot(2,5,1)","9nisSlot(5,2,1,1)","9nisSlot(5,2,2)","10nisDrag(5,5)","10nisDrag","10nisSlot(5,2,2,1)","10nisSlot(2,2,2,2,2)","10nisSlot(5,2,2,1)","12nisSlot(2,5,2,1,1,1)", "12nisSlot(10,2)","18nisSlot(10,2,2,2,2)","18nisSlot(10,5,1,1,1)","20nisSlot(10,10)","20nisSlot","25nisSlot(20,5)","25nisSlot(10,10,2,2,1)","30nisSlot(10,10,10)","30nisSlot(20,10)","35nisSlot(10,10,5,5,5)","35nisSlot(20,10,5)" ,"40nisSlot(10,10,5,5,5,5)","40nisSlot(20,20)","45nisSlot(5x9)","45nisSlot(10,10,20,1,1,1,1,1)","50nisDrag","50nisSlot(20,10,10,10)"},//50
                                                            new(){ "1nisDrag","2nisSlot(1,1)", "2nisDrag", "3nisSlot(2,1)", "3nisDrag(1,1,1)", "4nisDrag(2,2)", "4nisSlot(2,1,1)", "4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)", "5nisDrag", "5nisSlot(1,1,1,1,1)","6nisSlot(1,1,1,1,1,1)","6nisSlot(5,1)","6nisDrag(2,2,2)","7nisDrag(2,5)","7nisSlot(2,2,2,1)","7nisSlot(5,1,1)","8nisSlot(2,2,2,2)","8nisDrag(5,1,1,1)","8nisSlot(2,5,1)","9nisSlot(5,2,1,1)","9nisSlot(5,2,2)","10nisDrag(5,5)","10nisDrag","10nisSlot(5,2,2,1)","10nisSlot(2,2,2,2,2)","10nisSlot(5,2,2,1)","12nisSlot(2,5,2,1,1,1)", "12nisSlot(10,2)","18nisSlot(10,2,2,2,2)","18nisSlot(10,5,1,1,1)","20nisSlot(10,10)","20nisSlot","25nisSlot(20,5)","25nisSlot(10,10,2,2,1)","30nisSlot(10,10,10)","30nisSlot(20,10)","35nisSlot(10,10,5,5,5)","35nisSlot(20,10,5)" ,"40nisSlot(10,10,5,5,5,5)","40nisSlot(20,20)","45nisSlot(5x9)","45nisSlot(10,10,20,1,1,1,1,1)","50nisDrag","50nisSlot(20,10,10,10)","55nisSlot(20,10,10,10,5)","55nisSlot(50,5)","70nisSlot(20,10,10,10,10,10)","70nisSlot(20,50)","85nisSlot(20,10,50,5)","85nisSlot(20,20,20,10,10,5)","100nisDrag","100nisSlot(50,50)"},//100
                                                            new(){ "1nisDrag","2nisSlot(1,1)", "2nisDrag", "3nisSlot(2,1)", "3nisDrag(1,1,1)", "4nisDrag(2,2)", "4nisSlot(2,1,1)", "4nisSlot(1,1,1,1)", "5nisDrag(2,2,1)", "5nisSlot(2,1,1,1)", "5nisDrag", "5nisSlot(1,1,1,1,1)","6nisSlot(1,1,1,1,1,1)","6nisSlot(5,1)","6nisDrag(2,2,2)","7nisDrag(2,5)","7nisSlot(2,2,2,1)","7nisSlot(5,1,1)","8nisSlot(2,2,2,2)","8nisDrag(5,1,1,1)","8nisSlot(2,5,1)","9nisSlot(5,2,1,1)","9nisSlot(5,2,2)","10nisDrag(5,5)","10nisDrag","10nisSlot(5,2,2,1)","10nisSlot(2,2,2,2,2)","10nisSlot(5,2,2,1)","12nisSlot(2,5,2,1,1,1)", "12nisSlot(10,2)","18nisSlot(10,2,2,2,2)","18nisSlot(10,5,1,1,1)","20nisSlot(10,10)","20nisSlot","25nisSlot(20,5)","25nisSlot(10,10,2,2,1)","30nisSlot(10,10,10)","30nisSlot(20,10)","35nisSlot(10,10,5,5,5)","35nisSlot(20,10,5)" ,"40nisSlot(10,10,5,5,5,5)","40nisSlot(20,20)","45nisSlot(5x9)","45nisSlot(10,10,20,1,1,1,1,1)","50nisDrag","50nisSlot(20,10,10,10)","55nisSlot(20,10,10,10,5)","55nisSlot(50,5)","70nisSlot(20,10,10,10,10,10)","70nisSlot(20,50)","85nisSlot(20,10,50,5)","85nisSlot(20,20,20,10,10,5)","100nisDrag","100nisSlot(50,50)","120nisSlot(10,10,10,10,10,10,10,50)","120nisSlot(20,20,20,20,20,10,10)","150nisSlot(50,50,50)","150nisSlot(100,10,10,10,10,5,5)","200nisDrag","200nisDrag(100,50,20,20,10)"},//200
        };



        await FirebaseDatabase.DefaultInstance.GetReference("identifyingCoinsBills").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    dbRef.Child("identifyingCoinsBills").Child("1").Child("dragItems").SetValueAsync(identifying_dragItems[0]);
                    dbRef.Child("identifyingCoinsBills").Child("1").Child("slotItems").SetValueAsync(identifying_slotItems[0]);
                    dbRef.Child("identifyingCoinsBills").Child("2").Child("dragItems").SetValueAsync(identifying_dragItems[1]);
                    dbRef.Child("identifyingCoinsBills").Child("2").Child("slotItems").SetValueAsync(identifying_slotItems[1]);
                    dbRef.Child("identifyingCoinsBills").Child("5").Child("dragItems").SetValueAsync(identifying_dragItems[2]);
                    dbRef.Child("identifyingCoinsBills").Child("5").Child("slotItems").SetValueAsync(identifying_slotItems[2]);
                    dbRef.Child("identifyingCoinsBills").Child("10").Child("dragItems").SetValueAsync(identifying_dragItems[2]);
                    dbRef.Child("identifyingCoinsBills").Child("10").Child("slotItems").SetValueAsync(identifying_slotItems[3]);
                    dbRef.Child("identifyingCoinsBills").Child("20").Child("dragItems").SetValueAsync(identifying_dragItems[3]);
                    dbRef.Child("identifyingCoinsBills").Child("20").Child("slotItems").SetValueAsync(identifying_slotItems[4]);
                    dbRef.Child("identifyingCoinsBills").Child("50").Child("dragItems").SetValueAsync(identifying_dragItems[4]);
                    dbRef.Child("identifyingCoinsBills").Child("50").Child("slotItems").SetValueAsync(identifying_slotItems[5]);
                    dbRef.Child("identifyingCoinsBills").Child("100").Child("dragItems").SetValueAsync(identifying_dragItems[5]);
                    dbRef.Child("identifyingCoinsBills").Child("100").Child("slotItems").SetValueAsync(identifying_slotItems[6]);
                    dbRef.Child("identifyingCoinsBills").Child("200").Child("dragItems").SetValueAsync(identifying_dragItems[5]);
                    dbRef.Child("identifyingCoinsBills").Child("200").Child("slotItems").SetValueAsync(identifying_slotItems[7]);
                }
            }

        });
        await FirebaseDatabase.DefaultInstance.GetReference("ProductsToPurchase").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    dbRef.Child("ProductsToPurchase").Child("1").SetValueAsync(productsToPurchase[0]);
                    dbRef.Child("ProductsToPurchase").Child("2").SetValueAsync(productsToPurchase[1]);
                    dbRef.Child("ProductsToPurchase").Child("5").SetValueAsync(productsToPurchase[2]);
                    dbRef.Child("ProductsToPurchase").Child("10").SetValueAsync(productsToPurchase[3]);
                    dbRef.Child("ProductsToPurchase").Child("20").SetValueAsync(productsToPurchase[4]);
                    dbRef.Child("ProductsToPurchase").Child("50").SetValueAsync(productsToPurchase[5]);
                    dbRef.Child("ProductsToPurchase").Child("100").SetValueAsync(productsToPurchase[6]);
                    dbRef.Child("ProductsToPurchase").Child("200").SetValueAsync(productsToPurchase[7]);
                }
            }
        });

        await FirebaseDatabase.DefaultInstance.GetReference("coinsCollections").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    dbRef.Child("coinsCollections").Child("2").SetValueAsync(coinsCollection[0]);
                    dbRef.Child("coinsCollections").Child("5").SetValueAsync(coinsCollection[1]);
                    dbRef.Child("coinsCollections").Child("10").SetValueAsync(coinsCollection[2]);
                    dbRef.Child("coinsCollections").Child("20").SetValueAsync(coinsCollection[3]);
                    dbRef.Child("coinsCollections").Child("50").SetValueAsync(coinsCollection[4]);
                    dbRef.Child("coinsCollections").Child("100").SetValueAsync(coinsCollection[5]);
                    dbRef.Child("coinsCollections").Child("200").SetValueAsync(coinsCollection[6]);

                }
            }

        });
        return true;
    }
    private void changeCellSize(List<List<GameObject>> gmList, GridLayoutGroup gridLayout)
    {
        if (gmList[index] != null)
        {
            if (levelNames[index % levelNames.Count].Substring(0, 4).Equals("Matc") && gmList[index].Count == 2)
            {
                gridLayout.cellSize = new Vector2(80, 80);
                gridLayout.spacing = new Vector2(15, 5);
            }
            else if (levelNames[index % levelNames.Count].Substring(0, 4).Equals("Matc") && gmList[index].Count == 3)
            {
                gridLayout.cellSize = new Vector2(45, 25);
                gridLayout.spacing = new Vector2(15, 5);
            }
            else if (levelNames[index % levelNames.Count].Substring(0, 4).Equals("Matc") && gmList[index].Count == 4) {
                gridLayout.cellSize = new Vector2(35, 25);
                gridLayout.spacing = new Vector2(15, 0);
            }

            else if (gmList[index].Count == 1)
            {
                // no need for spacing only 1 element
                gridLayout.cellSize = new Vector2(100, 120);
            }
            else if (gmList[index].Count == 2)
            {
                //consider spacing
                gridLayout.cellSize = new Vector2(75, 75); //40,40
                gridLayout.spacing = new Vector2(10, 9);
            }
            else if (gmList[index].Count == 3 && !levelNames[index % levelNames.Count].Substring(0, 4).Equals("Matc"))
            {
                gridLayout.cellSize = new Vector2(75, 75);
                gridLayout.spacing = new Vector2(4, 10);

            }
            else if (gmList[index].Count == 4 && !levelNames[index % levelNames.Count].Substring(0, 3).Equals("Mat"))
            {
                gridLayout.cellSize = new Vector2(70, 70); //45,50
                gridLayout.spacing = new Vector2(10, 20);
            }
            else if (gmList[index].Count == 5)
            {
                gridLayout.cellSize = new Vector2(65, 65); //50,45
                gridLayout.spacing = new Vector2(10, 10);
            }
            else if (gmList[index].Count == 6)
            {
                gridLayout.cellSize = new Vector2(65, 55); //50,45
                gridLayout.spacing = new Vector2(10, 4);
            }
            else if (gmList[index].Count == 7)
            {
                gridLayout.cellSize = new Vector2(50, 50);
                gridLayout.spacing = new Vector2(10, 30);
            }
            else if (gmList[index].Count == 8)
            {
                gridLayout.cellSize = new Vector2(50, 45);
                gridLayout.spacing = new Vector2(4, 10);
            }
            else if (gmList[index].Count == 9)
            {
                gridLayout.cellSize = new Vector2(65, 50);//45,45
                gridLayout.spacing = new Vector2(4, 10);
            }
            else if (gmList[index].Count == 10)
            {
                gridLayout.cellSize = new Vector2(40, 45);
                gridLayout.spacing = new Vector2(4, 10);
            }

        }
    }

  
}


