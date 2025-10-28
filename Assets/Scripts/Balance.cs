using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor
{
    [Serializable]
    public class Balance
    {
        public int NumEnemies;
        public float EnemyVelocity;
        public float EnemyRadius;
        public float SpawnRadius;
        public float PlayerVelocity;
        public float MinCollisionDistance;
    }
}