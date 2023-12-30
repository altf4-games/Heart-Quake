using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public GameObject canvas;
    public GameObject eventSystem;
    private float health = 10.0f;
    private bool dead = false;

    private void Update()
    {
        if (dead) return;
        if (health <= 0.0f)
        {
            Instantiate(canvas);
            Instantiate(eventSystem);
            Invoke("RestartLevel", 1.0f);
            dead = true;
        }
    }

    public void DamagePlayer(float amnt)
    {
        health -= amnt;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
