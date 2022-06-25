
using UnityEngine;

public class PillPart : MonoBehaviour, IPillVirusBehaviours
{
    public int ThisID { get; set; }
    public void ThisDestroy()
    {
        Destroy(this.gameObject);
    }

    public bool IsEqualTo(int id)
    {
        return ThisID == id || id == (int) PillEnum.White || ThisID == (int) PillEnum.White;
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
}
