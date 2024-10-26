using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Linq;


public class MakeLevelSimulationMultiplayer : MonoBehaviour
{
    private List<string> _levelNames = new() { };
    private StudentScript _studentData;
    public List<GameObject> dragItemList = new() { };
    public List<GameObject> itemSlotList = new() { };
    public List<int> maxValues = new() { };
    public List<List<string>> questions = new() { };
    public GridLayoutGroup _leftGrid;
    public GridLayoutGroup _rightGrid;
    public HorizontalLayoutGroup _horLayGroup;
    public GameObject productsToDrag;
    private Slider _slider;

    private int _index = 0;
    List<string> identifying_dragItems;

    // Start is called before the first frame update

    private void Awake()
    {
        identifying_dragItems = new() { "1nisDrag", "2nisDrag", "5nisDrag", "10nisDrag", "20nisDrag", "50nisDrag", "100nisDrag", "200nisDrag" };
    }
    void Start()
    {
        _studentData = GameObject.Find("StudentData").GetComponent<StudentScript>();
        _levelNames.AddRange(new string[] {"2TaskCM", "FTask" });
        for (int i = 0; i < identifying_dragItems.Count; i++)
        {
            if (identifying_dragItems[i].Contains(_studentData.IdentifyingCoinsBiils.ToString()))
            {
                for (int j = 0; j < i + 1; j++)
                    dragItemList.Add(Resources.Load<GameObject>($"Prefabs/{identifying_dragItems[j]}"));
                break;
            }
        }
        GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
        GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
        _slider = GameObject.Find("progressBar").GetComponent<Slider>();
        MakeNewLevel("2TaskCM");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeNewLevel(string levelName)
    {
        changeCellSize(dragItemList, _leftGrid);
        changeCellSize(itemSlotList, _rightGrid);
        if (string.Equals(levelName, "2TaskCM") == true)
        {
            GameObject.Find("ExerciseInstructions").GetComponent<TextMeshProUGUI>().text = "בבקשה, תשלמ/י על ההזמנה";

            for (int i = 0; i < dragItemList.Count; i++)
            {
                GameObject temp = Instantiate(dragItemList[i], _leftGrid.transform);
                temp.GetComponent<Drag>().interactable = true;
            }
            Instantiate(Resources.Load<GameObject>("Prefabs/CashierCase"), _rightGrid.transform);
        }
        else if (string.Equals(levelName, "FTask") == true)
        {
            for (int i = 0; i < dragItemList.Count; i++) {
                GameObject temp1 = Instantiate(dragItemList[i], _horLayGroup.transform);
                temp1.GetComponent<Drag>().interactable = true;
            }
        }
        

    }


    public string GetLevelName()
    {
        try
        {
            return _levelNames[_index % _levelNames.Count];
        }
        catch { return "0000"; }
    }

    public void MoveToNextLevel()
    {
        foreach(Transform itemD in _leftGrid.transform)
        {
            Destroy(itemD.gameObject);
        }
        foreach (Transform itemS in _rightGrid.transform)
        {
            Destroy(itemS.gameObject);
        }
        foreach(Transform coin in GameObject.Find("GridForCoins").transform) 
        { 
            Destroy(coin.gameObject); 
        }
        foreach (Transform coin in _horLayGroup.transform)
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



        _slider.value = 0;
        GameObject.Find("Counter").GetComponent<TextMeshProUGUI>().text = _slider.value.ToString() + "/" + _studentData.RemainAmount.ToString();

        //GameObject.Find("VictMessage").GetComponent<TextMeshProUGUI>().enabled = false;
        Color tempColor = GameObject.Find("SepLine").GetComponent<Image>().color;
        tempColor.a = 1;
        GameObject.Find("SepLine").GetComponent<Image>().color = tempColor;

        if (string.Equals(_levelNames[(_index + 1) % _levelNames.Count], "FTask") == true)
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 1;
        }
        else if (string.Equals(_levelNames[(_index + 1) % _levelNames.Count], "2Task"))
        {
            GameObject.Find("Task2").GetComponent<Canvas>().sortingOrder = 1;
            GameObject.Find("Task1").GetComponent<Canvas>().sortingOrder = 2;
        }

        _index++;
        MakeNewLevel(_levelNames[_index % _levelNames.Count]);
    }

    

    private void changeCellSize(List<GameObject> gmList, GridLayoutGroup gridLayout)
    {
        if (gmList != null)
        {

            if (gmList.Count == 1)
            {
                // no need for spacing only 1 element
                gridLayout.cellSize = new Vector2(50, 50);
            }
            else if (gmList.Count == 2)
            {
                //consider spacing
                gridLayout.cellSize = new Vector2(40, 40);
                gridLayout.spacing = new Vector2(10, 9);
            }
            else if (gmList.Count == 3)
            {
                gridLayout.cellSize = new Vector2(30, 30);
                gridLayout.spacing = new Vector2(5, 9);
            }
            else if (gmList.Count == 4)
            {
                gridLayout.cellSize = new Vector2(25, 25);
                gridLayout.spacing = new Vector2(10, 20);
            }
            else if(gmList.Count == 5)
            {
                gridLayout.cellSize = new Vector2(25, 25);
                gridLayout.spacing = new Vector2(10, 40);
            }
            else if (gmList.Count == 6)
            {
                gridLayout.cellSize = new Vector2(25, 25);
                gridLayout.spacing = new Vector2(10, 40);
            }
            else if (gmList.Count == 7)
            {
                gridLayout.cellSize = new Vector2(25, 25);
                gridLayout.spacing = new Vector2(10, 30);
            }
        }
    }

  
}
