using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Locomotion")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float jumpStrength = 5f;
    [SerializeField] float jumpButtonGracePeriod;
    Quaternion targetRotation;
    float trueSpeed;
    float threshold;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    float ySpeed;
    bool isGrounded;
    float gravity = -9.81f;
    float? lastGroundedTime;
    float? jumpButtonPressedTime;
    bool isJumping;

    [Header("Combat")]
    [SerializeField] float attackRadius;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    float lastAttack = -3f;
    float attackCooldown = 3f;

    CameraController cameraController;
    CharacterController characterController;
    Animator animator;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // get player input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        // build vector using player input
        var directionInput = new Vector3(horizontal, 0, vertical).normalized;
        // check for walk or run
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        // movement dependent on camera rotation - DO NOT ACCOUNT FOR VERTICAL ROTATION
        var moveDirection = cameraController.PlanarRotation * directionInput;

        // grounded check
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset),
            groundCheckRadius, groundLayer);

        // check for input before adjusting character speed and rotation
        if (directionInput.magnitude > 0)
        {
            animator.SetBool("isMoving", true);
            if (isRunning)
            {
                trueSpeed = runSpeed;
                threshold = 1;
            }
            else
            {
                trueSpeed = walkSpeed;
                threshold = 0;
            }

            // change direction character is facing based on input
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            animator.SetBool("isMoving", false);
            threshold = 0;
        }

        // adjust vertical speed for gravity, if not grounded
        ySpeed += gravity * Time.deltaTime;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        // allow for jumps when not grounded within the grace period
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            ySpeed = 0f;
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);
            // allow for preemptive jump if button pressed within grace period
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpStrength;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            animator.SetBool("isGrounded", false);

            // transition to falling, check for jumping and vertical speed
            // or conditional for falling but not initially jumping (platform)
            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
        }

        // build velocity vector and move character
        Vector3 velocity = moveDirection * trueSpeed;
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);

        // smooth player rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            targetRotation, rotationSpeed * Time.deltaTime);

        // switch animations (dampened through animator)
        animator.SetFloat("moveAmount", threshold, 0.15f, Time.deltaTime);

        Attack();
    }

    private void Attack()
    {
        // guarantees that an attack is performed while not in midair
        if (Input.GetMouseButtonDown(0) && isGrounded && (Time.time - lastAttack >= attackCooldown))
        {
            animator.SetTrigger("isAttack");
            Debug.Log("You performed an attack.");
            // grab instance of attack trigger to compare with attack cooldown
            lastAttack = Time.time;
            // detect enemies in range of attack
            Collider[] hitThing = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);

            foreach (Collider thing in hitThing)
            {
                Debug.Log(thing.name + " was hit!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
        Gizmos.DrawSphere(attackPoint.position, attackRadius);
    }
}