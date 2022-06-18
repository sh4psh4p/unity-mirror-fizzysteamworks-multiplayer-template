using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public float walkSpeed = 5;
    public float sprintSpeed = 8;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    CharacterController controller;
    float speed = 0;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 165;

        if (isLocalPlayer)
        {
            if (GetComponent<CharacterController>() != null)
            {
                controller = GetComponent<CharacterController>();
            }

            else
            {
                controller = gameObject.AddComponent<CharacterController>();
            }
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }

            controller.Move(move.normalized * speed * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
        }
    }
}