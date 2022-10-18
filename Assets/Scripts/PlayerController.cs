using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;

struct CMD
{
    public float forwardMove;
    public float sideMove;
    public float upMove;
}

public class PlayerController : MonoBehaviour
{
    #region Camera Variables
    public Transform playerCamera;
    public float cameraYOffset = 0.7f;

    private float rotX = 0f;
    private float rotY = 0f;
    readonly private float clampXpos = 90f;
    readonly private float clampLookDownOffset = 30f;

    public float xMouseSensitivity = 30f;
    public float yMouseSensitivity = 30f;
    #endregion

    #region Movement
    CMD cmd;

    private CharacterController controller;
    private Vector3 pVelocity = Vector3.zero;
    private Vector3 moveDirNorm = Vector3.zero;

    public float moveSpeed = 7f;
    public float runAccel = 14f;
    public float runDeAccel = 10f;

    public float airDeccel = 2f;
    public float sideSpeed = 1f;
    public float sideAccel = 50f;
    public float gravity = 20f;
    public float jumpSpeed = 8f;


    public float friction = 6;




    readonly bool isBunnyHopping = false;
    bool wishJump = false;
    #endregion

    #region SFX
    public AudioSource audioSource;
    public AudioClip jumpSfx;
    #endregion


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerCamera == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                playerCamera = mainCamera.transform;
            }
        }

        playerCamera.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);

        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Cursor
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        //Camera
        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        if (rotX < -clampXpos)
        {
            rotX = -clampXpos;
        }
        else if (rotX > clampXpos - clampLookDownOffset)
        {
            rotX = clampXpos - clampLookDownOffset;
        }
        transform.rotation = Quaternion.Euler(0, rotY, 0);
        playerCamera.rotation = Quaternion.Euler(rotX, rotY, 0);

        //Movement
        QueueJump();
        if (controller.isGrounded)
            PlayerMove();
        else if (!controller.isGrounded)
            PlayerMidAir();

        controller.Move(pVelocity * Time.deltaTime);

        //Camera follow after movement
        playerCamera.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
    }

    private void QueueMoveDir()
    {
        cmd.forwardMove = Input.GetAxisRaw("Vertical");
        cmd.sideMove = Input.GetAxisRaw("Horizontal");
    }

    private void QueueJump()
    {
        if (isBunnyHopping)
        {
            //todo jumpsfx with bunnyhopping in mind
            wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !wishJump)
        {
            audioSource.PlayOneShot(jumpSfx);

            //isBunnyHopping = true;
            wishJump = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            //isBunnyHopping = false;
            wishJump = false;
        }
    }

    private void PlayerMove()
    {
        Vector3 wishdir;

        if (!wishJump)
        {
            ApplyFriction(1f);
        }
        else
        {
            ApplyFriction(0);
        }

        QueueMoveDir();

        wishdir = new Vector3(cmd.sideMove, 0, cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAccel);

        pVelocity.y = -gravity * Time.deltaTime;

        if (wishJump)
        {
            pVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void Accelerate(Vector3 wd, float ws, float accel)
    {
        float addSpeed;
        float accelSpeed;
        float currentSpeed;

        currentSpeed = Vector3.Dot(pVelocity, wd);
        addSpeed = ws - currentSpeed;

        if (addSpeed <= 0)
            return;

        accelSpeed = accel * Time.deltaTime * ws;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        pVelocity.x += accelSpeed * wd.x;
        pVelocity.z += accelSpeed * wd.z;
    }

    private void ApplyFriction(float v)
    {
        Vector3 vec = pVelocity;
        float speed;
        float newSpeed;
        float control;
        float drop;

        vec.y = 0f;
        speed = vec.magnitude;
        drop = 0f;

        if (controller.isGrounded)
        {
            control = speed < runDeAccel ? runDeAccel : speed;  
            drop = control * friction * Time.deltaTime * v;
        }

        newSpeed = speed - drop;

        if (newSpeed < 0)
        {
            newSpeed = 0;
        }
        if (speed > 0)
        {
            newSpeed /= speed;
        }

        pVelocity.x *= newSpeed;
        pVelocity.z *= newSpeed;

    }

    private void PlayerMidAir()
    {
        Vector3 wishDir;
        float accel;
        QueueMoveDir();

        wishDir = new Vector3(cmd.sideMove, 0, cmd.forwardMove);
        wishDir = transform.TransformDirection(wishDir);

        float wishspeed = wishDir.magnitude;
        wishspeed *= moveSpeed;

        wishDir.Normalize();
        moveDirNorm = wishDir;

        if (Vector3.Dot(pVelocity, wishDir) < 0)
            accel = airDeccel;
        else
            accel = airDeccel;
        if (cmd.forwardMove == 0 && cmd.sideMove != 0)
        {
            if (wishspeed > sideSpeed)
                wishspeed = sideSpeed;
            accel = sideAccel;
        }

        Accelerate(wishDir, wishspeed, accel);

        pVelocity.y -= gravity * Time.deltaTime;
    }
}
