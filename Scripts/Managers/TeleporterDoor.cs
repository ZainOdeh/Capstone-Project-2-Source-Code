using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterDoor : MonoBehaviour
{
    public GameMenuu GameMenu;
    private void OnTriggerEnter(Collider other) {
        if (other != null) {
            other.GetComponent<PlayerInputt>().UnlockCursor();
        }
        GameMenu.StartCoroutine(GameMenu.LoadGameAsync("Cutscene2"));
    }
}
