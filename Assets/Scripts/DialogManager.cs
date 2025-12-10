using System;
using System.Collections;
using System.Collections.Generic; 
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject enterQuizMode;

    [SerializeField] Button takeQuizButton;
    [SerializeField] GameObject multipleChoice;
    
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;
    public static DialogManager Instance { get; private set; }
    [SerializeField] private QuestionManager questionManager;

    private void Awake()
    {
        Instance = this;
        takeQuizButton.onClick.AddListener(TakeQuiz);
    }

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count - 1)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else if (currentLine == dialog.Lines.Count - 1)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                // yield return new WaitForSeconds(2f);
                enterQuizMode.SetActive(true);
            }
            else
            {
                /* dialogBox.SetActive(false);
                currentLine = 0;
                OnHideDialog?.Invoke(); */
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    public void TakeQuiz()
    {
        enterQuizMode.SetActive(false);
        multipleChoice.SetActive(true); // Ideally appears after Question 1 appears
        currentLine = 0;

        questionManager.StartQuiz(); // Change dialog box text to Question 1
    }
}
