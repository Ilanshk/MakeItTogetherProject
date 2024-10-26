using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler 
{
    public GameObject canvas;
    public GameObject initialParent;
    public int value;
    private bool switcher = false;
    private string levelName = "";
    private GameObject line;
    public GameObject linePrefab;
    public float scalerX = 1;
    public float scalerY = 3.5f;
    public bool interactable = false;
    public bool IsCoin = true;
    public string Name;


    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector2 pos;
    private void Start()
    {
		if(SceneManager.GetActiveScene().name == "GreenScene")
            levelName = GameObject.Find("ExerciseCanva").GetComponent<MakeLevelSimulationMultiplayer>().GetLevelName();
		else
			levelName = GameObject.Find("ExerciseCanva").GetComponent<MakeLevel>().GetLevelName();
        if (this.gameObject.name == "OnDropDetector")
        {
            canvas = GameObject.Find("Task2");

        }
        else if (levelName == "FTask")
        {
            canvas = GameObject.Find("Task2");
            initialParent = GameObject.Find("CoinsLayout");
        }
        else if (levelName.Substring(0, 4) == "Word")
        {
            canvas = GameObject.Find("WordProblems");
            initialParent = GameObject.Find("ProductsToDrag");
        }
        else if (levelName.Substring(0, 4) == "Comp" || levelName.Substring(0, 5) == "Coins")
        {
            canvas = GameObject.Find("ComparingPrices");
            //initialParent = this.transform.parent.gameObject;
            initialParent = canvas;
            Debug.Log("Comp : " + initialParent);
        }
        else if (levelName != "FTask")
        {
            canvas = GameObject.Find("Task1");
            initialParent = GameObject.Find("LeftGrid");
        }
        if (levelName == "MatchingCoins")
            linePrefab = GameObject.Find("LineDraw");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (levelName != "1TaskCM" && levelName.Substring(0,4) != "Comp" && levelName.Substring(0, 5) !="Coins" && levelName != "MatchingCoins" && interactable == true)
        {
            Debug.Log("OnBeginDrag");
            canvasGroup.blocksRaycasts = false;
            this.transform.SetParent(canvas.transform, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (levelName != "1TaskCM" && levelName != "ComparingPricesLowest" && levelName != "ComparingPricesHighest" && levelName!="CoinsComparingLowest" && levelName!="CoinsComparingHighest" && levelName != "MatchingCoins" && interactable == true)
        { 
            rectTransform.anchoredPosition += eventData.delta / canvas.GetComponent<Canvas>().scaleFactor;
        }
        else if (levelName == "MatchingCoins")
        {
            UpdateLine(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        if (levelName != "MatchingCoins" && interactable == true)
        {
            canvasGroup.blocksRaycasts = true;
            eventData.pointerDrag.transform.SetParent(initialParent.transform, true);
        }
        else 
        {
            Destroy(line);
        }
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (levelName == "MatchingCoins" && interactable == true)
            GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, null, this, line);
        else if ((levelName == "FTask" || levelName == "2TaskCM") && SceneManager.GetActiveScene().name == "GreenScene")
            GameObject.Find("ExerciseCanva").GetComponent<Game_Manager_Canvas>().GameManagerCheck(eventData, null, this, null);
        else if (levelName == "10TaskCM") {
            Debug.Log("Drag--->10TaskCM");
            Drag rightSideItem = GameObject.Find("RightGrid").transform.GetChild(0).GetComponent<Drag>();
            GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, null, rightSideItem, null);
        }
        else
            GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, null, this, null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        Debug.Log(eventData);
        if ((levelName == "1TaskCM" || levelName == "8TaskCM") && interactable == true)
            GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, null, null, null);
        else if(levelName == "MatchingCoins" && interactable == true)
        {
            if (line != null)
            {
                Destroy(line.gameObject.transform);
                line = null;
            }

            Debug.Log("OnPointerDown");
            line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform.parent.parent.parent);
            UpdateLine(rectTransform.position);
        }
        else if ((levelName.Substring(0, 4) == "Comp" || levelName.Substring(0,5) == "Coins") && interactable == true)
            GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, null, null, null);
    }


    public void SetSwitcher(bool sw)
    {
        this.switcher = sw;
    }

    public bool GetSwitcher()
    {
        return this.switcher;
    }

    /*public void OnPointerUp(PointerEventData eventData) {
        if (levelName == "MatchingCoins") {
            if (eventData.lastPress.GetComponent<Drag>().value == this.GetComponent<Drag>().value)
            {
                UpdateLine(this.transform.position);
            }
        
        }
    
    
    }*/

    public void UpdateLine(Vector3 position)
    {
        Debug.Log("UpdateLine");
        if (line != null && interactable == true)
        {
            Vector3 direction = position - transform.position;
            line.transform.up = direction;
            line.transform.localScale = new Vector3(1, direction.magnitude * 10f / canvas.GetComponent<RectTransform>().sizeDelta.y, 1);
            line.transform.position = (position + transform.position) / 2;

        }

    }

    public GameObject GetLine() { return line; }




}
