using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting; // Required for Unity Visual Scripting integration
using Firebase.Extensions;

public class LoginController : MonoBehaviour
{
    public GameObject loginPanel, notificationPanel;
    public GameObject gameMap, player, profilePanel, nonPlayerCharacter;
    
    public InputField studentIdField, PasswordField;
    public Button loginButton, closeButton;

    public Text errorMessage, playerNickname;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignedIn = false;

    void Start()
    {
        loginPanel.SetActive(true);
        GameController.Instance.SetState(GameState.Login);

        InitializeFirebase();
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(() => LoginUser(null, null));
        }
    }

    public void LoginUser(string studentId, string password)
    {
        studentId = studentIdField.text;
        password = PasswordField.text;

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

            // Update UI now that sign-in succeeded
            loginPanel.SetActive(false);
            gameMap.SetActive(true);
            player.SetActive(true);
            GameController.Instance.SetState(GameState.FreeRoam);
            nonPlayerCharacter.SetActive(true);

            playerNickname.text = !string.IsNullOrEmpty(signedInUser.DisplayName)
                ? signedInUser.DisplayName
                : (signedInUser.Email ?? studentId);
            
            profilePanel.SetActive(true);
            isSignedIn = true;
            user = signedInUser;
        });

        // UpdateUserProfile();
    }

    void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
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




}
