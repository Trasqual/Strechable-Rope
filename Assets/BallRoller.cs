using UnityEngine;

public class BallRoller : MonoBehaviour
{
    [SerializeField] float speed  = 100f;
    [SerializeField] float angularSpeed = 100f;
    [SerializeField] float randomTorqueLimit = 1f;
    [SerializeField] Joystick joystick;
    [SerializeField] Transform follower;
    [SerializeField] float verticalSideMult = 1;
    private float randomTorque;

    Rigidbody rb;

    Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 15f;
    }

    private void Update()
    {
        randomTorque = Random.Range(0.1f, randomTorqueLimit);
    }

    private void FixedUpdate()
    {
        var forceVector = new Vector3(joystick.Horizontal * verticalSideMult, 0f, joystick.Vertical).normalized;
        var torqueVector = new Vector3(joystick.Vertical * verticalSideMult, 0f, -joystick.Horizontal).normalized;
        float clampedMag = Mathf.Clamp(forceVector.magnitude, 0f, 1f);
        if(clampedMag > 0.9f)
        {
            clampedMag = 1f;
        }
        forceVector = Quaternion.LookRotation(follower.forward) * forceVector;
        torqueVector = Quaternion.LookRotation(follower.forward) * torqueVector;
        rb.AddForce(forceVector * 0.85f * clampedMag * speed);
        rb.AddTorque(torqueVector * 0.4f * clampedMag * angularSpeed);
    }


}
