using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlimeState { Moving,Attacking};
public enum SlimeType { Wind, Ice, Fire, Earth};
public class SlimeStateManager : MonoBehaviour
{
    public SlimeState slimeState = SlimeState.Moving;
    [SerializeField] SlimeType slimeType;

    bool hasInitialisedState = false;

    [SerializeField] SlimeMover slimeMover;

    public Material innerSlimeMaterial;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (slimeState)
        {
            case SlimeState.Moving:
                if (!hasInitialisedState)
                {
                    slimeMover.enabled = true;
                    hasInitialisedState = true;
                }
                if (slimeMover.IsInRange())
                {
                    slimeState = SlimeState.Attacking;
                    slimeMover.enabled = false;
                    hasInitialisedState = false;
                }
                break;
            case SlimeState.Attacking:
                if (!hasInitialisedState)
                {
                    hasInitialisedState = true;
                }
                switch (slimeType)
                {
                    case SlimeType.Wind:
                        if (GetComponent<WindSlimeBehaviour>().WindAttackBehaviour())
                        {
                            slimeState = SlimeState.Moving;
                            hasInitialisedState = false;
                        }
                        
                        break;
                    case SlimeType.Ice:
                        GetComponent<IceSlimeBehaviour>().IceAttackBehaviour();
                        if (!slimeMover.IsInRange())
                        {
                            GetComponent<IceSlimeBehaviour>().StopIceBehaviour();
                            slimeState = SlimeState.Moving;
                            hasInitialisedState = false;
                        }
                        break;
                    case SlimeType.Earth:
                        if (GetComponent<EarthSlimeBehaviour>().EarthAttackBehaviour())
                        {
                            slimeState = SlimeState.Moving;
                            hasInitialisedState = false;
                        }
                        break;
                    case SlimeType.Fire:
                        if (slimeMover.IsInRange())
                        {

                            GetComponent<FireSlimeBehaviour>().FireAttackBehaviour();
                        }
                        else
                        {
                            slimeState = SlimeState.Moving;
                            hasInitialisedState = false;
                        }
                        
                        break;
                }
                break;
        }
    }

    public SlimeType CaptureSlime()
    {
        Destroy(this.gameObject,Time.deltaTime);
        return slimeType;
        
    }

}
