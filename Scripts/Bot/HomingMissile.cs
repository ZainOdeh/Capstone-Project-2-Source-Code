using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour {
    public float moveSpeed = 10f;
    public float turnSpeedDegPerSec = 360f;
    public float lifeTime = 6f;
    public int damage = 1;

    public Transform _target;
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform t) => _target = t;

    private void FixedUpdate() {
        if (!_target) {
            MoveForward();
            return;
        }

        Vector3 toTarget = ((_target.position+Vector3.up*3f) - transform.position).normalized;
        if (toTarget.sqrMagnitude < 0.0001f) { MoveForward(); return; }

        Quaternion desired = Quaternion.LookRotation(toTarget, Vector3.up);
        Quaternion newRot = Quaternion.RotateTowards(transform.rotation, desired, turnSpeedDegPerSec * Time.fixedDeltaTime);

        if (_rb) {
            _rb.MoveRotation(newRot);
            _rb.MovePosition(_rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
        } else {
            transform.rotation = newRot;
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
    }

    private void MoveForward() {
        if (_rb)
            _rb.MovePosition(_rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
        else
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<PlayerInput>() != null) {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        } else {
            print("Object hit");
        }
        Destroy(gameObject);
    }
    /*
    private void OnCollisionEnter(Collision collision) {
        var ph = collision.collider.GetComponentInParent<PlayerHealth>();
        if (ph != null) {
            ph.TakeDamage(damage);
        }
        Destroy(gameObject);
    }*/
}
