using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherScript : MonoBehaviour
{
    float perSlimeScore = 100f;
    [SerializeField] AudioClip[] deathScreams;
    [SerializeField] AudioClip[] squelchDeathNoises;


    GameController gameController;
    float deathFadeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Slime") {
            GameController.current.AddScore(perSlimeScore);
            GameController.current.AdvanceMultiplier();
            Destroy(other.transform.parent.parent.gameObject);
            AudioSource.PlayClipAtPoint(deathScreams[Random.Range(0, deathScreams.Length)], other.transform.position);
            AudioSource.PlayClipAtPoint(squelchDeathNoises[Random.Range(0, squelchDeathNoises.Length)], other.transform.position);
        }    
        else if(other.tag == "Player")
        {
            GameController.current.PlayerDeath();
        }
    }


}
