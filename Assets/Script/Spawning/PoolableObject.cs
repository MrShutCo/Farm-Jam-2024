using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    [Header("Pooling Properties")]
    public ObjectPool Parent;
    public virtual void OnDisable()
    {
        if (Parent == null)
        {
            Debug.Log("Parent is null - object likely not part of a pool", this.gameObject);
            return;
        }
        Parent.ReturnObjectToPool(this);
    }
    public void DisablePoolableObject()
    {
        gameObject.SetActive(false);
    }

}
