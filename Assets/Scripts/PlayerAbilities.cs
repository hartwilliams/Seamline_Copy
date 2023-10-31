using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities _instance;
    //private DefaultInputAction playerInputAction;

    //private PlayerStats playerStatsInstance;
    //private PlayerMovement playerMovementInstance;


    [SerializeField] GameObject blockPrefab;
    private const float blockCD = 5f;
    private const float blockMovementSpeedMultiplier = 0.4f;
    private const float blockYarnPerSecond = 20f;

    [SerializeField] GameObject stunMeshPrefab;
    [SerializeField] AudioSource stitchingSFX;
    [SerializeField] Material lineMaterial;
    private const float stunCD = 5f;
    private const float stunDuration = 3f;
    private const float stunMaxCastingDuration = 20f;
    private const float stunYarnPerSecond = 5f;

    private bool stunCasting = false;
    private List<Vector3> stunCulledPath;
    private LineRenderer pathRender;

    private float lastStunAbilityTime;
    private float lastBlockAbilityTime;
    private bool isBlocking;
    private GameObject blockObject;



    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        GameObject pathRenderObj = new GameObject("PathRenderer", typeof(LineRenderer));
        pathRenderObj.transform.position = Vector3.zero;
        pathRender = pathRenderObj.GetComponent<LineRenderer>();
        pathRender.startWidth = 0.1f;
        pathRender.endWidth = 0.1f;
        if (lineMaterial)
        {
            pathRender.material = lineMaterial;
        }
        else
        {
            pathRender.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        pathRender.startColor = Color.magenta;
        pathRender.endColor = Color.magenta;
        
        //playerInputAction = new DefaultInputAction();
        //playerInputAction.Player.Ability1.started += startAbility;
        //playerInputAction.Player.Ability2.started += startAbility2;

        lastStunAbilityTime = -stunCD;
        lastBlockAbilityTime = -blockCD;

        stunCulledPath = new List<Vector3>(); // used for mechanics/ functionality

    }

    void FixedUpdate()
    {
        if (isBlocking)
        {
            PlayerStats._instance.UseYarn(blockYarnPerSecond * Time.fixedDeltaTime);
            if (PlayerStats._instance.currentYarnCount <= 0)
            {
                block();// cancel block
            }
        }

        if (stunCasting)
        {
            PlayerStats._instance.UseYarn(stunYarnPerSecond * Time.fixedDeltaTime);
            if (Time.time - lastStunAbilityTime < stunMaxCastingDuration && PlayerStats._instance.currentYarnCount>0)
            {
                //add new point
                Vector3 newPoint = transform.position;
                if (newPoint != stunCulledPath[stunCulledPath.Count - 1])
                {
                    if (stunCulledPath.Count >= 2)
                    {

                        //check if its inline with previous points for culled path
                        Vector3 oldDir = (stunCulledPath[stunCulledPath.Count - 1] - stunCulledPath[stunCulledPath.Count - 2]).normalized;
                        Vector3 newDir = (newPoint - stunCulledPath[stunCulledPath.Count - 1]).normalized;
                        if (oldDir != newDir && oldDir != newDir*-1f)
                        {
                            //print("adding new point");
                            stunCulledPath.Add(newPoint);
                        }
                        else
                        {
                            //replace previous point with new point
                            stunCulledPath[stunCulledPath.Count - 1] = newPoint;
                        }
                    }
                    else
                    {
                        //print("adding new point");
                        stunCulledPath.Add(newPoint);
                    }
                    
                }
            }
            else
            {
                //finish stun cast
                stun();
            }
            pathRender.positionCount = stunCulledPath.Count;
            pathRender.SetPositions(stunCulledPath.ToArray());
        }
    }
    
    //private void OnEnable()
    //{
    //    playerInputAction.Player.Ability1.Enable();
    //    playerInputAction.Player.Ability2.Enable();
    //}

    //private void OnDisable()
    //{
    //    playerInputAction.Player.Ability1.Disable();
    //    playerInputAction.Player.Ability2.Disable();
    //}

    //public void OnPause(bool paused)
    //{
    //    if (paused)
    //    {
    //        playerInputAction.Player.Ability1.Disable();
    //        playerInputAction.Player.Ability2.Disable();
    //    }
    //    else
    //    {
    //        playerInputAction.Player.Ability1.Enable();
    //        playerInputAction.Player.Ability2.Enable();
    //    }
    //}

    public void startBlock(InputAction.CallbackContext ctx)
    {
        print("block ability triggered");
        print(ctx.phase);
        if (Time.time - lastBlockAbilityTime > blockCD)
        { 
            block();
        }

    }
    public void startStun(InputAction.CallbackContext ctx)
    {
 
        if ( (Time.time - lastStunAbilityTime > stunCD || stunCasting) && ctx.started)
        {
            lastStunAbilityTime = Time.time;
            StartCoroutine(stun());
        }
    }

    private void block()
    {
        if (!isBlocking){
            isBlocking = true;
            PlayerMovement._instance.MultiplySpeed(blockMovementSpeedMultiplier);
            PlayerStats._instance.blocking = true;
            blockObject = Instantiate(blockPrefab) as GameObject;
            blockObject.transform.parent = this.transform;
            blockObject.transform.position = this.transform.position;
        }
        else
        {
            isBlocking = false;
            Destroy(blockObject);
            PlayerMovement._instance.MultiplySpeed(1f / blockMovementSpeedMultiplier);
            PlayerStats._instance.blocking = false;
            lastBlockAbilityTime = Time.time;
        }

        
    }

    private IEnumerator stun()
    {
        if (stunCasting)
        {
            if (stitchingSFX)
            {
                stitchingSFX.Stop();
            }
            //print("stun cast finished");
            stunCasting = false;
            GameObject stunObject = Instantiate(stunMeshPrefab) as GameObject;
            stunObject.GetComponent<MeshGenerator>().SetVertices(stunCulledPath.ToArray());
            stunObject.transform.position = Vector3.zero;
            stunCulledPath = new List<Vector3>();
            pathRender.positionCount = stunCulledPath.Count;
            pathRender.SetPositions(stunCulledPath.ToArray());
            
            yield return new WaitForSeconds(stunDuration);
            Destroy(stunObject);

        }
        else
        {
            if (stitchingSFX)
            {
                stitchingSFX.Play();
            }
            //print("stun cast started");
            stunCulledPath.Add(transform.position);
            lastStunAbilityTime = Time.time;
            stunCasting = true;
        }
    }

}
