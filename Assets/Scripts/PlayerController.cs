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

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        if (rotX < -clampXpos)
        {
            rotX = -clampXpos;
        }
        else if (rotX > clampXpos - clampLookDownOffset)
        {
            rotX = clampXpos - clampLookDownOffset ;
        }
        transform.rotation = Quaternion.Euler(0, rotY, 0);
        playerCamera.rotation = Quaternion.Euler(rotX, rotY, 0);

        

        // todo movement



        playerCamera.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
    }
}
