using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Generator", menuName = "Wave Generator")]
public class WaveGenerator : ScriptableObject
{
    public List<Wave> waves;
}
