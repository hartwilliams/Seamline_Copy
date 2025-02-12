// using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitchTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip trackA;
    [SerializeField] private AudioClip trackB;
    [SerializeField] private Collider2D trig;
    [SerializeField] private AmbientSystem theAS;

    [SerializeField] private GameObject dialogueBox;
    public CollisionDialogue collisionDialogue;
    public bool runDialogue;

    [SerializeField] private bool boss = false;
    [SerializeField] private CameraControl cam;
    [SerializeField] public Vector3 roomCenterPosition;    
    void Start()
    {
        // theAS = FindObjectOfType<AmbientSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("start fight");
        if(other == trig)
        {
            if(trackA!=null&&trackB!=null)
            {
                theAS.SwitchAudioClip(trackA,trackB);
                if(boss)
                {
                    cam.SwitchToBossRoom(roomCenterPosition);
                    if(runDialogue){
                        dialogueBox.SetActive(true);
                        collisionDialogue.StartRunning(dialogueBox);
                        runDialogue = false;
                    }
                }
            }
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("end fight");
        if(other ==trig)
        {
            if(trackA!=null&&trackB!=null)
            {
                theAS.playOG();
                if(boss)
                {
                    cam.SwitchToPlayerFocus();
                }
            }
            
        }
    }
}
