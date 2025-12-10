using System;
using System.Collections; // using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText; 
    public Text scoreText;
    public Text FinalScoreText;
    public Button[] choiceButtons;
    public QtsData qtsData; // Reference to the scriptable object containing questions
    public GameObject Correct;
    public GameObject Incorrect;
    public GameObject GameFinished;

    private int currentQuestion = 0;
    private int score = 0;

    public void StartQuiz() // Start()
    {
        SetQuestion(currentQuestion);
        Correct.gameObject.SetActive(false);
        Incorrect.gameObject.SetActive(false);
        GameFinished.gameObject.SetActive(false);
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
            // Enable Correct reply panel
            Correct.gameObject.SetActive(true);

            /* score++;
            scoreText.text = "" + score; */
        }
        else
        {
            // Enable Incorrect reply panel
            Incorrect.gameObject.SetActive(true);
        } 

        
        // Set Active false all reply buttons
        foreach (Button c in choiceButtons)
        {
            c.interactable = false;
        }

        // Next Question
        StartCoroutine(Next());
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
        Incorrect.gameObject.SetActive(false);

        // Enable all reply buttons
        foreach (Button c in choiceButtons)
        {
            c.interactable = true;
        }
    }
}
