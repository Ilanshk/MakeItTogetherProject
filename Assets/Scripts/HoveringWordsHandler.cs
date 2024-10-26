using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoveringWordsHandler : MonoBehaviour {
    private TextMeshProUGUI _textDescription;
    private RectTransform _instructionsRect;
    private Dictionary<string, string> _vocabInfo = new Dictionary<string, string>(){
        { "הזול", "המחיר הנמוך" },
        {"היקר","המחיר הגבוה" },
        {"עודף","  כסף אותו מחזיר המוכר לקונה שנתן סכום גבוה יותר מסכום הקניה או ערך המוצרים אותם רכש " }
    };
    
    [SerializeField] private GameObject _tooltipContainer;
    void Start()
    {
        _instructionsRect = GetComponent<RectTransform>();
        _textDescription = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWordsToDefine();
    }

    void CheckForWordsToDefine() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        bool isIntersectingWithWord = TMP_TextUtilities.IsIntersectingRectTransform(_instructionsRect, mousePos,null);
        if (!isIntersectingWithWord) { return; }
        var linkCode = TMP_TextUtilities.FindIntersectingLink(_textDescription, mousePos, null);
        if (linkCode == -1) { _tooltipContainer.SetActive(false); return; }
        var link = _textDescription.textInfo.linkInfo[linkCode];
        Debug.Log("AAA");
        Debug.Log(link.GetLinkText().Length);
        Findinfo(link.GetLinkText().Substring(0, link.GetLinkText().Length - 1), mousePos);
    }

    void Findinfo(string word, Vector3 mousePos)
    {
        if (word == null) { return; }
        Debug.Log("word: " + word);
        
        //char[] wordArr = word.ToCharArray();
        //Array.Reverse(wordArr);
        //string rtlWord = new string(wordArr);
        
        _tooltipContainer.SetActive(true);
        _tooltipContainer.GetComponentInChildren<TextMeshProUGUI>().text = _vocabInfo[word]; //rtlword
        if(mousePos.y > -(1080/2))
            _tooltipContainer.transform.position = mousePos + new Vector3(0,-200,0);
        else
            _tooltipContainer.transform.position = mousePos - new Vector3(0, -200, 0);
        
    }
}
