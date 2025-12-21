using System;
using System.Collections; // using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class QuestionManager : MonoBehaviour
{
    public Text questionText; 
    public Text scoreText;
    public Text FinalScoreText;
    public Button[] choiceButtons;
    public QtsData qtsData; // Reference to the scriptable object containing questions
    public GameObject Correct, Incorrect;
    public GameObject Reward, Consolation;
    public GameObject GameFinished;

    private int currentQuestion = 0;
    private int score = 0;
    // Firebase
    private FirebaseAuth auth;
    private DatabaseReference dbRootRef;
    private const string usersNode = "users";

    public void StartQuiz() // Start()
    {
        SetQuestion(currentQuestion);
        Correct.gameObject.SetActive(false);
        Incorrect.gameObject.SetActive(false);
        GameFinished.gameObject.SetActive(false);
        // Initialize Firebase references
        auth = FirebaseAuth.DefaultInstance;
        dbRootRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void SetQuestion(int questionIndex)
    {
        questionText.text = qtsData.questions[questionIndex].questionText;


        // Remove previous listeners before adding new ones
        foreach (Button c in choiceButtons)
        {
            c.onClick.RemoveAllListeners();
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].GetComponentInChildren<Text>().text = qtsData.questions[questionIndex].choices[i];
            int choiceIndex = i;
            choiceButtons[i].onClick.AddListener(() =>
            {
                CheckChoice(choiceIndex);
            });
        }
    }

    void CheckChoice(int choiceIndex)
    {
        if (choiceIndex == qtsData.questions[currentQuestion].correctChoiceIndex)
        {
            // Enable Correct answer notification and reward
            Correct.gameObject.SetActive(true);
            Reward.gameObject.SetActive(true);

            /* score++;
            scoreText.text = "" + score; */

            IncreaseUserIntelligence();
            IncreaseUserExperience();
            
        }
        else
        {
            // Enable Incorrect answer notification and consolation
            Incorrect.gameObject.SetActive(true);
            Consolation.gameObject.SetActive(true);
            IncreaseUserExperience();
        } 

        
        // Set Active false all reply buttons
        foreach (Button c in choiceButtons)
        {
            c.interactable = false;
        }

        // Next Question
        StartCoroutine(Next());
    }

    // Read character's intelligence value from the DB and increase it by 1.
    void IncreaseUserIntelligence() 
    {
        // Prefer the signed-in user stored by FirebaseController to avoid null auth timing issues
        var user = FirebaseController.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No Firebase user signed in — intelligence not updated.");
            return;
        }

        string uid = user.UserId;
        var dbRef = FirebaseController.DBreference ?? FirebaseDatabase.DefaultInstance.RootReference;
        var intelRef = dbRef.Child(usersNode).Child(uid).Child("int");

        intelRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogWarning("Failed to read intelligence: " + task.Exception);
                return;
            }

            if (!task.IsCompleted)
                return;

            long currentValue = 0;
            var snapshot = task.Result;
            if (snapshot != null && snapshot.Exists)
            {
                long.TryParse(snapshot.Value.ToString(), out currentValue);
            }

            long newValue = currentValue + 1;
            intelRef.SetValueAsync(newValue).ContinueWithOnMainThread(setTask =>
            {
                if (setTask.IsCompleted)
                    Debug.Log("Intelligence updated to " + newValue);
                else
                    Debug.LogWarning("Failed to set intelligence: " + setTask.Exception);
            });
        });
    }

    // Read character's experience value from the DB and increase it by 20.
    void IncreaseUserExperience()
    {
        // Prefer the signed-in user stored by FirebaseController to avoid null auth timing issues
        var user = FirebaseController.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No Firebase user signed in — intelligence not updated.");
            return;
        }

        string uid = user.UserId;
        var dbRef = FirebaseController.DBreference ?? FirebaseDatabase.DefaultInstance.RootReference;
        var intelRef = dbRef.Child(usersNode).Child(uid).Child("experience");

        intelRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogWarning("Failed to read experience: " + task.Exception);
                return;
            }

            if (!task.IsCompleted)
                return;

            long currentValue = 0;
            var snapshot = task.Result;
            if (snapshot != null && snapshot.Exists)
            {
                long.TryParse(snapshot.Value.ToString(), out currentValue);
            }

            long newValue = currentValue + 20;
            intelRef.SetValueAsync(newValue).ContinueWithOnMainThread(setTask =>
            {
                if (setTask.IsCompleted)
                    Debug.Log("Experience updated to " + newValue);
                else
                    Debug.LogWarning("Failed to set experience: " + setTask.Exception);
            });
        });
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(2);

        currentQuestion++;

        if (currentQuestion < qtsData.questions.Length)
        {
            // Reset the UI and enable all reply buttons
            Reset();

            // Set the next question
            SetQuestion(currentQuestion);
        }
        else
        {
            // Game over
            GameFinished.SetActive(true);

            // Calculate the score percentage
            float scorePercentage = (float)score / qtsData.questions.Length * 100;

            // Display the score percentage
            FinalScoreText.text = "You scored " + scorePercentage.ToString("F0") + "%";

            // Display the appropriate message based on the score percentage
            if (scorePercentage < 50)
            {
                FinalScoreText.text += "\nGame Over";
            }
            else if (scorePercentage < 60)
            {
                FinalScoreText.text += "\nKeep Trying!";
            }
            else if (scorePercentage < 70)
            {
                FinalScoreText.text += "\nGood Job!";
            }
            else if (scorePercentage < 80)
            {
                FinalScoreText.text += "\nWell Done!";
            }
            else if (scorePercentage == 100)
            {
                FinalScoreText.text += "\nPerfect!";
            }
            else
            {
                FinalScoreText.text += "\nYou're a genius!";
            }
        }
    }   
    


    public void Reset()
    {
        // Reset the UI elements
        questionText.text = "";
        /* scoreText.text = "" + score; */
        Correct.gameObject.SetActive(false);
        Reward.gameObject.SetActive(false);
        Incorrect.gameObject.SetActive(false);
        Consolation.gameObject.SetActive(false);

        // Enable all reply buttons
        foreach (Button c in choiceButtons)
        {
            c.interactable = true;
        }
    }
}
