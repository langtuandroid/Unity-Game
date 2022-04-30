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
    [SerializeField] private float distance;
    private Vector3 pmouse_pos;
    private float angle;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); 
        rb = GetComponent<Rigidbody2D>();
        horizontalInput = 0f;
        verticalInput = 0f;
        angle = 0f;
        Combat o = new Combat(this);
        new StandardMoveSet(this, o);
        o.SetAttackDamage(50);
        SetHealth(10);
        customTags.Add(Setting.TAG_PLAYER);
    }

    public override void OnKillEntity(int id, bool killingBlow)
    {
        if (killingBlow) {
            Debug.Log("Killed entity:" + id);
        }
    }

    // Update is called once per frame
    protected override void Movement() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetKeyUp(KeyCode.F)) {
            ((Action)GetEntityComponent(Setting.COMPONENT_ACTION)).EnqueueAction("circle_attack");
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
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
        FollowMouse();
    }

    private void FollowMouse() {
        // If the mouse hasn't moved, do not update the destination angle
        Vector2 mouse_pos = Input.mousePosition;
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 current = new Vector2(camera.transform.right.x, camera.transform.right.y);
        if (pmouse_pos != Input.mousePosition)
        {
            Vector2 dir = new Vector2(mouse_pos.x - object_pos.x, mouse_pos.y - object_pos.y);
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
