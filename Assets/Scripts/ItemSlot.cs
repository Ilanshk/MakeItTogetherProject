using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public int value;

    private void Start()
    {
        Debug.Log("ItemSlot");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject.Find("GameManager").GetComponent<Game_Manager>().GameManagerCheck(eventData, this, null, null);
    }





}
