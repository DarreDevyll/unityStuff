using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public float runSpeed = 20f;

    float horizontalMovement = 0f;
    float verticalMovement = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal") * runSpeed * Time.fixedDeltaTime;
        verticalMovement = Input.GetAxisRaw("Vertical") * runSpeed * Time.fixedDeltaTime;
        transform.Translate(horizontalMovement, verticalMovement, 0);
    }
}
