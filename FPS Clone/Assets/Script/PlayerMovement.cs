using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f; // Walking speed
    public float sprintSpeed = 18f; // Sprinting speed
    public float sprintDuration = 3f; // Max sprint time
    public float sprintCooldown = 3f; // Cooldown time before sprinting again
    private float sprintRemaining;
    private bool isSprinting = false;
    private bool isSprintCooldown = false;

    public float gravity = -9.81f;
    public Transform Groundcheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    public float jumpHeight = 3f;

    Vector3 velocity;

    // Sprint key
    public KeyCode sprintKey = KeyCode.LeftShift;

    private void Start()
    {
        sprintRemaining = sprintDuration; // Initialize sprint time
        Application.targetFrameRate = 60; // Cap frame rate to 60 FPS
    }


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(Groundcheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint logic
        if (Input.GetKey(sprintKey) && sprintRemaining > 0 && !isSprintCooldown)
        {
            controller.Move(move * sprintSpeed * Time.deltaTime); // Sprint movement
            isSprinting = true;
            sprintRemaining -= Time.deltaTime; // Drain sprint bar
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime); // Normal movement
            isSprinting = false;

            if (sprintRemaining < sprintDuration && !isSprintCooldown)
            {
                sprintRemaining += Time.deltaTime; // Regenerate sprint when not sprinting
            }
        }

        // Handle sprint cooldown
        if (sprintRemaining <= 0)
        {
            isSprintCooldown = true;
            StartCoroutine(SprintCooldown());
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Coroutine to manage sprint cooldown
    IEnumerator SprintCooldown()
    {
        yield return new WaitForSeconds(sprintCooldown);
        sprintRemaining = sprintDuration;
        isSprintCooldown = false;
    }
}
