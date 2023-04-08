using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{

    [SerializeField] int damage;
    [SerializeField] int timer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    public void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<Collider>().GetComponent<IDamage>();
        if(damageable != null )
        {
            damageable.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    
}
