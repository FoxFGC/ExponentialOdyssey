using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Jump, Attack, Damage }

public class EnemyAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public int damType;

    public float fleeRandomAngle = 100f;
    public float fleeDistance = 10f;
    public float lookRadius = 10f;
    Transform target;

    private int m_CurrentWaypointIndex = -1;
    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;

    public enum WalkType { Patroll, ToOrigin }
    private WalkType walkType;

    private EnemyHealth health;
    public Transform player;


    public float attackRange = 2f;
    public float patrolWaitTime = 1f;
    private bool _patrolScheduled = false;


    void Start()
    {
        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];


        if (waypoints == null || waypoints.Length == 0)
        {
            var wpObjs = GameObject.FindGameObjectsWithTag("Waypoint");
            System.Array.Sort(wpObjs, (a, b) => a.name.CompareTo(b.name));
            waypoints = wpObjs.Select(o => o.transform).ToArray();
        }


        if (waypoints.Length > 0)
        {
            currentState = SlimeAnimationState.Idle;
            Invoke(nameof(WalkToNextDestination), 0.5f);
        }

        health = GetComponent<EnemyHealth>();
        if (health != null)
            health.OnDamaged += dmg => currentState = SlimeAnimationState.Damage;


        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDamaged -= dmg => currentState = SlimeAnimationState.Damage;
    }

    public void CancelGoNextDestination() =>
       CancelInvoke(nameof(WalkToNextDestination));

    public void WalkToNextDestination()
    {
        if (waypoints.Length == 0) return;
        currentState = SlimeAnimationState.Walk;
        _patrolScheduled = false;

        agent.isStopped = false;
        agent.updateRotation = true;

        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        SetFace(faces.WalkFace);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Update()
    {

        if (currentState != SlimeAnimationState.Damage && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= lookRadius)
            {

                currentState = SlimeAnimationState.Walk;
                agent.isStopped = false;
                agent.updateRotation = true;


                Vector3 fleeDir = (transform.position - player.position).normalized;


                float angleOffset = Random.Range(-fleeRandomAngle, fleeRandomAngle);
                Quaternion jitterRot = Quaternion.AngleAxis(angleOffset, Vector3.up);
                Vector3 randomFleeDir = jitterRot * fleeDir;


                Vector3 rawTarget = transform.position + randomFleeDir * fleeDistance;


                NavMeshHit hit;
                if (NavMesh.SamplePosition(rawTarget, out hit, 1f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
                else
                    agent.SetDestination(rawTarget);


                SetFace(faces.WalkFace);
                animator.SetFloat("Speed", agent.velocity.magnitude);
                return;
            }
        }



        switch (currentState)
        {
            case SlimeAnimationState.Idle:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    StopAgent();
                    SetFace(faces.Idleface);
                }

                if (waypoints.Length > 0 && !_patrolScheduled)
                {
                    Invoke(nameof(WalkToNextDestination), patrolWaitTime);
                    _patrolScheduled = true;

                }

                break;

            case SlimeAnimationState.Walk:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = SlimeAnimationState.Idle;
                    _patrolScheduled = false;
                }
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;

            case SlimeAnimationState.Attack:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    StopAgent();
                    SetFace(faces.attackFace);
                    animator.SetTrigger("Attack");
                }
                break;

            case SlimeAnimationState.Jump:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                {
                    StopAgent();
                    SetFace(faces.jumpFace);
                    animator.SetTrigger("Jump");
                }
                break;

            case SlimeAnimationState.Damage:
                var info = animator.GetCurrentAnimatorStateInfo(0);
                if (!info.IsName("Damage0") && !info.IsName("Damage1") && !info.IsName("Damage2"))
                {
                    StopAgent();
                    SetFace(faces.damageFace);
                    animator.SetInteger("DamageType", damType);
                    animator.SetTrigger("Damage");
                }
                return;
        }
    }

    private void StopAgent()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
        animator.SetFloat("Speed", 0f);
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    public void AlertObservers(string message)
    {
        if (message == "AnimationDamageEnded")
        {
            currentState = SlimeAnimationState.Idle;
        }
        else if (message == "AnimationAttackEnded" || message == "AnimationJumpEnded")
        {
            currentState = SlimeAnimationState.Idle;
        }
    }

    void OnAnimatorMove()
    {
        var pos = animator.rootPosition;
        pos.y = agent.nextPosition.y;
        transform.position = pos;
        agent.nextPosition = transform.position;
    }

}
