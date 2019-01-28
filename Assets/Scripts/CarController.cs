using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{

    //****** VARIABLES ***********************

    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;
    private bool secondJump;
    private bool onGround;
    private bool frontDriverC;
    private bool frontPassengerC;
    private bool rearDriverC;
    private bool rearPassengerC;
    private bool dead;
    public bool allWheelsOnGround;
    public float boostForce;
    public float jumpForce;
    public float airRotationSpeed;
    public float maxSteerAngle = 30;
    public float motorForce = 50;

    public Transform boost;
    private Rigidbody rb;
    public AudioSource audioBoost;
    public WallOfDeathController wallController;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    




    //**************** METHODS **********************************




    //Get Horizontal and Vertical Inputs
    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

    }

    //Calculates steering angle
    private void Steer()
    {
        steeringAngle = maxSteerAngle * horizontalInput;
        frontDriverW.steerAngle = steeringAngle;
        frontPassengerW.steerAngle = steeringAngle;
    }

    //Car movement on ground
    private void Accelerate()
    {
        frontDriverW.motorTorque = verticalInput * motorForce;
        frontPassengerW.motorTorque = verticalInput * motorForce;
    }

    //Animates wheels
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    //Updates wheel position
    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 pos = _transform.position;
        Quaternion quat = _transform.rotation;

        _collider.GetWorldPose(out pos, out quat);

        _transform.position = pos;
        _transform.rotation = quat;
    }




  
 //*************** COLLISIONS ****************************************************
         




    //Collision enter
    void OnCollisionEnter(Collision collision)
    {
        //Checks if the car is on the ground
        if (collision.gameObject.tag == "ground")
        {
            onGround = true;
            secondJump = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        //Checks if the car hits any obstacle, if so, resets it
        if (collision.gameObject.tag == "death")
        {
            wallController.wallReset = true;
            dead = true;
            rb.rotation = Quaternion.identity;
            frontDriverW.motorTorque = 0;
            frontPassengerW.motorTorque = 0;
            rearDriverW.motorTorque = 0;
            rearPassengerW.motorTorque = 0;
            transform.position = new Vector3(0f, 1f, 0f);
            StartCoroutine(DeadFalse());
            audioBoost.Stop();
            wallController.active = false;
            StartCoroutine(ActivateWall());

        }

        //Checks if car finishes de level

        if (collision.gameObject.tag == "end")
        {
            wallController.wallReset = true;
            dead = true;
            rb.rotation = Quaternion.identity;
            frontDriverW.motorTorque = 0;
            frontPassengerW.motorTorque = 0;
            rearDriverW.motorTorque = 0;
            rearPassengerW.motorTorque = 0;
            transform.position = new Vector3(0f, 1f, 0f);
            StartCoroutine(DeadFalse());
            audioBoost.Stop();
            wallController.active = false;
            StartCoroutine(ActivateWall());
        }
    }

    //During collision
    private void OnCollisionStay(Collision collision)
    {
       
        WheelHit hit;
        frontDriverC = frontDriverW.GetGroundHit(out hit);
        frontPassengerC = frontPassengerW.GetGroundHit(out hit);
        rearDriverC = rearDriverW.GetGroundHit(out hit);
        rearPassengerC = rearPassengerW.GetGroundHit(out hit);

        //Checks if all wheels are touching the ground
        if (frontDriverC && frontPassengerC && rearDriverC && rearPassengerC)
        {
            allWheelsOnGround = true;
        }
        else
        {
            allWheelsOnGround = false;
        }
    }

    //Collision exit
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {

            onGround = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }




    //*********** COUROTINES ***********************************************






    //Second jump timer
    IEnumerator JumpTime()
    {
        yield return new WaitForSeconds(1.5f);
        if (!onGround)
        {
            secondJump = true;
        }
    }

    //Car Reset

    IEnumerator DeadFalse()
    {
        yield return new WaitForSeconds(1f);
        dead = false;
    }

    //Wall Activation

    IEnumerator ActivateWall()
    {
        
        yield return new WaitForSeconds(3f);
        wallController.active = true;
    }






    //************ GAME UPDATE **************************************







    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ActivateWall());
    }

    private void FixedUpdate()
    {
        //Calls all functions created above
        GetInput();
        Steer();
        UpdateWheelPoses();

        if (Input.GetKeyDown("j") && !dead)
        {
            audioBoost.Play();
        }

        if (Input.GetKeyUp("j") && !dead)
        {
            audioBoost.Stop();
        }

        //Makes the car boost
        if (Input.GetKey("j") && !dead)
        {
            Vector3 boostDirection = rb.transform.position - boost.position;
            rb.AddForce(boostDirection * boostForce, ForceMode.Acceleration);
            
        }

        if (onGround && !dead)
        {
            Accelerate();

            //Makes the car jump
            if (Input.GetButtonDown("Jump"))
            {
                if (allWheelsOnGround)
                {
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    StartCoroutine(JumpTime());
                }
                else
                {
                    rb.rotation = Quaternion.identity;
                }
                
            }
        }
        else if(!onGround && !dead)
        {
            //Car rotation in the air
            if (horizontalInput != 0)
            {
                //Air Roll
                if (Input.GetKey("k"))
                {
                    transform.Rotate(new Vector3(0f, 0f, 1f) * -horizontalInput * airRotationSpeed * 1.5f * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(new Vector3(0f, 1f, 0f) * horizontalInput * airRotationSpeed * Time.deltaTime);
                }
            }

            if (verticalInput != 0)
            {
                transform.Rotate(new Vector3(1f, 0f, 0f) * verticalInput * airRotationSpeed * Time.deltaTime);
            }

            //Second Jump

            if (!secondJump)
            {
                //Makes the car jump
                if (Input.GetButtonDown("Jump"))
                {
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    secondJump = true;
                }
            }
        }
    }
}
