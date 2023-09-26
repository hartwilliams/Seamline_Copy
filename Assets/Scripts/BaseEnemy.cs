using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{

	protected int health;
	protected int movementSpeed;
	protected bool alive;
	protected Rigidbody2D rb2d;
	protected float basicAttackCDLeft;
	[SerializeField] protected GameObject playerObject;
    [SerializeField] protected GameObject weaponPrefab;
    [SerializeField] protected float basicAttackRange;
    [SerializeField] protected float basicAttackCD;
    [Range(0f, 180f)]
    [SerializeField] protected float basicAttackRangeAngle;
    [SerializeField] protected float basicAttackSwingTime; //duration of swing animation(temporary until real animation exists)
    [SerializeField] protected AudioSource basicAttackSFX;
    [SerializeField] private int maxHealth;



    void Start()
    {
        health = maxHealth;
        alive = true;
        rb2d = GetComponent<Rigidbody2D>();
    }

    public bool isAlive() { return alive; }


    public virtual void ReactToHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    protected IEnumerator Die()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    }

    protected abstract void move();
    protected abstract void attack();
}

