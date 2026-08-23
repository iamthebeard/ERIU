using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }

    protected virtual void CloseTrigger()
    {
        Destroy(gameObject);
    }
}
