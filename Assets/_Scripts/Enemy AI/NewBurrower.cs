using UnityEngine;
using System.Collections;

public class NewBurrower : MonoBehaviour 
{
    private enum BurrowerState
    {
        Patrol,
        Chase
    }

    [SerializeField]
    Animator anim;

    [SerializeField]
    CapsuleCollider capCollider;

    [SerializeField]
    GroundCheck groundCheck;

    [SerializeField]
    GameObject DiggingParticles;

    [SerializeField]
    GameObject UnderGroundParticles;

    [SerializeField]
    EnemyAttackArea attackArea;

    [SerializeField]
    PlayerAttack attack;

    [SerializeField]
    float DistanceToChase;

    [SerializeField]
    GameObject model;

    [SerializeField]
    float timeToDig;

    [SerializeField]
    float maxTimeToChase;

    ParticleSystem[] diggingParticles;
    ParticleSystem[] underGroundParticles;

    Transform player;
    BurrowerState curState;
    Patrol patrol;

    NavMeshAgent agent;

    bool isChasing;
    bool isDigging;

    bool isUnderGround;

    public void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        curState = BurrowerState.Patrol;
        patrol = GetComponent<Patrol>();

        diggingParticles = DiggingParticles.GetComponentsInChildren<ParticleSystem>();
        underGroundParticles = UnderGroundParticles.GetComponentsInChildren<ParticleSystem>();

        agent = this.gameObject.GetComponent<NavMeshAgent>();

        model.SetActive(false);
        isUnderGround = true;
        capCollider.enabled = false;
        foreach (ParticleSystem ps in underGroundParticles)
            ps.Play();
    }

    public void Update()
    {
        switch(curState)
        {
            case BurrowerState.Patrol:
                if (!isUnderGround && !isDigging)
                {
                    if (Random.Range(0, 1000) <= 1)
                    {
                        StopCoroutine(Dig());
                        StartCoroutine(Dig());
                    }
                }

                if (!isDigging)
                {
                    if (Vector3.Distance(transform.position, player.position) <= DistanceToChase)
                    {
                        curState = BurrowerState.Chase;
                    }
                    else
                    {
                        
                        patrol.Patrolling();
                    }
                }
                break;
            case BurrowerState.Chase:
                if(!isChasing)
                {
                    StopCoroutine(Chasing());
                    StartCoroutine(Chasing());
                }
                break;

        }
    }

    IEnumerator Chasing()
    {
        isChasing = true;
        if(!isUnderGround)
        {
            StopCoroutine(Dig());
            StartCoroutine(Dig());
        }

        while (isDigging)
            yield return null;

        float curTime = 0;

        while(!groundCheck.PlayerAbove && curTime < maxTimeToChase)
        {
            curTime += Time.deltaTime;
            agent.SetDestination(player.position);
            yield return null;
        }

        
        agent.Stop();

        yield return new WaitForSeconds(.5f);

        foreach (ParticleSystem ps in diggingParticles)
            ps.Play();

        model.SetActive(true);
        attackArea.ActivateAttack(attack);
        anim.SetBool("Attack", true);
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(.33f);
        EndAttack();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        anim.SetBool("Attack", false);
        capCollider.enabled = true;

        foreach (ParticleSystem ps in underGroundParticles)
            ps.Stop();

        
        yield return new WaitForSeconds(.5f);

        agent.Resume();
        isChasing = false;
        isUnderGround = false;
        
        curState = BurrowerState.Patrol;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return null;
    }

    IEnumerator Dig()
    {
        isDigging = true;
        isUnderGround = true;
        agent.Stop();
        float curTime = 0;

        foreach (ParticleSystem ps in underGroundParticles)
            ps.Play();
        foreach (ParticleSystem ps in diggingParticles)
            ps.Play();

        model.SetActive(false);
        capCollider.enabled = false;

        yield return new WaitForSeconds(timeToDig);
        
        isDigging = false;
        agent.Resume();
        yield return null;
    }

    void EndAttack()
    {
        attackArea.EndAttack();
    }
}
