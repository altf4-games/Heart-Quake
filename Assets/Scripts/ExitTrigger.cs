using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public AudioClip teleportClip;
    public AudioClip deniedClip;

    private void OnTriggerEnter(Collider other)
    {
        if(EnemyManager.instance.activeRobots.Count == 0)
        {
            AudioManager.instance.PlayAudio(teleportClip, 1.0f);
            Invoke("LoadNextScene", 1.5f);
            Debug.Log("Load next scene");
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
