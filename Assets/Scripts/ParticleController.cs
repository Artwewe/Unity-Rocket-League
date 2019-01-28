
using UnityEngine;

public class ParticleController : MonoBehaviour {

    public ParticleSystem particles;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKey("j"))
        {
            particles.Emit(1);
        }
	}
}
