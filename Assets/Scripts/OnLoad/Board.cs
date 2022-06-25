using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;

    public static int m_height = 30;
    public static int m_width = 8;
    public static int m_header = 14;
    public static int m_freeSpace = 3;
    Transform[,] m_grid;

    public Transform[,] Grid => m_grid;

    private void Awake()
    {
        m_grid = new Transform[m_height, m_width];
    }

    void Start()
    {
        DrawEmptyCells();

        //DrawPills();
    }

    void DrawEmptyCells()
    {
        if (!m_emptySprite)
        {
            Debug.LogError("m_emptySprite is missing");
            return;
        }

        for (int y = 0; y < m_height - m_header; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                Transform clone;
                clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
                clone.name = $"Board Space(x = {x}) , y = {y})";
                clone.transform.parent = transform;
            }
        }
    }

    public bool IsValidPosition(Pill pill)
    {
        foreach (Transform child in pill.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            if (!IsWithinBoard((int) pos.x, (int) pos.y))
            {
                return false;
            }

            if (IsOcupied((int) pos.x, (int) pos.y, pill))
            {
                return false;
            }
        }

        return true;
    }


    public static bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0);
    }

    bool IsOcupied(int x, int y, Pill pill)
    {
        return (m_grid[y,x] != null && m_grid[y,x].parent != pill.transform);
    }

    public void StoreVirusInGrid(Virus virus)
    {
        if (virus == null)
        {
            Debug.LogError("Virus is missing");
            return;
        }

        Vector2 pos = VectorF.Round(virus.transform.position);
        m_grid[(int) pos.y, (int) pos.x] = virus.transform;
    }

    public void StorePillInGrid(Pill pill)
    {
        if (pill == null)
        {
            Debug.LogError("Pill is missing.");
            return;
        }

        foreach (Transform child in pill.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            m_grid[(int) pos.y, (int) pos.x] = child;
        }
    }

    public async Task<List<Transform>> GetPillsToCheck() //<- test
    {
        List<Transform> floatingPillsList = new List<Transform>(GetFloatingPills());
        await ShiftPillsDown(floatingPillsList);
        return floatingPillsList;
    }

    private async Task ShiftPillsDown(List<Transform> floatingPills )
    {
        List<Transform> floatingPillsCopy = new List<Transform>(floatingPills);
        while (floatingPillsCopy.Count > 0)
        {
            await Task.Delay(150);
            for (int index = 0; index < floatingPillsCopy.Count; index++)
            {
                if (TestIsValidPositionBelow(floatingPillsCopy[index].gameObject))
                {
                    ShiftFloatingPillDown(floatingPillsCopy[index]);
                }
                else
                {
                    floatingPillsCopy.RemoveAt(index--);
                }
            }
        }
    }

    private HashSet<Transform> GetFloatingPills()
    {
        HashSet<Transform> floatPillObjects = new HashSet<Transform>();
        foreach (Transform itemInGrid in m_grid)
        {
            if (itemInGrid)
            {
                if (itemInGrid.CompareTag("PillHalf"))
                {
                    if (TestIsValidPositionBelow(itemInGrid.parent.gameObject))
                    {
                        floatPillObjects.Add(itemInGrid.parent.transform);
                        ShiftFloatingPillDown(itemInGrid.parent.transform);
                    }
                }
            }
        }

        return floatPillObjects;
    }


    void ShiftFloatingPillDown(Transform itemInGrid ,float time=1f)
    {
        
        foreach (Transform child in itemInGrid.transform)
        {
            Vector2 childPos = VectorF.Round(child.position);
            m_grid[(int) childPos.y, (int) childPos.x] = null;
        }

        itemInGrid.GetComponent<Pill>().MoveDown();
        StorePillInGrid(itemInGrid.GetComponent<Pill>());
        
    }

    public bool TestIsValidPositionBelow(GameObject pill)
    {
        
        foreach (Transform child in pill.transform)
        {
            Vector2 childPos = VectorF.Round(child.position);
            int childPosX=(int)childPos.x;
            int childPosY=(int)childPos.y-1;
            if (!IsWithinBoard(childPosX,childPosY))
            {
                return false;
            }

            if (IsOcupied(childPosX,childPosY, pill.GetComponent<Pill>()))
            {
                return false;
            }
        }

        return true;
    }
    
    public void DestroyTargets(List<IPillVirusBehaviours> ObjectToDestroy)
    {
        Int16 virusKilled = 0;
        foreach (IPillVirusBehaviours objToDestroy in ObjectToDestroy)
        {
            Transform parent = objToDestroy.getTransform().parent;
            objToDestroy.getTransform().parent = null;
            Vector2 objPos = VectorF.Round(objToDestroy.getTransform().position);
            m_grid[(int) objPos.y, (int) objPos.x]= null;
            if (objToDestroy.getTransform().CompareTag("Virus")) virusKilled++;
            objToDestroy.ThisDestroy();
            if (parent.childCount == 0)
            {
                DestroyImmediate(parent.gameObject);
            }
            else
            {
                if (parent.CompareTag("Pill"))
                {
                    parent.GetComponent<Pill>().ChangeSprite();
                }
            }
        }
        ScoreManager.AddKilledVirus(virusKilled);
    }

    public bool IsOverLimit(Pill pill)
    {
        byte childsOutOfBound = 0;
        foreach (Transform child in pill.transform)
        {
            if (child.position.y>=(m_height-m_header-1))
            {
                childsOutOfBound++;
            }
        }

        return childsOutOfBound == 2;
    }
}