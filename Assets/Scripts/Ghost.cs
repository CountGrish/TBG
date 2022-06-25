using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Pill m_ghost;
    private bool m_hitBottom;
    public Color m_color = new Color(1f,1f,1f,0.2f);

    public void DrawGhost(PillSpawner spawner)
    {
        m_ghost = Instantiate(m_ghost, spawner.transform.position, spawner.transform.rotation) as Pill;
        m_ghost.transform.parent = transform;
        m_ghost.gameObject.name = "GhostPill";
        SpriteRenderer[] childrendSprite = m_ghost.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer child in childrendSprite)
        {
            child.color = m_color;
        }
    }

    public void UpdateGhost(Pill activePill, Board gameBoard)
    {
        m_ghost.transform.position = activePill.transform.position;
        m_ghost.transform.rotation = activePill.transform.rotation;
        m_hitBottom = false;
        while (!m_hitBottom)
        {
            m_ghost.MoveDown();
            if (!gameBoard .IsValidPosition(m_ghost))
            {
                m_ghost.MoveUp();
                m_hitBottom = true;
            }
        }
    }
}
