using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class NPC : MonoBehaviour {
    public List<GameObject> HostileList;
    public State CurrentState;
    private float alertness = 20f;
    private NavMeshAgent nav;
    private Animation anim;
    private Animator animator;
    private float wait = 0f;
    public Transform eyes;
    public float HP = 50.0f;
    public bool IsDead = false;
    public float TimeSpentIdle = 0f;
    

    public enum State
    {
        Idle, Walk, Search, Chase, Attack, Talk, Trade, Dead
    }

	// Use this for initialization
	void Start () {
        nav = GetComponent<NavMeshAgent>();
        CurrentState = State.Idle;
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (anim.isPlaying == false)
       // {
            switch (CurrentState)
            {
                case State.Idle:
                    if (anim != null)
                        anim.Play("idle");
                    else
                        animator.Play("idle");
                    if (TimeSpentIdle >= 50f) CurrentState = State.Walk;
                    TimeSpentIdle += 1f;
                    break;
                case State.Walk:
                    if (anim != null)
                        anim.Play("walk");
                    else
                        animator.Play("walk");
                    if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending)
                    {
                        if (TimeSpentIdle == 0f)
                        {
                            CurrentState = State.Idle;
                            break;
                        }
                        Vector3 randomPos = Random.insideUnitSphere * alertness;
                        NavMeshHit navHit;
                        NavMesh.SamplePosition(transform.position + randomPos, out navHit, 100f, NavMesh.AllAreas);
                        nav.SetDestination(navHit.position);
                        TimeSpentIdle = 0f;
                    }
                    CheckSight();
                    break;
                case State.Chase:
                    nav.destination = HostileList[0].transform.position;
                    if (anim != null)
                        anim.Play("run");
                    else
                        animator.Play("run");
                    float distance = Vector3.Distance(transform.position, HostileList[0].transform.position);
                    if (distance > 10f)
                    {
                        CurrentState = State.Search;
                        wait = 5f;
                        alertness = 5f;
                        CheckSight();
                    }
                    if (distance < 5f)
                    {
                        CurrentState = State.Attack;
                    }
                    break;
                case State.Search:
                    if (anim != null)
                        anim.Play("walk");
                    else
                        animator.Play("walk");
                    if (wait > 0f)
                    {
                        wait -= Time.deltaTime;
                        transform.Rotate(0f, 120f * Time.deltaTime, 0f);
                    }
                    else
                    {
                        CurrentState = State.Idle;
                    }
                    CheckSight();
                    break;
                case State.Talk:
                    if (anim != null)
                        anim.Play("talk");
                    else
                        animator.Play("talk");
                    break;
                case State.Attack:
                    float distance1 = Vector3.Distance(transform.position, HostileList[0].transform.position);
                    if (distance1 >= 5f)
                    {
                        CurrentState = State.Chase;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(HostileList[0].transform.position - transform.position), Time.deltaTime * 4);
                        if (anim != null)
                            anim.Play("attack 1");
                        else
                            animator.Play("attack 1");
                    }

                    break;
            case State.Dead:
                //nav.SetDestination(transform.position);
                //transform.rotation = transform.rotation;
                nav.isStopped = true;
                if (IsDead == false)
                {
                    anim.Play("death");
                    //add parameters for this later
                    gameObject.tag = "Lootable";
                    IsDead = true;
                }
                break;
          //  }
        }
    }



    void CheckSight()
    {
        RaycastHit rayHit;
        if (Physics.Linecast(eyes.position, HostileList[0].transform.position, out rayHit))
        {
            if (rayHit.collider.gameObject.name == HostileList[0].name)
            {
                CurrentState = State.Chase;
                nav.speed = 2.5f;
                //anim.Play("run");
            }

        }
        else
            {
                if (CurrentState == State.Chase) CurrentState = State.Search;
            }
    }

    private void OnTriggerEnter(Collider col)
    {
        try
        {
            if (col.gameObject.tag == "Spell")
            {
                Spell SpellScript = col.gameObject.GetComponent<Spell>();
                HP -= SpellScript.ThisSpell.Damage;
                if (HP <= 0)
                {
                    CurrentState = State.Dead;
                }
                else
                {

                    if (anim != null)
                        anim.Play("hit 1");
                    else
                        animator.Play("pain");
                }
            }
        }
        catch
        {

        }
    }
}
