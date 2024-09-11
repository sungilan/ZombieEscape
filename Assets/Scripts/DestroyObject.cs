using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Destroy),2f);
    }

    private void Destroy()
    {
        ObjectPoolManager.Instance.ReturnObject("Cartridge Case", this.gameObject);
    }

}
