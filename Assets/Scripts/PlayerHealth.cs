using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health = 10.0f;
    private bool dead = false;

    private void Update()
    {
        if (dead) return;
        if (health <= 0.0f)
        {
            RestartLevel();
            dead = true;
        }
    }

    public void DamagePlayer(float amnt)
    {
        health -= amnt;
    }

    private void RestartLevel()
    {
        Debug.Log("Fade");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
