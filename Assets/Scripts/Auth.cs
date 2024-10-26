using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using UnityEditor;

public class Auth : MonoBehaviour
{
    [SerializeField] private TMP_InputField studentCode;
    [SerializeField] private TMP_InputField studentGroup;
    [SerializeField] private StudentScript student;
    [SerializeField] private Game_Manager gameManager;
    [SerializeField] private Firebase.Auth.FirebaseAuth auth;
    DatabaseReference dbRef;
    // Start is called before the first frame update
    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        
    }

    // Update is called once per frame
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnSignIn);
        //auth.StateChanged += HandleAuthChange;
        //Debug.Log("Current user: " + auth.CurrentUser.UserId);

    }

    

    public async void OnSignIn() {
        string currentStudentCode = studentCode.text;
        string email = currentStudentCode + "@gmail.com";
        student.SetStudentCode(currentStudentCode.ToLower());
        student.SetStudentGroupName(studentGroup.text);
        if (auth.CurrentUser != null) {
            auth.SignOut();
        }
        await auth.SignInWithEmailAndPasswordAsync(email, currentStudentCode).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                auth.CreateUserWithEmailAndPasswordAsync(email, currentStudentCode).ContinueWithOnMainThread(async task => {
                    if (task.IsCompletedSuccessfully)
                    {
                        Dictionary<string, string> studentData = new Dictionary<string, string>() { { "id", currentStudentCode }, { "group", studentGroup.text } };
                        await dbRef.Child("Students").Child(currentStudentCode.ToLower()).SetValueAsync(studentData);
                        Debug.Log(auth.CurrentUser.UserId);
                        //studentLearningDataForm.SetActive(true);
                        student.SetChange(0);
                        student.SetCheapExpensive(0);
                        student.SetIdentifyingCoinsBills(0);
                        student.SetUnderstandingValue(0);
                        //student.SetStudentCode(currentStudentCode);
                        student.SetIsNewStudent(true);
                        //student.SetStudentGroupName(studentGroup.text);
                        Dictionary<string, int> computerParams = new Dictionary<string, int>();
                        computerParams.Add("_identifyingCoinsBills", 0);
                        computerParams.Add("_understandingValue", 0);
                        computerParams.Add("_changeCalc", 0);
                        computerParams.Add("_cheapExpensive", 0);
                        await dbRef.Child("ComputerParams").Child(currentStudentCode.ToLower()).SetValueAsync(computerParams);
                    }
                });
            }
        });
    await gameManager.PresentStudentData();
    }

}
