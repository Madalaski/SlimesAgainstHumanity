using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public static Cube instance;

    public GameObject[] bones;


    public Vector3 cubeCentre;

    public SkinnedMeshRenderer smRenderer;
    public Mesh playerBakedMesh;

    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < bones.Length; i++) 
        {
            cubeCentre += bones[i].transform.position;
        }
        cubeCentre /= bones.Length;
        transform.position = cubeCentre;

        cubeCentre = Vector3.zero;

        CubeBakeMeshToCollider();

    }

    void CubeBakeMeshToCollider()
    {
        smRenderer.BakeMesh(playerBakedMesh);
        
    }
}
