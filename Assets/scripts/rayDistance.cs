using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayDistance : MonoBehaviour {

    public float maxDis;
    public float disToWall;

	// Use this for initialization
	public float getDistance () {
        return disToWall;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        disToWall = 0;

        Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDis);

        List<float> dis = new List<float>();
        bool b = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                dis.Add(hit.distance);
                b = true;
            }
        }

        if (b)
        {
            float min = maxDis;
            for (int i = 0; i < dis.Count; i++)
            {
                if (dis[i] < min) min = dis[i];
            }
            disToWall = min/maxDis;
            Debug.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * maxDis, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * maxDis, Color.green);
            disToWall = 1;
        }
	}
}

