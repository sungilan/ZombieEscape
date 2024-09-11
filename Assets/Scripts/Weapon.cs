using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string weaponName;   
    [SerializeField] protected float range;         
    [SerializeField] protected int damage;
    [SerializeField] protected string attackSound;
    [SerializeField] protected Sprite hudIcon;      

    public Animator anim;

    protected abstract void UseWeapon();
   
    public string GetweaponName()
    {
        return weaponName;
    }
    public Sprite GethudIcon()
    {
        return hudIcon;
    }
}






