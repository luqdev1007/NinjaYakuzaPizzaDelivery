using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "Configs/Characters/New Character Config", order = 54)]
public class CharacterConfig : ScriptableObject
{
    [field: SerializeField][Min(0)] public float MovementSpeed { get; private set; } = 10;
    [field: SerializeField][Min(0)] public float Health { get; private set; } = 10;
}