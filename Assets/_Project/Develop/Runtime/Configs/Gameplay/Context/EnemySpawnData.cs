using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    [Serializable]
    public struct EnemySpawnData
    {
        public EntityConfig Config;
        public Vector3 Position;
        public Quaternion Rotation;

        public EnemySpawnData(EntityConfig config, Vector3 position, Quaternion rotation)
        {
            Config = config;
            Position = position;
            Rotation = rotation;
        }
    }
}