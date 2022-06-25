using UnityEngine;

public interface IPillVirusBehaviours
{
    void ThisDestroy();
    int ThisID { get; set; }
    bool IsEqualTo(int id);
    bool IsPrimaryColorCompatible();
    Transform getTransform();
    void changeSprite(Sprite newSprite);
    
}
