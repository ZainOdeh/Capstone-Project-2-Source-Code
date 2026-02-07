using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class NextBotAI : MonoBehaviour {
    [Header("Chase")]
    public NavMeshAgent agent;           // drag in Inspector or auto-filled in Awake
    [SerializeField] private string playerTag = "Player";

    [Header("Attack")]
    [Tooltip("Seconds between each hit")]
    [SerializeField] private float hitCooldown = 1.0f;

    private Transform playerTf;
    private float nextHitAllowed;    // absolute time stamp
    private bool active = true;     // set false when player loses

    public AudioClip hitSFX;

    private void Awake() {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }
    GameObject p;
    private void Start() {
        p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) playerTf = p.transform;
        else Debug.LogError($"Player with tag <{playerTag}> not found");

        nextHitAllowed = 0f;             // can hit immediately
    }

    private void Update() {
        if (!active) return;                             // bot shut down

        // stop everything once the Lose screen is up
        if ((GameMenuManager.Instance.gameEnded || GameMenuManager.Instance.PlayerDead)) {
            DisableBot();
            return;
        }

        // keep chasing
        if (playerTf && agent.isOnNavMesh && agent.enabled)
            agent.SetDestination(playerTf.position);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(playerTag)) TryHit();
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag(playerTag)) TryHit();
    }

    private void TryHit() {
        if (Time.time < nextHitAllowed) return;          // still on cooldown

        GameMenuManager.Instance.ReduceGPA();
        nextHitAllowed = Time.time + hitCooldown;
        GameManager.Instance.PlayDamageFlash();
        SFXManager.instance.PlaySoundFXClip(hitSFX, transform, 0.7f);

        if (GameMenuManager.Instance.PlayerDead)
            DisableBot();
    }

    private void DisableBot() {
        
            gameObject.SetActive(false);
        
        
    }
}
