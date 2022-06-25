using System.Security.Cryptography;
using UnityEngine;

public delegate void Rotator();
public enum PillEnum{Red,Blue,Yellow,White};
public class Pill : MonoBehaviour
{
    public bool m_canRotate = true;
    public bool IsVertical = false; //<=
    public Sprite m_pillDestroyed;

    public Rotator Rotate;
    public Rotator RevRotate;
    public static string m_direction = "Right";
    public Pill m_pill;

    void Start()
    {
        SetRotation(m_direction);
    }

    //TODO: spawn pill test

    public Pill CreatePill()
    {
        Pill pill = Instantiate(m_pill, transform.position, Quaternion.identity);
        foreach (PillPart child in pill.GetComponentsInChildren<PillPart>())
        {
            
        }

        return pill;
    }
    
    
    //
    public void SetRotation(string direction)
    {
        m_direction = direction;
        switch (direction)
        {
            case "Right":
                Rotate = RotateRight;
                RevRotate = RotateLeft;
                break;
            case "Left":
                Rotate = RotateLeft;
                RevRotate = RotateRight;
                break;
            default:
                Rotate = RotateRightAlt;
                break;
        }
    }

    public  static  PillStaticData[] PillData { get;} = 
    {
        new PillStaticData(100,18, 10, 0, Color.red), //red
        new PillStaticData(101, 18, 10, 0, new Color(0.40f, 0f, 1f, 1f)), //blue
        new PillStaticData(102,18, 10, 0, Color.yellow), //yellow
        new PillStaticData(103,18, 10, 0, Color.white), //White
    };

    void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;

    }

    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
        
    }

    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }

    void RotateLeft()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
    void RotateRight()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }

    void RotateRightAlt()
    {
        //transform.Rotate(0, 0, 90);
        if (IsVertical)
        {
            if (m_canRotate) transform.Rotate(0, 0, -90);
            IsVertical = false;
        }
        else
        {
            if (m_canRotate) transform.Rotate(0, 0, 90);
            IsVertical = true;
        }
    }

    //public void RotateLeftAlt()
    //{
    //    //transform.Rotate(0, 0, 90);
    //    if (IsVertical)
    //    {
    //        if (m_canRotate) transform.Rotate(0, 0, 90);
    //        IsVertical = false;
    //    }
    //    else
    //    {
    //        if (m_canRotate) transform.Rotate(0, 0, 90);
    //        IsVertical = true;
    //    }
    //}

    public void FlipPill()
    {
        foreach (Transform child in transform)
        {
            if (child.localPosition.x == 0)
            {
                child.localPosition = new Vector2(1f, 0f);
                if (child.localRotation.y == 0)
                {
                    child.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    child.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                child.localPosition = new Vector2(0f, 0f);
                if (child.localRotation.y == 0)
                {
                    child.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    child.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }


    public void ChangeSprite()
    {
        foreach (SpriteRenderer child in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            child.sprite = m_pillDestroyed;
           
        }
    }
    public class PillStaticData
    {
        public PillStaticData(int id,int pos, int chance, int maxChance, Color color)
        {
            Id = id;
            PositionOnGrid = pos;
            Chance = chance;
            MaxChance = maxChance;
            Color = color;
        }

    
        public int Id { get; }
        public int PositionOnGrid { get; set; }
        public int Chance { get; set; }
        public int MaxChance { get; }
        public Color Color { get; }

        
    }
}