using UnityEngine;
using UnityEngine.UI;

public class DistanceScript : MonoBehaviour {

    public Transform car;
    public Transform wall;
    private float distanceValue;
    public Text distance;
	
	// Update is called once per frame
	void Update () {

        distanceValue = car.position.z - wall.position.z;

        if (distanceValue < 20)
        {
            distance.color = Color.red;
            distance.fontSize = 25;
        }
        else
        {
            distance.color = Color.white;
            distance.fontSize = 20;
        }

        distance.text = distanceValue.ToString("0");

	}
}
