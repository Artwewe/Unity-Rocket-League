using UnityEngine;

public class EndTrigger : MonoBehaviour {

    public GameController GameController;

    private void OnTriggerEnter(Collider other)
    {
        GameController.CompleteLevel();
    }

}
