using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Board m_gameBoard;
    string gameBoardTag = "Board";

    PillSpawner m_spawner;
    string pillSpawnerTag = "PillSpawner";

    VirusSpawner m_virusSpawner;
    string virusSpawnerTag = "VirusSpawner";

    private Ghost m_ghost;
    private string ghostTag = "Ghost";

    Pill m_activePill;

    public GameObject m_rotationImage;
    public Sprite m_deadCell;

    public GameObject m_gameOverPanel;

    public GameObject m_winPanel;
    public ScoreManager m_scoreManager;

    public GameObject m_pausePanel;

    public float m_dropInterval = 0.9f;
    float m_timeToDrop;

    public int m_userInput;

    private bool m_gameOver=false;
    public bool m_isPaused = false;
    private bool m_canSwap = false;

    public static int speedMultiplier { get; set; }


    [Range(0.02f, 1f)] public float m_keyRepeatRateRightLeft = 0.25f;
    float m_timeToNextKeyRightLeft;

    [Range(0.02f, 1f)] public float m_keyRepeatRateRotate = 0.25f;
    float m_timeToNextKeyRotate;

    [Range(0.01f, 1f)] public float m_keyRepeatRateDown = 0.25f;
    float m_timeToNextKeyDown;

    [Range(0,20)]
    public byte level = 0;

    private Rotator Rotate;

    public bool m_defaultRotation = true;
    // Start is called before the first frame update
    void Start()
    {

        m_gameBoard = GameObject.FindGameObjectWithTag(gameBoardTag).GetComponent<Board>();
        m_spawner = GameObject.FindGameObjectWithTag(pillSpawnerTag).GetComponent<PillSpawner>();
        m_virusSpawner = GameObject.FindGameObjectWithTag(virusSpawnerTag).GetComponent<VirusSpawner>();
        m_ghost = GameObject.FindGameObjectWithTag(ghostTag).GetComponent<Ghost>();
        m_virusSpawner.DrawVirus(MainMenu.m_level);
        m_dropInterval = MainMenu.m_speed;
        m_timeToNextKeyRightLeft = Time.time + m_keyRepeatRateRightLeft;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

        m_ghost.DrawGhost(m_spawner);
        if (m_spawner)
        {
            m_spawner.transform.position = VectorF.Round(m_spawner.transform.position);
            if (m_activePill == null)
            {
                m_activePill = m_spawner.SpawnPill();
            }
        }
        SetRotationType(m_defaultRotation);
        m_pausePanel.SetActive(false);
        m_scoreManager.InitializeScore();
        m_gameOverPanel.SetActive(false);
        m_winPanel.SetActive(false);
    }

    public void SetRotationType(bool DefaultRotation)
    {
        if (DefaultRotation)
        {
            Rotate = RotateDefault;
        }
        else
        {
            Rotate = RotateAlt;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!m_activePill || m_gameOver)
        {
             return;
        }
        //PlayerInput();
        uiKeyboard();
    }

    void LateUpdate()
    {
        if (m_activePill)
        {
            m_ghost.UpdateGhost(m_activePill,m_gameBoard);
        }
        
    }

    public void UserInput(int button)
    {
        m_userInput = button;
    }

    void PlayerInput()
    {
        if (m_userInput == 1 && Time.time > m_timeToNextKeyRightLeft)
        {
            m_activePill.MoveRight();
            m_timeToNextKeyRightLeft = Time.time + m_keyRepeatRateRightLeft;
            if (!m_gameBoard.IsValidPosition(m_activePill)) m_activePill.MoveLeft();
        }
        else if (m_userInput == 2 && Time.time > m_timeToNextKeyRightLeft)
        {
            m_activePill.MoveLeft();
            m_timeToNextKeyRightLeft = Time.time + m_keyRepeatRateRightLeft;
            if (!m_gameBoard.IsValidPosition(m_activePill)) m_activePill.MoveRight();
        }
        else if (m_userInput == 3 && Time.time > m_timeToNextKeyRotate)
        {
            Rotate();
        }
        else if (m_userInput == 4 && Time.time > m_timeToNextKeyDown || Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropInterval;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_activePill.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activePill))
            {
                if (m_gameBoard.IsOverLimit(m_activePill))
                {
                    GameOver();
                }
                else
                {
                    LandPill(m_activePill);
                }

            }
        }



    }

    private void uiKeyboard()
    {
        


        if (Input.GetButton("MoveRight") && Time.time > m_timeToNextKeyRightLeft || Input.GetButtonDown("MoveRight"))
        {
            m_activePill.MoveRight();
            m_timeToNextKeyRightLeft = Time.time + m_keyRepeatRateRightLeft;
            if (!m_gameBoard.IsValidPosition(m_activePill)) m_activePill.MoveLeft();
        }
        else if (Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyRightLeft || Input.GetButtonDown("MoveLeft"))
        {
            m_activePill.MoveLeft();
            m_timeToNextKeyRightLeft = Time.time + m_keyRepeatRateRightLeft;
            if (!m_gameBoard.IsValidPosition(m_activePill)) m_activePill.MoveRight();
        }
        else if (Input.GetButton("Rotate") && Time.time > m_timeToNextKeyRotate)
        {
            //bool canRotate = true;
            //m_activePill.Rotate();
            //m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            //if (!m_gameBoard.IsValidPosition(m_activePill))
            //{
            //    m_activePill.MoveLeft();
            //    if (!m_gameBoard.IsValidPosition(m_activePill))
            //    {
            //        m_activePill.MoveRight();
            //        m_activePill.Rotate();
            //        canRotate = false;
            //    }
            //}

            //if (canRotate & m_activePill.IsVertical) m_activePill.FlipPill();
            Rotate();
        }
        else if (Input.GetButton("MoveDown") && Time.time > m_timeToNextKeyDown || Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropInterval;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_activePill.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activePill))
            {
                if (m_gameBoard.IsOverLimit(m_activePill))
                {
                    GameOver();
                }
                else
                {
                    LandPill(m_activePill);
                }

            }
        }


    }
    private void RotateDefault()
    {
        m_activePill.Rotate();
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        if (!m_gameBoard.IsValidPosition(m_activePill))
        {
            m_activePill.RevRotate();
        }
    }

    private void RotateAlt()
    {
        bool canRotate = true;
        m_activePill.Rotate();
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        if (!m_gameBoard.IsValidPosition(m_activePill))
        {
            m_activePill.MoveLeft();
            if (!m_gameBoard.IsValidPosition(m_activePill))
            {
                m_activePill.MoveRight();
                m_activePill.Rotate();
                canRotate = false;
            }
        }

        if (canRotate & !m_activePill.IsVertical) m_activePill.FlipPill();
    }

    private void GameOver()
    {
        m_gameOver = true;
        m_activePill.MoveUp();
        m_gameOverPanel.SetActive(true);
    }

    void WinGame()
    {
        m_gameOver = true;
        m_winPanel.SetActive(true);
        m_scoreManager.SaveHighScore();

    }

    public void LandPill(Pill pill)
    {
        m_ghost.gameObject.SetActive(false);
        m_activePill = null;
        m_timeToNextKeyRightLeft = Time.time;
        m_timeToNextKeyRotate = Time.time;
        m_timeToNextKeyDown = Time.time;
        pill.MoveUp();
        m_gameBoard.StorePillInGrid(pill);
        CheckBoard(pill);
    }


    async void CheckBoard(Pill pill)
    {
        List<IPillVirusBehaviours> objToDestroy = new List<IPillVirusBehaviours>(); 
        foreach (Transform pillPart in pill.transform)
        {
            ObjectsToDelete newDeleteQueue = new ObjectsToDelete(m_gameBoard.Grid, pillPart);
            objToDestroy = objToDestroy.Union(newDeleteQueue.ObjsForDeletion).ToList();
        }
        while (objToDestroy.Count > 0)
        {
            m_canSwap = true;
            foreach (IPillVirusBehaviours obj in objToDestroy)
            {
                obj.changeSprite(m_deadCell);
            }
            await Task.Delay(500);
            m_gameBoard.DestroyTargets(objToDestroy);
            objToDestroy = new List<IPillVirusBehaviours>();
            List<Transform> PillsToCheckList = new List<Transform>(await m_gameBoard.GetPillsToCheck());
            foreach (Transform pillToCheck in PillsToCheckList)
            {
                foreach (Transform pillPart in pillToCheck)
                {
                    ObjectsToDelete newDeleteQueue = new ObjectsToDelete(m_gameBoard.Grid, pillPart);
                    objToDestroy = objToDestroy.Union(newDeleteQueue.ObjsForDeletion).ToList();

                }
            }
        }
        m_scoreManager.UpdateScore(speedMultiplier);
        if (!m_virusSpawner)
        {
            WinGame();
            return;
        }
        m_activePill = m_spawner.SpawnPill();
        m_ghost.gameObject.SetActive(true);
    }


    public void SwapPills(int index)
    {
        if (m_canSwap && m_activePill)
        {
            m_timeToNextKeyRightLeft = Time.time;
            m_timeToNextKeyRotate = Time.time;
            m_timeToNextKeyDown = Time.time;
            m_spawner.SwapPills(m_activePill,index);
            m_activePill.transform.position = m_spawner.transform.position;
            m_activePill.transform.rotation = Quaternion.identity;
            m_canSwap = false;
        }
        
    }

    #region UI Buttons

    public void TogglePause()
    {
        m_isPaused = !m_isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            Time.timeScale = (m_isPaused) ? 0 : 1;
        }


    }

    public void SetRotation(string orientation)
    {
        //switch (orientation)
        //{
        //    case "Left":
        //        m_rotationImage.transform.rotation=Quaternion.Euler(0f,0f,0f);
        //        break;
        //    default:
        //        m_rotationImage.transform.rotation =  Quaternion.Euler(0f,180f,0f);
        //        break;
        //}
        m_activePill.SetRotation(orientation);
    }
    public void Nextlevel()
    {
        MainMenu.m_level = (byte) (MainMenu.m_level >19 ? 20 : MainMenu.m_level + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    #endregion
}