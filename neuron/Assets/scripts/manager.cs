using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manager : MonoBehaviour {

    private car[] cars;
    private GameObject[] carsModels;
    private float[][][] bestWeight;
    private float[][] bestBias;
    private float bestFitness;
    public int carNumber;
    public int[] layers;
    public Vector3 newCarPos;
    public Vector3 newCarRot;
    public GameObject carsModel;
    public int generation;
    public int type1 = 7;
    public int type2 = 7;
    public int type3 = 7;
    public int type4 = 7;
    public int type5 = 2;
    private float lastTime = 10;
    public float generationLength;
    public int maxindex;
    private int bestGen;
    public Text text;

    // Use this for initialization
    void Start () {
        cars = new car[carNumber];
        carsModels = new GameObject[carNumber];
        GenerateFirst();
        bestFitness = 0;
        bestGen = 0;
    }
	
	// Update is called once per frame
	void Update () {
        writeGen();
        car max = cars[0];
        maxindex = 0;
		for(int i = 1; i < cars.Length; i++)
        {
            if (cars[i].compareFitnessTo(max) > 0)
            {
                max = cars[i];
                maxindex = i;
            }
                
        }

        if (max.getNFitness() > bestFitness /*|| bestGen + 5 < generation*/)
        {
            bestFitness = max.getNFitness();
            bestWeight = max.getWeights();
            bestBias = max.getBias();
            bestGen = generation;
        }

        bool alldead = true;
        for(int i = 0; i < cars.Length; i++)
        {
            if (carsModels[i].activeSelf)
            {
                if(cars[i].getNFitness() * 3 > max.getNFitness())
                    alldead = false;
            }
        }
        if (alldead || Time.time - generationLength >= lastTime)
        {
            nextWave();
        }
	}

    void GenerateFirst()
    {
        lastTime = Time.time;
        generation = 1;
        for (int i = 0; i < cars.Length; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 1, null, null);
        }
    }

    void nextWave()
    {
        lastTime = Time.time;
        generation++;
        destroyCars();

        generationLength = (float)System.Math.Min(60 , System.Math.Max(2 * System.Math.Log((double) bestFitness + 1 , 1.5d) + 5 , 5));

        for (int i = 0; i < type1; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 1, null, null);
        }

        for (int i = type1; i < type1 + type2; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 2, bestWeight, bestBias);
        }

        for (int i = type1 + type2; i < type1 + type2 + type3; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 3, bestWeight, bestBias);
        }

        for (int i = type1 + type2 + type3; i < type1 + type2 + type3 + type4; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 4, bestWeight, bestBias);
        }

        for (int i = type1 + type2 + type3 + type4; i < type1 + type2 + type3 + type4 + type5; i++)
        {
            carsModels[i] = (GameObject)Instantiate(carsModel);//new car(layers, 1, null, null);
            carsModels[i].transform.position = newCarPos;
            carsModels[i].transform.localEulerAngles = newCarRot;
            cars[i] = carsModels[i].GetComponent<car>();
            cars[i].instantiate(layers, 5, bestWeight, bestBias);
        }
    }

    void destroyCars()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            Destroy(carsModels[i]);
            cars[i] = null;
        }
    }

    void writeGen()
    {
        text.text = "Generation = " + generation + "\nTime = " + (Time.time - lastTime).ToString("F2") + "\nGlobal Time = "+ Time.time.ToString("F2") + "\nFPS = " + (1.0f/Time.deltaTime).ToString("F1");
    }
}