
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject completeLevelUI;
    public AudioSource music;

    private void Start()
    {
        music.Play();
    }

    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
    }
}
