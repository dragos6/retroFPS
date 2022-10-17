using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Camera Variables
    public Transform playerCamera;
    public float cameraYOffset = 0.7f;

    private float rotX = 0f;
    private float rotY = 0f;
    private float clampXpos = 90f;
    private float clampLookDownOffset = 30f;

    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;
    #endregion

    #region Movement
    private CharacterController controller;
    private Vector3 pVelocity = Vector3.zero;

    bool isBunnyHopping = false;
    bool wishJump = false;
    #endregion

    #region SFX
    public AudioSource audioSource;
    public AudioClip jumpSfx;
    #endregion  


    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

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
        Jump();
        if (controller.isGrounded) PlayerMove();
        else if (!controller.isGrounded) PlayerMidAir();
        controller.Move(pVelocity * Time.deltaTime);

        //Camera follow after movement
        playerCamera.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
    }

    private void Jump()
    {
        if(isBunnyHopping)
        {
            wishJump = Input.GetButton("Jump");
            audioSource.PlayOneShot(jumpSfx);
            return;
        }

        if(Input.GetButtonDown("Jump")&& !wishJump)
        {
            wishJump = true;
        }
        if(Input.GetButtonUp("Jump"))
        {
            wishJump = false;
        }
    }

    private void PlayerMove()
    {
        Vector3 wishdir;

        if(!wishJump)
        {
            ApplyFriction(1f);
        }
        else
        {
            ApplyFriction(0);
        }
    }

    private void ApplyFriction(float v)
    {
        throw new NotImplementedException();
    }

    private void PlayerMidAir()
    {
        throw new NotImplementedException();
    }

}
