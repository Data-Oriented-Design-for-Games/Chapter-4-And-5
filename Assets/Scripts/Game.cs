using UnityEngine;
using CommonTools;
using System.Collections.Generic;
using TMPro;

namespace Survivor
{
    public class Game : Singleton<Game>
    {
        public Board Board;
        public Camera MainCamera;

        public GameObject UIMainMenu;
        public GameObject UIGameOver;

        // to be added in a future chapter
        // public TextMeshProUGUI CurrentTimeText; 
        // public TextMeshProUGUI BestTimeText;

        GameData m_gameData = new GameData();
        [SerializeField] Balance m_balance;

        int m_screenShotIdx = 0;

        // Start is called before the first frame update
        void Start()
        {
            Logic.AllocateGameData(m_gameData, m_balance);
            Board.Init(m_balance);

            UIMainMenu.SetActive(true);
            UIGameOver.SetActive(false);
        }

        public void StartGame()
        {
            UIMainMenu.SetActive(false);
            UIGameOver.SetActive(false);
            Board.Show(m_gameData, m_balance, MainCamera);
        }

        public void GameOver()
        {
            Board.Hide(m_balance);
            UIGameOver.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_gameData.MenuState == MENU_STATE.IN_GAME)
                Board.Tick(m_gameData, m_balance, Time.deltaTime);

            if (Input.GetKeyUp("s"))
                captureScreenshot();
        }
        void captureScreenshot()
        {
            ScreenCapture.CaptureScreenshot("screenshot" + (m_screenShotIdx++) + ".png");
        }
    }
}