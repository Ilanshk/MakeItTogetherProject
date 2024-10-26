using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static Game.Events.ToggleEvents;

public class ToggleController : MonoBehaviour

{
    private bool toggleState = false;
    [SerializeField] private string category;
    [SerializeField] private StudentScript student;
    [SerializeField] private int value;
    [SerializeField] private bool isTrueFalseValueOnly;
    [SerializeField] private bool isSubCategoryTrueFalseValue;
    [SerializeField] private bool isNumerical;
    [SerializeField] private int isYesOrNoAnswer;


    private void Start()
    {
        setToggle += InitToggle;
    }

    public bool GetToggleState() { return toggleState; }
    public void SetToggle() {
        this.toggleState = this.GetComponent<Toggle>().isOn;
        if ((this.isSubCategoryTrueFalseValue || this.isTrueFalseValueOnly) && toggleState)
        {
            if (this.category == "_recognizeCoins")
            {
                this.student.SetRecognizeCoins(value);
            }
            else if (this.category == "_recognizeBills")
            {
                this.student.SetRecognizeBills(value);
            }
            else if (this.category == "_matchBillToValue")
            {
                this.student.SetMatchBillToValue(value);
            }
            else if (this.category == "_matchCoinToValue")
            {
                this.student.SetMatchCoinToValue(value);
            }
            else if (this.category == "_chooseProductMatchingToOwnMoney")
            {
                this.student.SetchooseProductMatchingToOwnMoney(this);
            }
            else if (this.category == "_recognizeSmallestOrBiggestNumber")
            {
                this.student.SetRecognizeSmallestOrBiggestNumber(this);
            }
            else if (this.category == "_comparingOwnSumToProductPrice")
            {
                this.student.SetComparingOwnSumToProductPrice(this);
            }
            else if (this.category == "_distinguishCheapExpensive")
            {
                this.student.SetDistinguishCheapExpensive(this);
            }
            else if (this.category == "_recognizeChangeGet")
            {
                this.student.SetRecognizeChangeGet(this);
            }
            else if (this.category == "_recognizeNoChange")
            {
                this.student.SetRecognizeNoChange(this);
            }
            if (this.student.StudentParams.ContainsKey(this.category))
            {
                this.student.StudentParams[this.category] = value;
            }
            else
            {
                this.student.StudentParams.Add(this.category, value);
            }
        }
        else
        {
            if (this.student.StudentParams.ContainsKey(this.category))
            {
                this.student.StudentParams[this.category] = toggleState ? value : 0;
            }
            else
            {
                this.student.StudentParams.Add(this.category, value);
            }
        }
    }

    public void InitToggle(String category, int value, int state) {
        if (this.category == category)
        {
            if ((this.isSubCategoryTrueFalseValue || this.isNumerical) && value == this.value)
            {
                this.GetComponent<Toggle>().isOn = state != 0;
            }
            else if (this.isTrueFalseValueOnly) {
                this.GetComponent<Toggle>().isOn =  (this.isYesOrNoAnswer == 0 && state == 0) || (this.isYesOrNoAnswer == 1 && state == 1);
                // this.isYesOrNoAnswer != 0 ? state != 0 : state == 0;
            }
        }
        
    }
}
