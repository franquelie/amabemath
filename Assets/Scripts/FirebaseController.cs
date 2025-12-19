using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting; // Required for Unity Visual Scripting integration
using Firebase.Extensions;

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, notificationPanel, credentials, characterCreate;
    public GameObject gameMap, player, profilePanel, nonPlayerCharacter;
    
    public InputField studentIdField, passwordField, desiredNameField;
    public Button loginButton, closeButton, createButton;

    public Text errorMessage, playerNickname;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    public DatabaseReference DBreference;

    bool isSignedIn = false;

    void Start()
    {
        loginPanel.SetActive(true);
        GameController.Instance.SetState(GameState.Login);

        InitializeFirebase();
        InitializeDatabase();

        if (loginButton != null)
        {
            loginButton.onClick.AddListener(() => LoginUser(null, null));
        }
    }

    public async void LoginUser(string studentId, string password)
    {
        studentId = studentIdField.text;
        password = passwordField.text;

        if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Please enter both Student ID and Password.");
            Exception exception = new Exception("Missing credentials");
            Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
            if (firebaseEx != null)
            {
                var errorCode = (AuthError)firebaseEx.ErrorCode;
                ShowNotificationMessage(GetErrorMessage(errorCode));
            }
            return;
        }

        // Initiate login process with Firebase Authentication and handle result on main thread
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
      
        // Ensure the database reference is initialized with an explicit Database URL
        if (DBreference == null)
        {
            InitializeDatabase();
        }

        auth.SignInWithEmailAndPasswordAsync(studentId, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                
                // TODO: show error to user via UI
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage(GetErrorMessage(errorCode));
                    }
                }

                return;
            }

            var result = task.Result;
            var signedInUser = result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                signedInUser.DisplayName, signedInUser.UserId);

            // Read ischaractercreated from the DB on the main thread and proceed accordingly.
            DBreference.Child("users").Child(signedInUser.UserId).Child("ischaractercreated")
                .GetValueAsync()
                .ContinueWithOnMainThread(getTask =>
            {
                bool isCharacterCreated = false;

                if (getTask.Exception != null)
                {
                    Debug.LogWarning("Failed to read ischaractercreated: " + getTask.Exception);
                }
                else
                {
                    var snapshot = getTask.Result;
                    if (snapshot.Exists && snapshot.Value != null)
                    {
                        var raw = snapshot.Value;
                        bool parsed = false;
                        if (raw is bool b) parsed = b;
                        else if (raw is long l) parsed = (l != 0);
                        else if (raw is int i) parsed = (i != 0);
                        else
                        {
                            var s = raw.ToString().Trim();
                            if (int.TryParse(s, out var n)) parsed = (n != 0);
                            else parsed = string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
                        }
                        isCharacterCreated = parsed;
                    }
                    else
                    {
                        isCharacterCreated = false; // default when node missing
                    }
                }

                // Activate UI depending on ischaractercreated value
                if (!isCharacterCreated)
                {
                    credentials.SetActive(false);
                    characterCreate.SetActive(true);

                    if (createButton != null)
                    {
                        createButton.onClick.RemoveListener(CreateCharacter);
                        createButton.onClick.AddListener(CreateCharacter);
                    }            
                }
                else
                {
                    loginPanel.SetActive(false);
                    gameMap.SetActive(true);
                    player.SetActive(true);
                    GameController.Instance.SetState(GameState.FreeRoam);
                    nonPlayerCharacter.SetActive(true);

                    // Read nickname from the DB on the main thread and proceed accordingly.
                    DBreference.Child("users").Child(signedInUser.UserId).Child("nickname")
                        .GetValueAsync()
                        .ContinueWithOnMainThread(getTask =>
                    {
                        // Default fallback is the student ID
                        string displayName = studentId;

                        if (getTask.Exception != null)
                        {
                            Debug.LogWarning("Failed to read nickname: " + getTask.Exception);
                        }
                        else
                        {
                            var snapshot = getTask.Result;
                            if (snapshot.Exists && snapshot.Value != null)
                            {
                                try
                                {
                                    // Safely convert the stored value to string and trim whitespace
                                    var raw = snapshot.Value;
                                    var name = raw.ToString().Trim();
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        displayName = name;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogWarning("Error parsing nickname: " + ex);
                                    displayName = studentId;
                                }
                            }
                            else
                            {
                                // Node missing => keep fallback
                                displayName = studentId;
                            }
                        }

                        // Update UI with the resolved display name
                        playerNickname.text = displayName;
                        profilePanel.SetActive(true);
                        isSignedIn = true;
                        user = signedInUser;  

                    });   
                }
            }); 
        });
    }

    void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void InitializeDatabase()
    {
        try
        {
            // Replace with your Firebase Realtime Database URL (found in the Firebase Console)
            string databaseUrl = "https://amabe-math-default-rtdb.asia-southeast1.firebasedatabase.app/";
            DBreference = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, databaseUrl).RootReference;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to get FirebaseDatabase instance: " + ex.Message + " — make sure the Database URL is set in your Firebase config or replace the placeholder.");
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
                isSignedIn = true;
            }
        }
    }

    void OnDestroy() {
    auth.StateChanged -= AuthStateChanged;
    auth = null;
    }

    void UpdateUserProfile() // For future updating of name, gender, etc.
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null) {
        Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile {
            DisplayName = "John Q. User",
        };
        user.UpdateUserProfileAsync(profile).ContinueWith(task => {
            if (task.IsCanceled) {
            Debug.LogError("UpdateUserProfileAsync was canceled.");
            return;
            }
            if (task.IsFaulted) {
            Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
            return;
            }

            Debug.Log("User profile updated successfully.");
        });
        }       
    }

    private void ShowNotificationMessage(string message)
    {
        errorMessage.text = message;
        notificationPanel.SetActive(true);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseNotificationMessage);
        }
        
    }

    private void CloseNotificationMessage()
    {
        notificationPanel.SetActive(false);
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                message = "Unregistered Student ID. Please make sure you type correctly.";
                break;
            default:
                message = "Incorrect Student ID or Password. Please try again.";
                break;
        }
        return message;
    }

    private void CreateCharacter()
    {
        string displayName = desiredNameField.text;

        StartCoroutine(UpdateDisplayName(displayName));
        StartCoroutine(UpdateIsCreated(true));
        StartCoroutine(UpdateExperience(0));
        StartCoroutine(UpdateIntelligence(0));

        characterCreate.SetActive(false);
        loginPanel.SetActive(false);
        gameMap.SetActive(true);
        player.SetActive(true);
        GameController.Instance.SetState(GameState.FreeRoam);
        nonPlayerCharacter.SetActive(true);

        playerNickname.text = displayName;
        profilePanel.SetActive(true);
    }

    private IEnumerator UpdateDisplayName(string _nickname)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("nickname").SetValueAsync(_nickname);
        yield return new WaitUntil(() => DBTask.IsCompleted);
    }

    private IEnumerator UpdateExperience(int _experience)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("experience").SetValueAsync(_experience);
        yield return new WaitUntil(() => DBTask.IsCompleted);
    }

    private IEnumerator UpdateIntelligence(int _intelligence)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("int").SetValueAsync(_intelligence);
        yield return new WaitUntil(() => DBTask.IsCompleted);
    }

    private IEnumerator UpdateIsCreated(bool _isCreated)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("ischaractercreated").SetValueAsync(_isCreated);
        yield return new WaitUntil(() => DBTask.IsCompleted);
    }


   

}
