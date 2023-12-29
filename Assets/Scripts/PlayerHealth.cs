using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float health = 10.0f;

    private void Update()
    {
        if (health <= 0.0f)
        {
            Debug.Log("TODO: IMPLEMENT GAME RESTART!");
        }
    }

    public void DamagePlayer(float amnt)
    {
        health -= amnt;
    }
}
