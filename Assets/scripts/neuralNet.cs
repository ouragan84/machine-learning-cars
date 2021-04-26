using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class neuralNet {

    public int[] layers;
    public float [][] bias;
    public float[][][] weight;
    private float reward;

    public neuralNet(int[] lay)
    {

        reward = 0;

        layers = lay;


        bias = new float[layers.Length][];

        for (int i = 0; i < layers.Length; i++)
            bias[i] = new float[layers[i]];


        weight = new float[layers.Length][][];

        for (int i = 0; i < layers.Length; i++)
            weight[i] = new float[layers[i]][];
        
        for (int i = 1; i < weight.Length; i++)
            for (int j = 0; j < weight[i].Length; j++)
                weight[i][j] = new float[layers[i - 1]];

    }

    public float[][][] getWeights()
    {
        return weight;
    }

    public float[][] getBias()
    {
        return bias;
    }

    public void initiateRandom(float biasRange, float weightRange)
    {

        for(int i = 1; i < bias.Length; i++)
        {
            for (int j = 0; j < bias[i].Length; j++)
            {
                bias[i][j] = (UnityEngine.Random.value)* biasRange * 2 - biasRange;
            }
        }

        for (int i = 1; i < weight.Length; i++)
        {
            for (int j = 0; j < weight[i].Length; j++)
            {
                for (int k = 0; k < weight[i][j].Length; k++)
                {
                    weight[i][j][k] = (UnityEngine.Random.value) * weightRange * 2 - weightRange;
                }
            }
        }
    }

    public void initiate(float[][] bias, float[][][] weight)
    {

        //this.bias = bias;
        //this.weight = weight;

        for (int i = 1; i < bias.Length; i++)
        {
            for (int j = 0; j < bias[i].Length; j++)
            {
                this.bias[i][j] = bias[i][j];
            }
        }

        for (int i = 1; i < weight.Length; i++)
        {
            for (int j = 0; j < weight[i].Length; j++)
            {
                for (int k = 0; k < weight[i][j].Length; k++)
                {
                    this.weight[i][j][k] = weight[i][j][k];
                }
            }
        }

    }

    public void mutate(float percent)
    {

        for (int i = 1; i < bias.Length; i++)
        {
            for (int j = 0; j < bias[i].Length; j++)
            {
                bias[i][j] += (UnityEngine.Random.Range(0, 2) * 2 - 1) * (UnityEngine.Random.value) * Mathf.Abs(bias[i][j]) * (percent/100);
            }
        }

        for (int i = 1; i < weight.Length; i++)
        {
            for (int j = 0; j < weight[i].Length; j++)
            {
                for (int k = 0; k < weight[i][j].Length; k++)
                {
                    weight[i][j][k] += (UnityEngine.Random.Range(0, 2) * 2 - 1) * (UnityEngine.Random.value) * Mathf.Abs(weight[i][j][k]) * (percent / 100);
                }
            }
        }

    }

    public float[] input(float[] inputVal)
    {
        //String s = "in ";
        List<float> activation = new List<float>();
        for(int i = 0; i < layers[0]; i++)
        {
            activation.Add(inputVal[i]);
            //s += inputVal[i] + " ";
        }
        //Debug.Log(s);

        for (int i = 1; i < layers.Length; i++)
        {
            List<float> temp = new List<float>();
            for (int j = 0; j < layers[i]; j++)
            {
                float c = 0.0f;
                for (int k = 0; k < weight[i][j].Length; k++)
                {
                    c += weight[i][j][k] * activation[k];
                }
                temp.Add((float)System.Math.Tanh(((double)bias[i][j]+c)));
            }
            activation = temp;
        }
        return activation.ToArray();
    }

    public double getReward()
    {
        return reward;
    }

    public void addReward(float amount)
    {
        reward += amount;
    }

    //a.compareTo(b) -> a-b
    public int compareTo(neuralNet other)
    {
        if(this.getReward() > other.getReward())
            return 1;
        if (this.getReward() < other.getReward())
            return -1;
        return 0;
    }

}