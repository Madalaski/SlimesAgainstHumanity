using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballDetectionField : MonoBehaviour
{

    public bool enemy = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(enemy && other.tag == "Player")
        {
            //FIREBALL KNOCKBACK
            Debug.Log("Fireball knockback");

            other.GetComponent<PlayerHandler>().PushPlayer(((other.transform.position - transform.position).normalized + Vector3.up*1f) * 60f);

            Destroy(gameObject);
        }
        else if (!enemy && other.tag == "Slime") {
            other.GetComponentInParent<SlimeMover>().SetSlimeVelocity(((other.transform.position - transform.position).normalized + Vector3.up*0.5f) * 40f);
            Destroy(gameObject);
        }
        else if (other.tag == "Ground") {
            Destroy(gameObject);
        }
    }
}
