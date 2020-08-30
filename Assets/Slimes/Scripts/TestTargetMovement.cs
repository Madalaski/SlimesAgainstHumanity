using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetMovement : MonoBehaviour
{
    CharacterController characterController;
    float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            characterController.Move(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            characterController.Move(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.D))
        {
            characterController.Move(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            characterController.Move(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0f, -9.81f* Time.deltaTime,0f));
        }
    }
}
