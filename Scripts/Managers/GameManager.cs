// ???????????????????????????????????????????????????????????????
//  GameManager.cs ? add once to an empty scene object
// ???????????????????????????????????????????????????????????????
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns collectibles in phase order A ? B ? C and
/// keeps score of how many the player has picked up.
/// </summary>
public class GameManager : MonoBehaviour {
    #region Singleton
    public static GameManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
    }
    #endregion


    // ??????????????????????? Prefabs & Spawn Data ???????????????????????
    [Header("Collectible Prefabs")]
    public Collectible prefabP;
    public Collectible prefabM;
    public Collectible prefabD;

    [Header("Spawn Settings")]
    [Tooltip("Every empty transform in this list is a possible spawn point.")]
    public Transform[] spawnPoints;


    //UI Stuff
    public GameMenuManager menuManager;
    public TextMeshProUGUI criteria;
    public TextMeshProUGUI WinScreenText;
    int maxCriterValue;
    int currentCriteriaValue;

    public Light directionalLight;


    [Header("Hit Flash")]
    public Image damageFlash;
    [SerializeField] private float flashAlpha = 0.45f;   // peak opacity
    [SerializeField] private float flashInTime = 0.08f;   // quick pop
    [SerializeField] private float flashOutTime = 0.4f;    // smooth fade

    private Coroutine damageRoutine;

    


    // ??????????????????????? Runtime State ???????????????????????
    private enum Phase { P, M, D, Done }
    private Phase currentPhase = Phase.P;

    private readonly Dictionary<CollectibleType, int> _totalNeeded =
        new() { { CollectibleType.P, 0 }, { CollectibleType.M, 0 }, { CollectibleType.D, 0 } };

    private readonly Dictionary<CollectibleType, int> _collected =
        new() { { CollectibleType.P, 0 }, { CollectibleType.M, 0 }, { CollectibleType.D, 0 } };


    // ??????????????????????? Unity Flow ???????????????????????
    private void Start() {
        SpawnCurrentPhase();   // kick things off with A objects
        directionalLight.color = Color.white;
    }
    // ??????????????????????? Public Read-Only API  ???????????????????????
    public int CollectedA => _collected[CollectibleType.P];
    public int CollectedB => _collected[CollectibleType.M];
    public int CollectedC => _collected[CollectibleType.D];

    public bool AComplete => CollectedA >= _totalNeeded[CollectibleType.P];
    public bool BComplete => CollectedB >= _totalNeeded[CollectibleType.M];
    public bool CComplete => CollectedC >= _totalNeeded[CollectibleType.D];

    // ??????????????????????? Called by Collectible ???????????????????????
    public void RegisterCollection(CollectibleType type) {
        _collected[type]++;
        criteria.text = type.ToString() + ": " + _collected[type] +"/"+prefab.amount;
        // When a phase finishes, immediately spawn the next one
        if (PhaseIsFinished(type)) {
            switch (currentPhase) {
                case Phase.P: currentPhase = Phase.M; SpawnCurrentPhase();criteria.text = "M: 0/"+prefab.amount ; break;
                case Phase.M: currentPhase = Phase.Done; WinGameDoor(); criteria.text = "";WinScreenText.text = "Go To Orange Village"; break;
                case Phase.D: currentPhase = Phase.Done; menuManager.WinGameVisuals(); break;
            }
        }
    }

    //
    public GameObject OVDoor;
    public GameObject normalDoor;

    public void WinGameDoor() {
        OVDoor.SetActive(true);
        normalDoor.SetActive(false);
    }
    //


    public void TeleportToOrangeVillage() {
        print("Going to orange vilalge");
    }
    



    // ??????????????????????? Internals ???????????????????????
    private bool PhaseIsFinished(CollectibleType type) =>
        _collected[type] >= _totalNeeded[type];

    Collectible prefab;
    private void SpawnCurrentPhase() {
        if (currentPhase == Phase.Done) return;

        prefab = currentPhase switch {
            Phase.P => prefabP,
            Phase.M => prefabM,
            Phase.D => prefabD,
            _ => null
        };

        if (prefab == null) {
            Debug.LogError($"Prefab for phase {currentPhase} is missing!");
            return;
        }

        // Decide how many and remember that number for completion checking
        _totalNeeded[prefab.type] = prefab.amount;

        // Shuffle spawn points so each round feels different
        List<Transform> shuffled = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffled.Count; i++) {
            Transform temp = shuffled[i];
            int swapIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[swapIndex];
            shuffled[swapIndex] = temp;
        }

        // Spawn the pickups
        int spawnCount = Mathf.Min(prefab.amount, shuffled.Count);
        for (int i = 0; i < spawnCount; i++) {
            Instantiate(prefab, shuffled[i].position, Quaternion.identity);
        }

        Debug.Log($"? Spawned {spawnCount} × {prefab.type}");
    }

    public void PlayDamageFlash() {
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        damageRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine() {
        Color c = damageFlash.color;

        /* flash IN */
        float t = 0f;
        while (t < flashInTime) {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, flashAlpha, t / flashInTime);
            damageFlash.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        /* flash OUT */
        t = 0f;
        while (t < flashOutTime) {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(flashAlpha, 0f, t / flashOutTime);
            damageFlash.color = c;
            yield return null;
        }

        c.a = 0f;
        damageFlash.color = c;
        damageRoutine = null;
    }
}
