using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class SlimeMover : MonoBehaviour
{
    [SerializeField] float slimeJumpRange = 8f;
    float maxViewDistance = 150f;


    float maxJumpCooldownRate = 5f;
    float minJumpCooldownRate = 1.5f;

    float minJumpDistance = 2f;
    float maxJumpDistance = 5f;

    private bool isReadyToJump = false;
    private bool isInCooldownCoroutine = false;

    [SerializeField] GameObject[] bones;
    [SerializeField] public GameObject center;

    Vector3 vectorView;

    NavMeshPath path;
    Vector3 lastSavedDestination;

    float timer = 0f;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        //DEBUG TO DRWA PATH
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }

        if (isReadyToJump && CheckIfGrounded())
        {
            Jump();
            isReadyToJump = false;
        }
        if(!isReadyToJump && !isInCooldownCoroutine)
        {
            StartCoroutine(JumpCooldown());
            isInCooldownCoroutine = true;
        }

        if (timer < 0f) {
            timer = 0f;
            foreach(GameObject bone in bones)
            {
                bone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }

        timer -= Time.deltaTime;

    }


    public bool CheckIfGrounded()
    {
        foreach(GameObject bone in bones)
        {
            if (bone.GetComponent<SlimeBone>().isTouchingGround)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsInRange()
    {
        if (player == null) {
            return true;
        }

        if (Vector3.Distance(center.transform.position, player.transform.position)<slimeJumpRange) 
        {
            return true;
        }
        return false;
    }
    private void Jump()
    {

        Vector3 vectorToPlayer = player.transform.position - center.transform.position;

        //bool isHigherThanPlayer = player.transform.position.y < center.transform.position.y - 1f;
        //RaycastHit hit;
        //int layerMask = ~LayerMask.GetMask("Slime");
        //Ray ray = new Ray(center.transform.position, vectorToPlayer);
        
        if (vectorToPlayer.magnitude > slimeJumpRange)
        {
            Vector3 velocityToPoint = Vector3.zero;
            //Debug.Log(Physics.Raycast(ray, out hit, maxViewDistance, layerMask));
            //if (player.transform.position.y < center.transform.position.y - 1f 
            //    && Physics.Raycast(ray, out hit, maxViewDistance, layerMask)
            //    && hit.transform.tag == "Player")
            //{
            //    velocityToPoint = CalculateForceForParabalo(hit.transform.position);
            //}
            //else
            //{
                
            //}
            Vector3 pathDestination = GenerateJumpVector();
            velocityToPoint = CalculateForceForParabalo(pathDestination);
            if (velocityToPoint.magnitude > 500f) {
                velocityToPoint.Normalize();
            }
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].transform.GetComponent<Rigidbody>().AddForce(velocityToPoint, ForceMode.Impulse);
            }
        }
        
    }

    public void SetSlimeVelocity(Vector3 velocity)
    {
        foreach(GameObject bone in bones)
        {
            bone.GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    public void Freeze(float timeToFreeze) {
        timer = timeToFreeze;
        foreach(GameObject bone in bones)
        {
            bone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private Vector3 GenerateJumpVector()
    {
        //Weird bug for incomplete paths, that aren't getting filtered?
        NavMesh.CalculatePath(center.transform.position, player.transform.position, NavMesh.AllAreas, path);
        if(path.status != NavMeshPathStatus.PathComplete)
        {
            return lastSavedDestination;
        }
        else
        {
            lastSavedDestination = path.corners[1];
            
        }
            
        return path.corners[1];

        
    }
    public bool CanSlimeSeePlayer()
    {
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Slime","Hitbox","SlimeVertex");
        Ray ray = new Ray(center.transform.position, player.transform.position-center.transform.position);
        if(Physics.Raycast(ray,out hit, maxViewDistance, layerMask)&&hit.transform.tag=="Player")
        {
            return true;
        }
        return false;
    }
    Vector3 CalculateForceForParabalo(Vector3 target)
    {
        Vector3 vectorToTarget = target - center.transform.position;
        float yDiff = vectorToTarget.y;
        vectorToTarget.y = 0;
        float distance = vectorToTarget.magnitude;
        if (distance > maxJumpDistance)
        {
            distance = maxJumpDistance;
            vectorToTarget = vectorToTarget.normalized * distance;
        }
        else if(distance < minJumpDistance)
        {
            distance = minJumpDistance;
            vectorToTarget = vectorToTarget.normalized * minJumpDistance;
        }

        float force = Mathf.Sqrt(-Physics.gravity.y / ((Mathf.Sqrt(2f)/2f) * (distance - yDiff)));

        vectorToTarget = Quaternion.AngleAxis(-45f, Vector3.Cross(Vector3.up, vectorToTarget)) * vectorToTarget * force;
        vectorView = vectorToTarget;
        return vectorToTarget;
    }

    IEnumerator JumpCooldown()
    {
        float cooldownRate = UnityEngine.Random.Range(minJumpCooldownRate, maxJumpCooldownRate);
        yield return new WaitForSeconds(cooldownRate);
        isReadyToJump = true;
        isInCooldownCoroutine = false;
    }
}
