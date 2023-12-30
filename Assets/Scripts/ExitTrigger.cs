using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public AudioClip teleportClip;
    public AudioClip deniedClip;
    public GameObject canvas;
    public GameObject eventSystem;

    private void OnTriggerEnter(Collider other)
    {
        if(EnemyManager.instance.activeRobots.Count == 0)
        {
            AudioManager.instance.PlayAudio(teleportClip, 1.0f);
            Instantiate(canvas);
            Instantiate(eventSystem);
            Invoke("LoadNextScene", 1.5f);
        }
        else
        {
            AudioManager.instance.PlayAudio(deniedClip, 1.0f);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
