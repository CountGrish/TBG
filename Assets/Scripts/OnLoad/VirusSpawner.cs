using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VirusSpawner : MonoBehaviour
{
    //public Transform m_Virus;
    Board m_gameBoard;
    string gameBoardTag = "Board";

    public Virus m_Virus;
    public int[,] m_gridRef;
    int virusCount; 
    int virusMaxChance=0;
    
    private void Awake()
    {
        m_gameBoard = GameObject.FindGameObjectWithTag(gameBoardTag).GetComponent<Board>();
        m_gridRef = new int[Board.m_height - Board.m_header, Board.m_width];

        virusCount = Virus.VirusData.Length;
        for (int index = 0; index < virusCount; index++)
        {
            virusMaxChance += Virus.VirusData[index].Chance;
        }
        
    }
    public  void DrawVirus(int level)
    {
        
        int virusesInLevel = (level + 1) * 4;
        int playerSpace = 3;
        int spawnHeight = virusesInLevel / Board.m_width + playerSpace;
        int maxVirusesInSpace = Board.m_width * spawnHeight;
        int[] forbiddenColors = new int[] { -1, -1 };

        for (int height = 0; height < spawnHeight; height++)
        {
            for (int width = 0; width < Board.m_width; width++)
            {
                m_gridRef[height, width] = -1; //initialize
                if (height > 1)
                {
                    if (m_gridRef[height - 1, width] == m_gridRef[height - 2, width]) //Down spot is same color with the previous(Down)
                    {
                        forbiddenColors[0] = m_gridRef[height - 1, width];
                    }
                }
                if (width > 1)
                {
                    if (m_gridRef[height, width - 1] == m_gridRef[height, width - 2]) //Left spot is same color with the previous(Left)
                    {
                        forbiddenColors[1] = m_gridRef[height, width - 1];
                        if (forbiddenColors[0] == forbiddenColors[1])
                        {
                            forbiddenColors[1] = -1;
                        }

                    }

                }
                m_gridRef[height, width] = RefVirus(forbiddenColors);
                forbiddenColors = new int[] { -1, -1 };
            }
        }
        //HashSet<int> virusToDeleteAt = new HashSet<int>(Randomizer.GetUniqueRandoms(maxVirusesInSpace, virusesInLevel));
        foreach (int v in Randomizer.GetUniqueRandoms(virusesInLevel, maxVirusesInSpace))
        {
            int y = v / Board.m_width;
            int x = v % Board.m_width;
            m_gridRef[y, x] = -1;
        }
        SpawnVirusAt(m_gridRef, Board.m_width, spawnHeight);
    }

      int RefVirus(int[] forbidenColors)
    {
        int nextVirus = 0;
        int max_chance = virusMaxChance - (forbidenColors[0] > -1 ? Virus.VirusData[forbidenColors[0]].Chance : 0) - (forbidenColors[1] > -1 ? Virus.VirusData[forbidenColors[1]].Chance : 0) + 1;
        int chance = Random.Range(1, max_chance);
        for (int index = 0; index < virusCount; index++)
        {
            if (forbidenColors[0] != index && forbidenColors[1] != index)
            {
                chance -= Virus.VirusData[index].Chance;
                if (chance <= 0)
                {
                    nextVirus = index;
                    break;
                }
            }
        }
        UpdateChances(nextVirus);
        return nextVirus;
    }

     void UpdateChances(int nextVirus)
    {
        int chance = (Virus.VirusData[nextVirus].Chance / 2);
        int chanceRemaining = chance % (virusCount - 1);
        chance /= virusCount - 1;
        Virus.VirusData[nextVirus].Chance = Virus.VirusData[nextVirus].Chance / 2 + Virus.VirusData[nextVirus].Chance % 2;
        if (chance > 0)
        {
            for (int index = 0; index < virusCount; index++)
            {
                if (index != nextVirus)
                {
                    Virus.VirusData[index].Chance += chance;
                }
            }
        }

        if (chanceRemaining > 0)
        {
            int index;
            while (chanceRemaining-- > 0)
            {
                do
                {
                    index = Random.Range(0, virusCount);
                } while (index == nextVirus);
                Virus.VirusData[index].Chance++;
            }
        }

    }
     void SpawnVirusAt(int[,] virusAt, int width, int height)
    {
        for (int y = 0; y < height; y++)

        {
            for (int x = 0; x < width; x++)
            {
                if (virusAt[y,x]!=-1)
                {
                    Virus virus;
                    virus = Instantiate(m_Virus, new Vector3(x, y, 0), Quaternion.identity);
                    virus.name = $"Virus(x = {x}) , y = {y})";
                    virus.transform.parent = transform;
                    virus.GetComponent<SpriteRenderer>().material.color = Virus.VirusData[virusAt[y, x]].Color;
                    virus.ThisID = Virus.VirusData[virusAt[y, x]].Id;
                    m_gameBoard.StoreVirusInGrid(virus);
                }


            }
        }


        //vClone = Instantiate(m_Virus)
    }
}
