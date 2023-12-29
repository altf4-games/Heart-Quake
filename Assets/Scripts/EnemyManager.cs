using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<RobotMG> activeRobots = new List<RobotMG>();

    private void Start()
    {
        instance = this;
        foreach (GameObject robot in GameObject.FindGameObjectsWithTag("RobotMG"))
        {
            activeRobots.Add(robot.transform.parent.GetComponent<RobotMG>());
        }
    }
}
