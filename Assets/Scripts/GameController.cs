using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {Login, FreeRoam, Dialog, Battle}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    GameState state;

    public static GameController Instance { get; private set; }

    void Awake() => Instance = this;
    public void SetState(GameState newState) => state = newState;

    private void Start()
    {
        
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }


    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    } 
}
