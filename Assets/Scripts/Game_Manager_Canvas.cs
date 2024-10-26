using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Game_Manager_Canvas : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Slider slider;
    private MakeLevelSimulationMultiplayer makeLevel;
    private StudentScript student = null;
    private string currId = null;
    private ItemSlot itemSlot = null;
    private Drag itemDrag = null;
    private Sprite mark = null;
    bool switcher = false;

    private void Start()
    {
        makeLevel = GameObject.Find("ExerciseCanva").GetComponent<MakeLevelSimulationMultiplayer>();
        //message.GetComponent<TextMeshProUGUI>().enabled = false;
        slider.value = 0;
    }

    public void GameManagerCheck(PointerEventData eventData, ItemSlot iSlot, Drag iDrag, GameObject line)
    {
        if (iSlot != null)
            itemSlot = iSlot;
        if (iDrag != null)
            itemDrag = iDrag;
        if (GameObject.Find("ExerciseCanva").activeInHierarchy == true)
        {
            if (makeLevel.GetLevelName() == "2TaskCM")
            {
                if (eventData.pointerDrag != null && itemDrag != null)
                {
                    int val = eventData.pointerDrag.GetComponent<Drag>().value;
                    Debug.Log("Coin is collected");
                    this.GetComponent<StudentCanvaManager>().AddNumberClientRpc(val);
                }
                if (eventData.pointerDrag != null)
                    eventData.pointerDrag.transform.SetParent(GameObject.Find("LeftGrid").transform, true);
            }
            else if (makeLevel.GetLevelName() == "FTask" && (SceneManager.GetActiveScene().name == "SampleScene" || SceneManager.GetActiveScene().name == "GreenScene"))
            {
                if (eventData.pointerDrag != null && itemDrag != null)
                {
                    this.GetComponent<StudentCanvaManager>().AddNumberClientRpc(eventData.pointerDrag.GetComponent<Drag>().value);
                    if (this.GetComponent<StudentCanvaManager>().ProgressValue <= GameObject.Find("StudentData").GetComponent<StudentScript>().RemainAmount)
                    {
                        GameObject temp = Instantiate(eventData.pointerDrag, GameObject.Find("GridForCoins").transform);
                        temp.GetComponent<Drag>().interactable = false;
                    }
                    else
                    {
                        this.GetComponent<StudentCanvaManager>().ProgressValue -= eventData.pointerDrag.GetComponent<Drag>().value;
                    }

                }
                eventData.pointerDrag.transform.SetParent(makeLevel._horLayGroup.transform, true);
            }
        }

        itemSlot = null;
        itemDrag = null;
    }
}
