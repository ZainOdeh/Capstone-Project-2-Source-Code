//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Collectible : MonoBehaviour
//{
//    void Update()
//    {
//       transform.localRotation = Quaternion.Euler(0f , Time.time * 100f, 0f);
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            MenuManager.Instance.IncreaseScore();
//            Destroy(gameObject);
//        }

//    }
//}


//New
using UnityEngine;

public enum CollectibleType { P, M, D }

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour {
    [Tooltip("Set this in the prefab inspector (P, M, or D)")]
    public CollectibleType type;
    public int amount;
    public GameObject LetterParticle;
    public AudioClip pickSound;

    void Update() {
        transform.localRotation = Quaternion.Euler(0f, Time.time * 100f, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        GameManagerMainGame.Instance.RegisterCollection(type);
        GetComponentInChildren<MeshFilter>().gameObject.SetActive(false);
        Instantiate(LetterParticle, gameObject.transform.position,gameObject.transform.rotation,null);
        //SFXManager.instance.PlaySoundFXClip(pickSound,transform,0.7f);
        Destroy(gameObject);
    }
}