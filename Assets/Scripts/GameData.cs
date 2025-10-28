using UnityEngine;

namespace Survivor
{
    public enum MENU_STATE { NONE, IN_GAME, GAME_OVER };

    public class GameData
    {
        public Vector2[] EnemyPosition;

        public Vector2 PlayerDirection;

        public float GameTime;
        public float BestTime;

        public MENU_STATE MenuState = MENU_STATE.NONE;
    }
}