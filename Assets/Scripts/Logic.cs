using UnityEngine;
using System;

namespace Survivor
{
    public static class Logic
    {
        public static void AllocateGameData(GameData gameData, Balance balance)
        {
            gameData.EnemyPosition = new Vector2[balance.NumEnemies];
        }

        public static void StartGame(GameData gameData, Balance balance)
        {
            gameData.MenuState = MENU_STATE.IN_GAME;
            gameData.GameTime = 0.0f;

            gameData.PlayerDirection = Vector2.zero;

            for (int i = 0; i < balance.NumEnemies; i++)
                gameData.EnemyPosition[i] = spawnEnemy(gameData, balance);
        }

        static Vector2 spawnEnemy(GameData gameData, Balance balance)
        {
            Vector2 direction = gameData.PlayerDirection;
            float angle = UnityEngine.Random.value * 180.0f - 90.0f;
            if (direction.magnitude == 0.0f)
            {
                direction = new Vector2(0.0f, 1.0f);
                angle = UnityEngine.Random.value * 360.0f;
            }
            direction = RotateVector(direction, angle);
            return direction.normalized * balance.SpawnRadius;
        }

        private const double DegToRad = Math.PI / 180.0d;
        private const double RadToDeg = 180.0d / Math.PI;

        public static Vector2 RotateVector(Vector2 a, double degrees)
        {
            double radians = degrees * DegToRad;
            double ca = Math.Cos(radians);
            double sa = Math.Sin(radians);
            a.x = (float)(ca * a.x - sa * a.y);
            a.y = (float)(sa * a.x + ca * a.y);
            return a;
        }

        public static void Tick(GameData gameData, Balance balance, float dt, out bool gameOver)
        {
            gameData.GameTime += dt;

            moveEnemies(gameData, balance, dt);

            checkEnemyOutOfBounds(gameData, balance);

            doEemyToEnemyCollision(gameData, balance);

            movePlayer(gameData, balance, dt);

            gameOver = checkGameOver(gameData, balance);
        }

        static void moveEnemies(GameData gameData, Balance balance, float dt)
        {
            for (int i = 0; i < balance.NumEnemies; i++)
            {
                Vector2 dir = -gameData.EnemyPosition[i].normalized;
                gameData.EnemyPosition[i] = gameData.EnemyPosition[i] + dir * balance.EnemyVelocity * dt;
            }
        }

        static void checkEnemyOutOfBounds(GameData gameData, Balance balance)
        {
            float distanceSqr = balance.SpawnRadius * balance.SpawnRadius * 1.1f;
            for (int i = 0; i < balance.NumEnemies; i++)
                if (gameData.EnemyPosition[i].sqrMagnitude > distanceSqr)
                    gameData.EnemyPosition[i] = spawnEnemy(gameData, balance);
        }

        static void doEemyToEnemyCollision(GameData gameData, Balance balance)
        {
            float diameter = balance.EnemyRadius + balance.EnemyRadius;
            float diameterSqr = diameter * diameter;
            for (int enemyIdx1 = 0; enemyIdx1 < balance.NumEnemies; enemyIdx1++)
            {
                for (int enemyIdx2 = enemyIdx1 + 1; enemyIdx2 < balance.NumEnemies; enemyIdx2++)
                {
                    Vector2 diff = gameData.EnemyPosition[enemyIdx1] - gameData.EnemyPosition[enemyIdx2];
                    if (diff.sqrMagnitude <= diameterSqr)
                    {
                        Vector2 diffNormalized = diff.normalized;
                        Vector2 midPoint = (gameData.EnemyPosition[enemyIdx1] + gameData.EnemyPosition[enemyIdx2]) / 2.0f;
                        gameData.EnemyPosition[enemyIdx1] = midPoint + diffNormalized * balance.EnemyRadius;
                        gameData.EnemyPosition[enemyIdx2] = midPoint - diffNormalized * balance.EnemyRadius;
                    }
                }
            }
        }

        static void movePlayer(GameData gameData, Balance balance, float dt)
        {
            Vector2 playerPosition = gameData.PlayerDirection * balance.PlayerVelocity * dt;
            for (int i = 0; i < balance.NumEnemies; i++)
                gameData.EnemyPosition[i] -= playerPosition;
        }

        static bool checkGameOver(GameData gameData, Balance balance)
        {
            for (int i = 0; i < balance.NumEnemies; i++)
                if (gameData.EnemyPosition[i].magnitude < balance.MinCollisionDistance)
                {
                    if (gameData.GameTime > gameData.BestTime)
                        gameData.BestTime = gameData.GameTime;
                    gameData.MenuState = MENU_STATE.GAME_OVER;

                    return true;
                }
            return false;
        }

        public static void MouseMove(GameData gameData, Vector2 mouseDownPos, Vector2 mouseCurrentPos)
        {
            gameData.PlayerDirection = (mouseCurrentPos - mouseDownPos).normalized;
        }

        public static void MouseUp(GameData gameData)
        {
            gameData.PlayerDirection = Vector2.zero;
        }
    }
}