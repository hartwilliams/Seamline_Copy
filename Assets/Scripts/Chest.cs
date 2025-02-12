using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour {

    [SerializeField] private SpriteRenderer chest;
    [SerializeField] private Sprite openChest;
    [SerializeField] private Sprite closedChest;
    [SerializeField] protected GameObject lootSpawnerPrefab;
    [SerializeField] protected int healthPotionDroprate;
    [SerializeField] protected int yarnDroprate;
    [SerializeField] private AudioSource audSource;
    protected bool lootDropped = false;
    // [SerializeField] private Animation anim;
    [SerializeField] private Camera mainCamera;

    private GameObject player;
    private DefaultInputAction playerInputAction;
    [SerializeField] private bool playerCanOpen;   // false by default


    // Start is called before the first frame update
    void Awake() {
        audSource = GetComponent<AudioSource>();
        audSource.Pause();
        audSource.mute = false;
        // anim = gameObject.GetComponent<Animation>();

        mainCamera = Camera.main;

        player = GameObject.Find("Player");
        playerInputAction = new DefaultInputAction();
        playerInputAction.Player.Pickup.performed += playerOpenChest;
    }


    private void OnEnable() {
        playerInputAction.Player.Pickup.Enable();
    }

    private void OnDisable() {
        playerInputAction.Player.Pickup.Disable();
    }


    // When player presses P, if player is within range of chest, open chest
    public void playerOpenChest(InputAction.CallbackContext ctx) {
        // If this chest's playerCanOpen boolean is true and player is within 3.0f of chest, open it when press P
        if ((this.playerCanOpen == true) & (Vector3.Distance(transform.position, player.transform.position) <= 3.0f)) {
            OpenChest();
        }
    }



    // Set the sprite to an open chest or a closed chest
    public void setChestOpen(bool boolean) {
        // Start coroutine to open chest
        if (boolean == true) {
            StartCoroutine(CameraOpenChest());
        }
        // Else, close chest
        else {
            chest.sprite = closedChest;
        }

        // audSource.mute = true;
    }




    // Stuff to do when open chest
    private void OpenChest() {
        chest.sprite = openChest;
        // anim.Play("Chest_open");

        audSource.Play();   // Play chest open sound effect

        if (lootDropped == false) {
            Instantiate(lootSpawnerPrefab, transform.position, Quaternion.identity)
                .GetComponent<LootSpawner>().SpawnLoot(healthPotionDroprate, yarnDroprate);
            lootDropped = true;
        }
    }



    // Coroutine for camera when open chest
    private IEnumerator CameraOpenChest() {
        // Camera focus on chest
        mainCamera.GetComponent<CameraControl>().SwitchToBossRoom(this.transform.position);
        Debug.Log("Should focus on chest");

        yield return new WaitForSeconds(0.75f);

        // Change chest sprite/animation and drop loot
        OpenChest();

        yield return new WaitForSeconds(1.5f);
        mainCamera.GetComponent<CameraControl>().SwitchToPlayerFocus();
    }


}
