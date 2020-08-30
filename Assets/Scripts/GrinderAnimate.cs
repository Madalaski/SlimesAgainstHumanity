using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrinderAnimate : MonoBehaviour
{
    [SerializeField] Transform[] leftSpinObjects;
    [SerializeField] Transform[] rightSpinObjects;

    float rotationSpeed = 150f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform leftSpin in leftSpinObjects)
        {
            leftSpin.Rotate(transform.up, rotationSpeed*Time.deltaTime);
        }
        foreach (Transform rightSpin in rightSpinObjects)
        {
            rightSpin.Rotate(transform.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
