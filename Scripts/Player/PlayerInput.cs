using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool escape;


    //New - Faris
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    //New - Faris
    public bool canSprint = true;
    //UI reference
    public bool canPause = true;
    public bool paused = false;

    public GameMenuManager gameMenuManager;

    private void Start() {
        //No need for this
        //// For invisible cursor and locking it in middle of screen
        //Cursor.visible = false;
        //LockCursor();
    }

    private void Update() {
        //Could need some modification for control settings
        MoveInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        LookInput(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        JumpInput(Input.GetButton("Jump"));
        //New
        if(canSprint) {
            SprintInput(Input.GetKey(KeyCode.LeftShift));
        }
        else{
            sprint = false;
        }
        //EscapeInput(Input.GetKeyDown(KeyCode.Escape));
    }

    /*public void EscapeInput(bool newEscapeState) {
        if(GameMenuManager.Instance.PlayerDead == false) {
            if (newEscapeState) {
                print("actually started");
                if (gameMenuManager.canPause) {
                    if (gameMenuManager.isPaused == false) {
                        gameMenuManager.Pause();
                        gameMenuManager.isPaused = true;
                        gameMenuManager.canPause = false;
                        UnlockCursor();
                        print("pased");
                    }
                } else {
                    gameMenuManager.Resume();
                    gameMenuManager.isPaused = false;
                    gameMenuManager.canPause = true;
                    LockCursor();
                    print("resumed");
                }
            }
        }
    }*/

    public void MoveInput(Vector2 newMoveDirection) {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection) {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState) {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState) {
        sprint = newSprintState;
    }
    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
