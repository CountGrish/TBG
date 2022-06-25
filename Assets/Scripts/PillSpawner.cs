using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PillSpawner : MonoBehaviour
{
    public Pill m_pill;
    public Transform[] m_queuedXForms = new Transform[3];
    Vector3[] m_queuedXFormsPos = new Vector3[3];
    float m_queueScale = 0.75f;
    Vector3 m_queueOffset = new Vector3(-0.375f,0,0);
    Pill[] m_queuedPills = new Pill[3];
    private struct PillColor
    {
        public PillColor(Enum color)
        {
            id = Convert.ToInt32(color);
            chance = 10;
            if (id == 4)
            {
                chance = 0;
            }
        }
        
        public int ReduceChance()
        {
            int reduceBy = chance / 2;
            chance -= reduceBy;
            return reduceBy;
        }

        public void AddChance(int chance)
        {
            this.chance += chance;
        }

        private int id,chance;

        public int ID => id;

        public int Chance => chance;
    }

    //public Transform[] m_queueSquares = new Transform[3];
    void Awake()
    {
        InitQueue();
    }

    void Start()
    {
        for (int index = 0; index < m_queuedXForms.Length; index++)
        {
            m_queuedXFormsPos[index] = Camera.main.ScreenToWorldPoint(m_queuedXForms[index].position);
            m_queuedXFormsPos[index].z = 0;
        }
    }

    public Pill SpawnPill()
    {
        Pill pill = GetQueuedPill();
        pill.transform.position = transform.position;
        pill.transform.localScale = Vector3.one;

        return pill;
    }

    private void GenerateRandomPill(Pill pill)
    {
        foreach (PillPart child in pill.GetComponentsInChildren<PillPart>())
        {
            int chosenColor = getRandomColor();
            child.ThisID = chosenColor;
            child.GetComponent<SpriteRenderer>().color = Pill.PillData[chosenColor].Color;
        }
    }

    int getRandomColor()
    {
        if (Random.Range(0,99) < 5)
        {
            return Pill.PillData.Length - 1;
        }
        return Random.Range(0, Pill.PillData.Length -1);
        //return 0;
    }

    void InitQueue()
    {
        for (int i = 0; i < m_queuedPills.Length; i++)
        {
            m_queuedPills[i] = null;
        }
        FillQueue();
    }

    void FillQueue()
    {
        for (int i = 0; i < m_queuedPills.Length; i++)
        {
            if (!m_queuedPills[i])
            {
                m_queuedPills[i] = Instantiate(m_pill, transform.position, Quaternion.identity);
                GenerateRandomPill(m_queuedPills[i]);
                m_queuedPills[i].transform.position = m_queuedXFormsPos[i] + m_queueOffset;
                m_queuedPills[i].transform.localScale = new Vector3(m_queueScale,m_queueScale,m_queueScale);

            } 
        }
    }

    Pill GetQueuedPill()
    {
        Pill firstPill = null;
        if (m_queuedPills[0])
        {
            firstPill = m_queuedPills[0];

        }

        for (int i = 1; i < m_queuedPills.Length; i++)
        {
            m_queuedPills[i - 1] = m_queuedPills[i];
            m_queuedPills[i - 1].transform.position = m_queuedXFormsPos[i - 1] + m_queueOffset;
        }

        m_queuedPills[m_queuedPills.Length - 1] = null;
        FillQueue();
        return firstPill;
    }

    public void SwapPills(Pill currentPill, int index)
    {
        PillPart[] Pillchildren = new PillPart[4]
        {
            currentPill.transform.GetChild(0).GetComponent<PillPart>(),
            currentPill.transform.GetChild(1).GetComponent<PillPart>(),

            m_queuedPills[index].transform.GetChild(0).GetComponent<PillPart>(),
            m_queuedPills[index].transform.GetChild(1).GetComponent<PillPart>()
        };


        for (int i = 0; i < Pillchildren.Length / 2; i++)
        {
            int originalId = Pillchildren[i].ThisID;
            Pillchildren[i].ThisID = Pillchildren[i + 2].ThisID;
            Pillchildren[i].GetComponent<SpriteRenderer>().color = Pill.PillData[Pillchildren[i].ThisID].Color;

            Pillchildren[i+2].ThisID = originalId;
            Pillchildren[i+2].GetComponent<SpriteRenderer>().color = Pill.PillData[originalId].Color;
        }

        //Pill returnPill = m_queuedPills[index];
        //m_queuedPills[index] = currentPill;

        //m_queuedPills[index].transform.position = m_queuedXFormsPos[index] + m_queueOffset;
        //m_queuedPills[index].transform.localScale = new Vector3(m_queueScale,m_queueScale,m_queueScale);

        //return returnPill;
    }
}
