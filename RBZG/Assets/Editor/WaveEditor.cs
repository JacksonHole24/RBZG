using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;

[CustomEditor(typeof(ZombieSpawner))]
public class WaveEditor : Editor
{
    ZombieSpawner zombieSpawner;

    List<Wave> waves = new List<Wave>();

    void OnEnable()
    {
        zombieSpawner = (ZombieSpawner)target;

        waves = zombieSpawner.waveGenerator.waves;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        waves = zombieSpawner.waveGenerator.waves;
        if (GUILayout.Button("Add Wave Changes"))
        {
            Wave newWave = CreateInstance<Wave>();
            waves.Add(newWave);
            zombieSpawner.waveGenerator.waves = waves;
        }

        for(var i = 0; i < waves.Count; ++i)
        {
            waves[i].wave = EditorGUILayout.IntField(new GUIContent("Wave", "Wave you want these changes to take place"), waves[i].wave);
            waves[i].extraZombiesPerWave = EditorGUILayout.IntField(new GUIContent("Extra Zombies", "How many extra zombies you want to start spawning from this wave forward") ,waves[i].extraZombiesPerWave);
            waves[i].newMinJoggersPerWave = EditorGUILayout.Slider(new GUIContent("Min % of Joggers", "Percentage of zombies which you want to spawn as joggers from this wave forward minimum"), waves[i].newMinJoggersPerWave, 0f, 1f);
            waves[i].newMaxJoggersPerWave = EditorGUILayout.Slider(new GUIContent("Max % of Joggers", "Percentage of zombies which you want to spawn as joggers from this wave forward maximum"), waves[i].newMaxJoggersPerWave, 0f, 1f);
            waves[i].newMinRunnersPerWave = EditorGUILayout.Slider(new GUIContent("Min % of Runners", "Percentage of zombies which you want to spawn as runners from this wave forward minimum"), waves[i].newMinRunnersPerWave, 0f, 1f);
            waves[i].newMaxRunnersPerWave = EditorGUILayout.Slider(new GUIContent("Max % of Runners", "Percentage of zombies which you want to spawn as runners from this wave forward maximum"), waves[i].newMaxRunnersPerWave, 0f, 1f);

            if (GUILayout.Button("Save Wave"))
            {
                string newWavePath = "Assets/ScriptableObjects/Waves/Wave " + waves[i].wave + ".asset";
                AssetDatabase.CreateAsset(waves[i], newWavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                zombieSpawner.waveChanges.Add(waves[i]);
            }

            if (GUILayout.Button("Delete Wave"))
            {
                string newWavePath = "Assets/ScriptableObjects/Waves/Wave " + waves[i].wave + ".asset";
                zombieSpawner.waveChanges.Remove(waves[i]);
                AssetDatabase.DeleteAsset(newWavePath);
                waves.Remove(waves[i]);
                zombieSpawner.waveGenerator.waves = waves;
            }
        }

        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }
}