using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform liquidModel;
    public Transform lineSource;
    LineRenderer lineRenderer;

    Quaternion offsetRot;

    Vector3 initialOffset;

    public bool showLine = false;

    float ammo = 0f;

    float targetAmmo = 0f;

    SlimeType type = SlimeType.Earth;

    Material liquidMaterial;

    public Color[] slimeColors;

    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        initialOffset = transform.localPosition;
        liquidMaterial = liquidModel.GetComponent<MeshRenderer>().material;
        //offsetRot = Quaternion.FromToRotation(transform.forward, transform.parent.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (showLine) {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.GetChild(0).position);
            lineRenderer.SetPosition(1, lineSource.position);
        }
        else {
            lineRenderer.enabled = false;
        }
        

        transform.GetChild(0).Rotate(360f*Time.deltaTime*Vector3.right, Space.Self);

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialOffset, Time.deltaTime * 10f);

        ammo = Mathf.Lerp(ammo, targetAmmo, Time.deltaTime*10f);
        liquidMaterial.SetFloat("_FillAmount",Mathf.Lerp(0.8f, 0.28f, ammo));

        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
 
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * Time.time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * Time.time);
 
        liquidMaterial.SetFloat("_WobbleX", wobbleAmountX);
        liquidMaterial.SetFloat("_WobbleZ", wobbleAmountZ);
 
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;
 
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
 
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void LookInDirection(Vector3 direction) {
        transform.forward = Vector3.Lerp(transform.forward, direction, 0.125f);
    }

    public void Fire () {
        transform.localPosition = initialOffset + new Vector3(0f, 0.1f, -0.2f);
    }

    public void IncreaseSpeed () {
        transform.GetChild(0).Rotate(360f*Time.deltaTime*Vector3.right, Space.Self);
    }

    public void UpdateAmmo(float newAmmo, SlimeType newType) {
        targetAmmo = newAmmo;

        if (newType != type) {
            type = newType;
            Color baseColor = slimeColors[(int) type];
            Color foamColor = Color.Lerp(baseColor, Color.white, 0.5f);
            liquidMaterial.SetColor("_Tint", baseColor);
            liquidMaterial.SetColor("_TopColor", foamColor);
            liquidMaterial.SetColor("_FoamColor", foamColor);
        }
    }
}
