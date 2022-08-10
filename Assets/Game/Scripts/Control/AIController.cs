using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attribute;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] float aggroCooldownTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float dwellingTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;

        GameObject player;
        Fighter fighter;
        Mover mover;
        Health health;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeAfterReachWaypoint = Mathf.Infinity;
        float timeSiceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");

            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {

                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();

            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();

        }
        
        public void Aggrevate()
        {
            timeSiceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeAfterReachWaypoint += Time.deltaTime;
            timeSiceAggrevated += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void SuspicionBehaviour()
        {
            print("losing target");
            GetComponent<ActionScheduler>().CancelCurrentAction(); // cancel后mover.MoveTo()设定的destination仍未取消，enemy移动到cancel前最后的destionation
            //fighter.Cancel();
            //mover.Cancel();
        }

        private void PatrolBehaviour()
        {

            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeAfterReachWaypoint = 0;
                    //print("At waypoint " + currentWaypointIndex);
                    CycleWaypoint();
                    
                }
 
                nextPosition = GetCurrentWaypoint();
                //print("reaching the next waypoint");
            }
            if(timeAfterReachWaypoint > dwellingTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= wayPointTolerance;

        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);

        }

        private Vector3 GetCurrentWaypoint()
        {

            return patrolPath.GetWayPoint(currentWaypointIndex);
        }


        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                AIController ai =  hit.transform.GetComponent<AIController>();
                if (ai == null) continue;
                ai.Aggrevate();
            }
        }


        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance || timeSiceAggrevated < aggroCooldownTime;
            
        }

        //Called by unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

        }
    }
}

