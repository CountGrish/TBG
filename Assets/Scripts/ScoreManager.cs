using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static UInt64 score;
    private static UInt64 highscore;
    private static int m_virusKilled;
    public Text m_scoreText;
    public Text m_highScoreText;
    public void InitializeScore()
    {
        score = 0;
        highscore = LoadHighScore();
        m_highScoreText.text = PadZeros(highscore, 6);
        m_virusKilled = 0;
    }
    public static void AddKilledVirus(int viruses)
    {
        m_virusKilled += viruses;
    }
    public void UpdateScore(int Speedmultiplier)
    {

        switch (m_virusKilled)
        {
            case 0:
                return;
            case 1:
                score += 10 * (UInt64) Speedmultiplier;
                break;
            default:
                score +=(UInt64) (10 * Math.Pow(2, m_virusKilled)) * (UInt64) Speedmultiplier;
                break;
        }
        m_scoreText.text = PadZeros(score, 6);
        if (score>highscore)
        {
            highscore = score;
            m_highScoreText.text = PadZeros(score, 6);
        }
        m_virusKilled = 0;
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("Highscore",(int)highscore);
    }

    private UInt64 LoadHighScore()
    {
       return (UInt64) PlayerPrefs.GetInt("Highscore",0);
       
    }
    private string PadZeros(UInt64 number, int padDigits)
    {
        string nStr = number.ToString();
        while (nStr.Length<padDigits)
        {
            nStr = "0" + nStr;
        }

        return nStr;
    }

}
