using UnityEngine;
using System.Collections;

public class NewBomber : MonoBehaviour, IKillable
{
    private enum BomberState
    {
        Patrol,
        Threaten,
        Attack,
        Die
    }

    private Patrol patrol;

    private BomberState curState;

    private Transform player;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private float DistanceToThreaten = 8f;

    [SerializeField]
    private float AttackSpeed = 15f;

    [SerializeField]
    private float TimeToThreaten = 10;

    [SerializeField]
    private float TimeToAttack = 3;

    [SerializeField]
    private GameObject Ooze;

    [SerializeField]
    private PlayerAttack oozeAttack;

    [SerializeField]
    private EnemyAttackArea attackArea;

    private ParticleSystem[] OozeParticles;
    private bool isThreatening = false;
    private bool isAttacking = false;

	// Use this for initialization
	void Start () 
    {
        patrol = GetComponent<Patrol>();
        curState = BomberState.Patrol;

        OozeParticles = Ooze.GetComponentsInChildren<ParticleSystem>();

        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        switch(curState)
        {
            case BomberState.Patrol:
                if(Vector3.Distance(transform.position, player.position) <= DistanceToThreaten)
                {
                    curState = BomberState.Threaten;
                }
                else
                    patrol.Patrolling();
                break;
            case BomberState.Threaten:
                if(!isThreatening)
                {
                    StopCoroutine(Threat());
                    StartCoroutine(Threat());
                }
                break;
            case BomberState.Attack:
                if(!isAttacking)
                {
                    StopCoroutine(Attack());
                    StartCoroutine(Attack());
                }
                break;
        }
	}


    IEnumerator Threat()
    {
        isThreatening = true;
        float timeTaken = 0;

        while (timeTaken < TimeToThreaten && curState != BomberState.Die)
        {
            timeTaken += Time.deltaTime;

            Vector3 toLookAt = player.position - transform.position;
            toLookAt.y = 0;

            Quaternion lr = Quaternion.LookRotation(toLookAt);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lr, patrol.RotateSpeed * Time.deltaTime);

            transform.position += Vector3.up * Mathf.Lerp(0, (player.position.y + 3.5f) - transform.position.y, Time.deltaTime);
            yield return null;
        }

        isThreatening = false;
        curState = curState == BomberState.Die ? BomberState.Die : BomberState.Attack;
        yield return null;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        float timeTaken = 0;

        foreach (ParticleSystem ps in OozeParticles)
            ps.Play();

        attackArea.ActivateAttack(oozeAttack);

        while (timeTaken < TimeToAttack && curState != BomberState.Die)
        {
            timeTaken += Time.deltaTime;

            transform.position += transform.forward * AttackSpeed * Time.deltaTime;

            yield return null;
        }

        attackArea.EndAttack();

        isAttacking = false;
        curState = curState == BomberState.Die ? BomberState.Die : BomberState.Patrol;
        yield return null;
    }

    public void Kill()
    {
        StopCoroutine(Attack());
        StopCoroutine(Threat());

        attackArea.EndAttack();

        anim.SetBool("Death", true);
        curState = BomberState.Die;

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        Destroy(gameObject, 2);
    }
}
