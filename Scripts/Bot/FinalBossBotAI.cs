using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FinalBossBotAI : MonoBehaviour {
    [Header("Target")]
    public Transform player;

    [Header("Look At (Smooth)")]
    [Tooltip("Higher = snappier rotation. Lower = more delay.")]
    public float lookSmooth = 5f;
    public bool lockYRotation = true;

    [Header("Missile Attack")]
    public GameObject missilePrefab;
    public Transform missileSpawnPoint;
    public float fireInterval = 2.0f;
    public float missileMoveSpeed = 10f;
    public float missileTurnSpeed = 360f;
    public float missileLifeTime = 6f;
    public int missileDamage = 1;

    [Header("Boss Health")]
    public int maxHitsToKill = 4;   // 4 hits kills U
    public Slider bossHealthSlider; // Slider value 0..1

    [Header("Hit Feedback")]
    public Color hitFlickerColor = Color.white;
    public float flickerDuration = 2f;
    public float flickerHz = 12f;

    [Header("Defeat")]
    public UnityEvent onDefeated; // leave empty for now

    private int _hitsRemaining;
    private bool _isDefeated;
    private float _fireTimer;

    private Renderer[] _renderers;
    private Color[] _originalColors;
    private Coroutine _flickerRoutine;


    private void Start() {
    }

    private void Update() {
        if (!player) return;

        SmoothLookAtPlayer();
        
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= fireInterval) {
            _fireTimer = 0f;
            FireMissile();
        }
    }

    private void SmoothLookAtPlayer() {
        Vector3 dir = player.position - transform.position;
        if (lockYRotation) dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        float t = 1f - Mathf.Exp(-lookSmooth * Time.deltaTime); // smooth & frame-rate independent
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
    }

    
    private void FireMissile() {
        if (!missilePrefab) return;

        GameObject m = Instantiate(missilePrefab, missileSpawnPoint);
        m.transform.parent = null;
        HomingMissile hm = m.GetComponent<HomingMissile>();
        if (hm) {
            hm.SetTarget(player);
            //hm.moveSpeed = missileMoveSpeed;
            //hm.turnSpeedDegPerSec = missileTurnSpeed;
            //hm.lifeTime = missileLifeTime;
            //hm.damage = missileDamage;
        }
    }
}
