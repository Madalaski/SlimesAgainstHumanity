using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSlimeBehaviour : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject windHitBox;
    [SerializeField] GameObject center;

    public ParticleSystem particleSystem;
    float chillTime = 0.5f;
    float hitBoxMinRange = 1f;
    float maxHitBoxAlpha;
    float maxKnockBackForce = 100f;
    float knockUpForce = 10f;

    bool isAttackComplete = false;
    bool isInCoroutine = false;
    // Start is called before the first frame update
    void Start()
    {
        //MeshRenderer meshRenderer = windHitBox.GetComponent<MeshRenderer>();
        //Material material = meshRenderer.material;
        //maxHitBoxAlpha = material.color.a;
        //meshRenderer.material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;
        //int layerMask = ~LayerMask.GetMask("Slime","Hitbox");
        //Ray ray = new Ray(center.transform.position, player.transform.position - center.transform.position);
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    if (Physics.Raycast(ray, out hit, 150f, layerMask) && hit.transform.tag == "Player")
        //    {
        //        WindAttackBehaviour();
        //    }
        //}

    }

    public bool WindAttackBehaviour()
    {
        if (!isAttackComplete)
        {
            if (!isInCoroutine)
            {
                windHitBox.transform.position = center.transform.position + hitBoxMinRange * new Vector3(player.transform.position.x - center.transform.position.x, 0, player.transform.position.z - center.transform.position.z).normalized;
                windHitBox.transform.rotation = Quaternion.LookRotation(player.transform.position - center.transform.position, Vector3.zero);
                StartCoroutine(ChargeWind());
                isInCoroutine = true;
            }
            return false;
        }
        else
        {
            isAttackComplete = false;
            return true;
        }
        
    }

    IEnumerator ChargeWind()
    {
        float time = 0f;
        if (time < chillTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0f;
        //MeshRenderer meshRenderer = windHitBox.GetComponent<MeshRenderer>();
        //Material material = meshRenderer.material;
        //material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        
        while (time < 0.5f)
        {
            //material.color = new Color(material.color.r, material.color.g, material.color.b, maxHitBoxAlpha * time / 1.5f);
            time += Time.deltaTime;
            yield return null;
        }
        //material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        windHitBox.GetComponent<WindDetectionField>().WindKnockback(maxKnockBackForce, knockUpForce);
        particleSystem.Play();
        isAttackComplete = true;
        isInCoroutine = false;
    }
}
