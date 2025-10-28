using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Survivor
{
    public class Board : MonoBehaviour
    {
        public GameObject Player;
        public GameObject EnemyPrefab;
        public Transform SpriteParent;

        GameObject[] m_enemyPool;
        Camera m_mainCamera;
        Vector2 m_mouseDownPos;

        public GameObject InputCircleOut;
        public GameObject InputCircleIn;

        public GameObject UI;
        public TextMeshProUGUI GameTimeText;

        // Start is called before the first frame update
        public void Init(Balance balance)
        {
            m_enemyPool = new GameObject[balance.NumEnemies];
            for (int i = 0; i < balance.NumEnemies; i++)
            {
                m_enemyPool[i] = Instantiate(EnemyPrefab, SpriteParent);
                m_enemyPool[i].SetActive(false);
            }

            InputCircleOut.SetActive(false);

            Player.SetActive(false);

            UI.SetActive(false);
        }

        public void Show(
            GameData gameData,
            Balance balance,
            Camera mainCamera)
        {
            m_mainCamera = mainCamera;
            Logic.StartGame(gameData, balance);

            for (int i = 0; i < balance.NumEnemies; i++)
            {
                m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];
                m_enemyPool[i].SetActive(true);
            }
            Player.SetActive(true);

            InputCircleOut.SetActive(false);

            UI.SetActive(true);
        }

        public void Hide(Balance balance)
        {
            for (int i = 0; i < balance.NumEnemies; i++)
                m_enemyPool[i].SetActive(false);
            Player.SetActive(false);

            UI.SetActive(false);
        }

        // Update is called once per frame
        public void Tick(GameData gameData, Balance balance, float dt)
        {
            handleInput(gameData);

            bool gameOver;
            Logic.Tick(gameData, balance, dt, out gameOver);

            if (!gameOver)
            {
                for (int i = 0; i < balance.NumEnemies; i++)
                    m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];

                GameTimeText.text = getTimeElapsedString(gameData.GameTime);

            }
            else
                Game.Instance.GameOver();
        }

        string getTimeElapsedString(float time)
        {
            string timeString = "";
            int m = Mathf.FloorToInt(time / 60.0f);
            int s = Mathf.FloorToInt(time - m * 60.0f);
            if (m >= 10)
                timeString += m;
            else
                timeString += "0" + m;
            timeString += ":";
            if (s >= 10)
                timeString += s;
            else
                timeString += "0" + s;

            return timeString;
        }

        void handleInput(GameData gameData)
        {
#if UNITY_EDITOR
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseMove = Input.GetMouseButton(0);
            bool mouseUp = Input.GetMouseButtonUp(0);
            Vector3 mousePosition = Input.mousePosition;
#else
bool mouseDown = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Began;
bool mouseMove = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Moved;
bool mouseUp = (Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled);
Vector3 mousePosition = Vector3.zero;
if (Input.touchCount > 0)
mousePosition = Input.GetTouch(0).position;
#endif
            Vector3 mouseWorldPos = m_mainCamera.ScreenToWorldPoint(mousePosition);
            Vector2 mouseLocalPos = SpriteParent.InverseTransformPoint(mouseWorldPos);

            if (mouseDown)
            {
                InputCircleOut.SetActive(true);
                m_mouseDownPos = mouseLocalPos;
                InputCircleOut.transform.position = m_mouseDownPos;
            }
            if (mouseMove)
            {
                Vector2 diff = (mouseLocalPos - m_mouseDownPos);
                float dist = diff.magnitude;
                if (dist > 1.0f)
                    dist = 1.0f;
                InputCircleIn.transform.localPosition = diff.normalized * dist * ((1.0f - InputCircleIn.transform.localScale.x) / 2.0f);
                Logic.MouseMove(gameData, m_mouseDownPos, mouseLocalPos);
            }
            if (mouseUp)
            {
                InputCircleOut.SetActive(false);
                Logic.MouseUp(gameData);
            }
        }
    }
}