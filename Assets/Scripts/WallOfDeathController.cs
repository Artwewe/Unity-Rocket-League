
using UnityEngine;

public class WallOfDeathController : MonoBehaviour {

    public float wallSpeed;
    public bool active;
    public bool wallReset = false;
    private Vector3 originalPos;


    private void Start()
    {
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        if (active)
        {
            transform.Translate(Vector3.forward * wallSpeed * Time.deltaTime);
        }

        if (wallReset)
        {
            transform.position = originalPos;
            wallReset = false;
        }
    }



}
