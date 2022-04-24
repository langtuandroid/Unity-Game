using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public Camera camera;
    Rigidbody2D rb;
    float horizontalInput;
    float verticalInput;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); 
        rb = GetComponent<Rigidbody2D>();
        horizontalInput = 0f;
        verticalInput = 0f;
        Combat o = new Combat(this);
        o.SetAttackDamage(50);
        base.SetHealth(10);
    }

    // Update is called once per frame
    protected override void Movement() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetKeyUp(KeyCode.F)) {
            ((Action)GetEntityComponent("Action")).EnqueueAction("circle_attack");
        }
        
    }

    private void FixedUpdate()
    {
        float temp = speed;
        if (horizontalInput != 0 && verticalInput != 0) {
            temp /= Mathf.Sqrt(2);
        }
        rb.velocity = new Vector2(horizontalInput *temp, verticalInput * temp);
    }

    private void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10);

        Vector2 mouse_pos = Input.mousePosition;
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);

        // Define unresponsive radius
        Vector3 mouse_wpos = camera.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, 10));
        float dis = Mathf.Abs(Vector3.Distance(transform.position, mouse_wpos));
        if (dis < 0.4) {
            return;
        }

        Vector2 current = new Vector2(camera.transform.right.x, camera.transform.right.y);
        Vector2 dir = new Vector2(mouse_pos.x - object_pos.x, mouse_pos.y - object_pos.y);
        float angle = Vector2.SignedAngle(current, dir);
        float currentAngle = transform.rotation.eulerAngles.z;
        float angleDif = Mathf.DeltaAngle(angle, currentAngle);
        if (Mathf.Abs(angleDif) > 0) {
            float rotateMax = rotateSpeed * Time.deltaTime;
            if (Mathf.Abs(angleDif) > rotateMax)
            {
                if (angleDif > 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle - rotateMax));
                }
                else {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle + rotateMax));
                }
            }
            else {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

       
    }
}
