using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.6f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;


    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
       

        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra xem nhân vật có đang trên mặt đất không
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Đặt lại vận tốc y nếu đang đứng trên mặt đất
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Lấy giá trị từ input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Tạo vector di chuyển
        Vector3 move = transform.right * x + transform.forward * z;

        // Thực hiện di chuyển
        controller.Move(move * speed * Time.deltaTime);

        // Xử lý nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Xử lý rơi xuống
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Di chuyển với vận tốc trọng lực
        controller.Move(velocity * Time.deltaTime);

        // Cập nhật trạng thái di chuyển
        if (Vector3.Distance(lastPosition, gameObject.transform.position) > 0.01f && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Cập nhật vị trí cuối cùng
        lastPosition = gameObject.transform.position;
    }

}
