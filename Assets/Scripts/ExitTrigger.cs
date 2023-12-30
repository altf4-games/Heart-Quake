using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(EnemyManager.instance.activeRobots.Count == 0)
        {
            Debug.Log("Load next scene");
        }
    }
}
