using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VirusEnum {  Red, Blue, Yellow };
public class Virus : MonoBehaviour, IPillVirusBehaviours
{
    public static VirusStaticData[] VirusData = new VirusStaticData[]
    {
        new VirusStaticData(0, 10, 0, 50, Color.red), //red
        new VirusStaticData(1, 10, 0, 50, new Color(0.40f, 0f, 1f, 1f)), //blue
        new VirusStaticData(2, 10, 0, 50, Color.yellow) //yellow
    };
    public int ThisID { get; set; }
    public bool IsEqualTo(int id)
    {
        return ThisID == id || id == (int) PillEnum.White;
    }

    public void ThisDestroy()
    {
        Destroy(this.gameObject);
    }

    public bool IsPrimaryColorCompatible()
    {
        return ThisID != (int) PillEnum.White;
    }
    public Transform getTransform()
    {
        return transform;
    }
    public void changeSprite(Sprite newSprite)
    {
        SpriteRenderer sprite = transform.GetComponent<SpriteRenderer>();
        sprite.sprite = newSprite;
    }
    public class VirusStaticData
    {
        public int Id { get; }

        public int Chance { get; set; }

        public int MaxChance { get; }

        public int GiveScore { get; }

        public Color Color { get; }

        public VirusStaticData (int _id, int _chance, int _maxChance, int _giveScore, Color _color)
        {
            Id = _id;
            Chance = _chance;
            MaxChance = _maxChance;
            GiveScore = _giveScore;
            Color = _color;
            //Sprite = _sprite;

        }
    }
}

