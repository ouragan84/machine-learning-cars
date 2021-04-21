// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ccc : MonoBehaviour
{
    public float speed = 12.0f;
    public float rotateSpeed = 25.0f;
    private float carSpeed = 0;

    private bool carOnGround = false;

    public int lap = 0;
    public int checkpointPos = 1;
    public float disToNextCheckpoint;

    public float CPScore;
    public float LapScore;
    public float SpeedScore;
    public float NotMovingScore;
    public float TimeScore;
    public float TimeRangeScore;

    private float lastCPTime;

    public float score;

    private void Start()
    {
        lastCPTime = Time.time;
    }

    void Update()
    {
        float previousCP = checkpointPos;

        move();
        stop();

        float previousDis = disToNextCheckpoint;

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("checkpoint");

        foreach(GameObject j in gos)
        {
            if(j.GetComponent<checkPoint>().pos == checkpointPos+1 || (j.GetComponent<checkPoint>().pos == 0 && checkpointPos == gos.Length - 1))
            {
                Vector3 closestPoint = GetComponent<Collider>().ClosestPointOnBounds(j.transform.position);
                disToNextCheckpoint = Vector3.Distance(closestPoint, j.transform.position);
            }
        }

        if (previousCP == checkpointPos)
            score += SpeedScore * (previousDis - disToNextCheckpoint);
        if ((previousDis - disToNextCheckpoint) == 0)
            score += NotMovingScore;
    }

    void move()
    {
        if (Input.GetKey("up"))
        {
            carSpeed += .5f;

            if (carSpeed > speed)
            {
                carSpeed = speed;
            }
        }

        if (Input.GetKey("down"))
        {
            carSpeed -= .5f;

            if (carSpeed > -1*speed)
            {
                carSpeed = -1*speed;
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * carSpeed, ForceMode.Acceleration);

        if ((Input.GetKey("left")) && (carOnGround == true))
        {
            rb.AddTorque(transform.up * -1 * rotateSpeed * System.Math.Sign(carSpeed));
            if(carSpeed == 0) rb.AddTorque(transform.up * -1 * rotateSpeed);
        }
        if ((Input.GetKey("right")) && (carOnGround == true))
        {
            rb.AddTorque(transform.up * rotateSpeed * System.Math.Sign(carSpeed));
            if (carSpeed == 0) rb.AddTorque(transform.up * rotateSpeed);
        }
    }

    void stop()
    {
        if (Input.GetKey("up") == false)
        {
            if (carSpeed > 0)
            { carSpeed -= 0.5f; }
            else if (carSpeed < 0)
            { carSpeed += 0.5f; }

            if (range(carSpeed, -1, 1))
                carSpeed = 0;
        }
    }

    bool range(float thisNum, float numMin, float numMax)
    {
        if ((thisNum > numMin) && (thisNum < numMax))
        { return true; }
        else
        { return false; }
    }

    void OnCollisionEnter(Collision collision)
    {
        carOnGround = true;
        //if (collision.gameObject.tag == "wall")
           // gameObject.SetActive(false);
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        carOnGround = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "checkpoint")
        {
            if (other.gameObject.GetComponent<checkPoint>().pos == checkpointPos + 1)
            {
                checkpointPos++;
                score += CPScore;
                score -= TimeScore * (lastCPTime - Time.time + TimeRangeScore);
                lastCPTime = Time.time;
            }
            if (other.gameObject.GetComponent<checkPoint>().pos == 0 && checkpointPos != 0)
            {
                lap++; checkpointPos = 0;
                score += LapScore;
                score -= TimeScore * (lastCPTime - Time.time + TimeRangeScore) ;
                lastCPTime = Time.time;
            }
        }
    }
}