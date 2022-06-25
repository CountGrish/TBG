using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static byte m_level;
    public static float m_speed;
    public TextMeshProUGUI[] m_updatingText = new TextMeshProUGUI[2];
    public Slider[] sliders = new Slider[2];

    public void PlayGame()
    {
        m_level=(byte) sliders[0].value;
        switch ((byte) sliders[1].value)
        {
            case 1:
                m_speed = 0.9f;
                break;
            case 2:
                m_speed = 0.5f;
                break;
            case 3:
                m_speed = 0.4f;
                break;
            default:
                m_speed = 0.25f;
                break;

        }

        GameController.speedMultiplier = (int) sliders[1].value;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void UpdateText(int tag)
    {
        m_updatingText[tag].text = sliders[tag].value.ToString();
    }


    public void Quit()
    {
        Application.Quit();
    }
}
