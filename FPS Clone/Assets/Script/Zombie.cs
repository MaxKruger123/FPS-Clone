using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Zombie : MonoBehaviour
{

    private enum ZombieState
    {
        Walking,
        Ragdoll
    }

    public Target target;
    public CharacterController characterController;

    private Rigidbody[] _ragdollRigidbodies;
    [SerializeField]
    private Camera _camera;
    public Animator animator;

    private ZombieState _currentState = ZombieState.Walking;

    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        DisableRagdoll();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case ZombieState.Walking:
                WalkingBehaviour();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour(); 
                break;
        }

        if (target.currentHealth <= 0)
        {
            EnableRagdoll();
        }
    }

    private void DisableRagdoll()
    {
        foreach(var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        animator.enabled = false;
        characterController.enabled = false;
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitForce)
    {
        EnableRagdoll();

        Rigidbody hitRigidbody = _ragdollRigidbodies.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitForce)).First();

        hitRigidbody.AddForceAtPosition(force, hitForce, ForceMode.Impulse);

        _currentState = ZombieState.Ragdoll;
    }

    private void WalkingBehaviour()
    {
        Vector3 direction = _camera.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);

        
    }

    private void RagdollBehaviour()
    {

    }
}
