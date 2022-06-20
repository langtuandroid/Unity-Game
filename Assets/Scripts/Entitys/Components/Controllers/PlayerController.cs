using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Action))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private VoidEventChannel gameEndChannel;
    private Entity player;
    [SerializeField]private Camera camera;
    Rigidbody2D rb;
    float horizontalInput;
    float verticalInput;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float distance;
    private Vector3 pmouse_pos;
    private float angle;

    public void Start(){
        player = GetComponent<Entity>();
        rb = player.GetComponent<Rigidbody2D>();
        horizontalInput = 0f;
        verticalInput = 0f;
        angle = 0f;
    }
    public void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyUp(KeyCode.F))
        {
            GetComponent<Action>().EnqueueAction(Setting.STD_CIRCLE_ATTACK);
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            gameEndChannel.RaiseEvent();
        }
    }

    public void FixedUpdate()
    {
        float temp = speed;
        if (horizontalInput != 0 && verticalInput != 0)
        {
            temp /= Mathf.Sqrt(2);
        }
        rb.velocity = new Vector2(horizontalInput * temp, verticalInput * temp);
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
        FollowMouse();
    }

    private void FollowMouse()
    {
        // If the mouse hasn't moved, do not update the destination angle
        Vector2 mouse_pos = Input.mousePosition;
        Transform transform = player.transform;
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 current = new(camera.transform.right.x, camera.transform.right.y);
        if (pmouse_pos != Input.mousePosition)
        {
            Vector2 dir = new(mouse_pos.x - object_pos.x, mouse_pos.y - object_pos.y);
            angle = Vector2.SignedAngle(current, dir);
        }

        float currentAngle = transform.rotation.eulerAngles.z;
        float angleDif = Mathf.DeltaAngle(angle, currentAngle);
        if (Mathf.Abs(angleDif) > 0)
        {
            float rotateMax = rotateSpeed * Time.deltaTime;
            if (Mathf.Abs(angleDif) > rotateMax && rotateSpeed > 0)
            {
                if (angleDif > 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle - rotateMax));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle + rotateMax));
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        // Store the current mouse position for next frame
        pmouse_pos = Input.mousePosition;
    }
}
