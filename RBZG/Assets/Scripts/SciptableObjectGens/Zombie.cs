using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zombie", menuName = "Zombie")]
public class Zombie : ScriptableObject
{
    [Tooltip("The prefab of the Zombie type.")]
    public GameObject prefab;

    [Tooltip("Random animation controller overrides")]
    public List<AnimatorOverrideController> animators;

    [Tooltip("The radius of the sphere around the player where zombies will run to a random point within.")]
    public float chasePlayerRadius = 10.0f;

    [Tooltip("The minimum time in seconds before the zombie updates the player's position.")]
    public float minReactionTime = 1.0f;

    [Tooltip("The maximum time in seconds before the zombie updates the player's position.")]
    public float maxReactionTime = 3.0f;

    [Tooltip("The minimum speed at which the zombie moves.")]
    public float minMoveSpeed = 1.0f;

    [Tooltip("The maximum speed at which the zombie moves.")]
    public float maxMoveSpeed = 5.0f;

    [Tooltip("The distance at which the zombie will stop running to random points and run directly to the player.")]
    public float stopChaseDistance = 5.0f;

    [Tooltip("The distance at which the zombie will attack the player.")]
    public float attackRange = 1.0f;

    [Tooltip("Damage the zombie deals.")]
    public int damage = 25;

    [Header("Zombie Type")]
    [Tooltip("Is it a walker zombie.")]
    public bool isWalker;
    [Tooltip("Is it a jogging zombie.")]
    public bool isJogger;
    [Tooltip("Is it a running zombie.")]
    public bool isRunner;
    [Tooltip("Is it a boss zombie.")]
    public bool isBoss;

}