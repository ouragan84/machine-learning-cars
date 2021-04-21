using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour {

    private neuralNet brain;

    public float weigthRange;
    public float biasRange;

    private float rightDis;
    private float leftDis;
    private float forwardDis;

    public int lap;
    public int checkpointPos;
    public float disToNextCheckpoint;

    public float speed = 12.0f;
    public float rotateSpeed = 25.0f;
    public float carSpeed = 0.0f;

    private bool carOnGround = false;

    public float score;

    //public float CPScore;
    //public float LapScore;
    public float SpeedScore;
    public float NotMovingScore;
    //public float TimeScore;
    //public float TimeRangeScore;

    //private float lastCPTime;

    public GameObject right;
    public GameObject left;
    public GameObject forward;

    public Material red;
    public Material yellow;
    public Material cyan;
    public Material blue;
    public Material green;
    //public float[][] bias;
    //public float[][][] weight;


    private float[] outputs;

    //best weigth & bias need to be issued from a neuralnet with SAME layer[] var

    public void instantiate(int[] layers, int type /*1=random, 2=10%, 3=1%, 4=best*/, float[][][] bestWeight, float[][] bestBias)
    {
        brain = new neuralNet(layers);
        outputs = new float[layers[layers.Length - 1]];

        //speed = 12;
        //rotateSpeed = 25;

        score = 0;

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Specular");

        if (type == 1)
        {
            GetComponent<Renderer>().material = red;
            brain.initiateRandom(biasRange, weigthRange);
        }
        else if (type == 2)
        {
            GetComponent<Renderer>().material = yellow;
            brain.initiate(bestBias, bestWeight);
            brain.mutate(10.0f);
        }
        else if (type == 3)
        {
            GetComponent<Renderer>().material = cyan;
            brain.initiate(bestBias, bestWeight);
            brain.mutate(1.0f);
        }
        else if (type == 4)
        {
            GetComponent<Renderer>().material = blue;
            brain.initiate(bestBias, bestWeight);
            brain.mutate(0.1f);
        }
        else if (type == 5)
        {
            GetComponent<Renderer>().material = green;
            brain.initiate(bestBias, bestWeight);
        }
        else
        {
            Destroy(gameObject);
        }
        //bias = getBias();
        //weight = getWeights();
    }

    /*private void Start()
    {
        instantiate(new int[]{ 4, 4,3,2},1,null,null);

        for(int i = 1; i < weight.Length; i++)
        {
            for (int j = 0; j < weight[i].Length; j++)
            {
                String s = "";
                for (int k = 0; k < weight[i][j].Length; k++)
                {
                    s += weight[i][j][k] + " ";
                }
                Debug.Log(s);
            }
            Debug.Log("----------");
        }
    }*/

    public float[][][] getWeights()
    {
        return brain.getWeights();
    }

    public float[][] getBias()
    {
        return brain.getBias();
    }


    void Update()
    {
        float previousCP = checkpointPos;

        getDecision();
        move();
        stop();

        float previousDis = disToNextCheckpoint;

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("checkpoint");
        foreach (GameObject j in gos)
        {
            if (j.GetComponent<checkPoint>().pos == checkpointPos + 1 || (j.GetComponent<checkPoint>().pos == 0 && checkpointPos == gos.Length - 1))
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


    void getDecision()
    {
        //right = GameObject.Find("rayRight");
        float rightDis = right.GetComponent<rayDistance>().getDistance();

        float leftDis = left.GetComponent<rayDistance>().getDistance();

        float forwardDis = forward.GetComponent<rayDistance>().getDistance();

        float Spd = carSpeed/speed;
        //Debug.Log(Spd + " " + carSpeed/ speed);
        float[] inputs = { rightDis, leftDis, forwardDis, Spd };

        outputs = brain.input(inputs);// = [speeddown - up, turn left - right]

        String s = "out ";
        for (int k = 0; k < outputs.Length; k++)
        {
            s += outputs[k] + " ";
        }
        Debug.Log(s);

        //Debug.Log("decision made, " + outputs + " with " + inputs);

    }

    void move()
    {
        carSpeed += .5f * outputs[0];

        if (carSpeed > System.Math.Abs(System.Math.Sign(outputs[0]) * speed))
        {
            carSpeed = System.Math.Sign(outputs[0])*speed;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * carSpeed, ForceMode.Acceleration);

            rb.AddTorque(transform.up * outputs[1] * rotateSpeed * System.Math.Sign(carSpeed));
            if (carSpeed == 0) rb.AddTorque(transform.up * outputs[1] * rotateSpeed);
    }

    void stop()
    {
        if (outputs[0] <= 0)
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
        if (collision.gameObject.tag == "wall")
        gameObject.SetActive(false);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
            carOnGround = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "checkpoint")
        {
            if (other.gameObject.GetComponent<checkPoint>().pos == checkpointPos + 1)
            {
                checkpointPos++;
                //score += CPScore;
                //score -= TimeScore * (lastCPTime - Time.time + TimeRangeScore);
                //lastCPTime = Time.time;
            }
            if (other.gameObject.GetComponent<checkPoint>().pos == 0 && checkpointPos != 0)
            {
                lap++; checkpointPos = 0;
                //score += LapScore;
                //score -= TimeScore * (lastCPTime - Time.time + TimeRangeScore);
                //lastCPTime = Time.time;
            }
        }
    }

    //a-b
    public int compareScoreTo(car other)
    {
        if (this.getScore() > other.getScore())
            return 1;
        if (this.getScore() < other.getScore())
            return -1;
        return 0;
    }

    public float getScore()
    {
        return score;
    }

    //a-b
    public int compareFitnessTo(car other)
    {
        if (this.getFitness()[0] == other.getFitness()[0])
            if (this.getFitness()[1] == other.getFitness()[1])
                if (this.getFitness()[2] == other.getFitness()[2])
                    return 0;
                else if (this.getFitness()[2] > other.getFitness()[2])
                    return 1;
                else
                    return -1;

            else if (this.getFitness()[1] > other.getFitness()[1])
                return 1;
            else
                return -1;

        else if (this.getFitness()[0] > other.getFitness()[0])
            return 1;
        else
            return -1;
    }

    public float[] getFitness()
    {
        return new float[]{lap, checkpointPos, disToNextCheckpoint };
    }

    public float getNFitness()
    {
        return lap* 17 /* /!\/!\/!\ NUMBER OF CHECKPOINTS /!\/!\/!\ */ + checkpointPos + (-1/5)*disToNextCheckpoint+1 ;
    }
}