using UnityEngine;

[CreateAssetMenu(fileName = "New QuestionData", menuName = "QuestionData")]
public class QtsData : ScriptableObject
{
    [System.Serializable]
    public struct Question
    {
        public string questionText; // Store questions
        public string[] choices; // Store choices
        public int correctChoiceIndex; // Store the correct reply index
        public string explanation; // Store the explanation for the answer
    }

    public Question[] questions;
}
