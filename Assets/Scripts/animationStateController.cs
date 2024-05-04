using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJoggingHash;
    int isRunningHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isJoggingHash = Animator.StringToHash("isJogging");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool isJogging = animator.GetBool(isJoggingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        // if player presses w key
        if (forwardPressed && !isJogging)
        {
            // set boolean isJogging to true
            animator.SetBool(isJoggingHash, true);
        }

        // if player is not pressing w key
        if (!forwardPressed && isJogging)
        {
            // set boolean isJogging to false
            animator.SetBool(isJoggingHash, false);
        }

        // if player is moving forward and pressing left shift
        if (!isRunning && (forwardPressed && runPressed))
        {
            // set boolean isRunning to true
            animator.SetBool(isRunningHash, true);
        }

        // if player stops moving forward or stops pressing left shift
        if (isRunning && (!forwardPressed || !runPressed))
        {
            // set boolean isRunning to false
            animator.SetBool(isRunningHash, false);
        }
    }
}
