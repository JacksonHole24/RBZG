using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    [Tooltip("Wave you want these changes to take place")]
    public int wave;

    [Tooltip("How many extra zombies you want to start spawning from this wave forward")]
    public int extraZombiesPerWave;

    [Tooltip("Percentage of zombies which you want to spawn as joggers from this wave forward minimum")]
    public float newMinJoggersPerWave;

    [Tooltip("Percentage of zombies which you want to spawn as joggers from this wave forward maximum")]
    public float newMaxJoggersPerWave;

    [Tooltip("Percentage of zombies which you want to spawn as runners from this wave forward minimum")]
    public float newMinRunnersPerWave;

    [Tooltip("Percentage of zombies which you want to spawn as runners from this wave forward maximum")]
    public float newMaxRunnersPerWave;
}
