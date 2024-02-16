using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Humans
{
    public abstract class Job
    {
        protected Rigidbody2D rb;
        public virtual void StartJob(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        public abstract void UpdateJob(Human human, double deltaTime);
        public abstract void FixedUpdateJob(Human human, double fixedDeltaTime);
        public abstract void StopJob();
        public Action OnStopJob { get; set; }
        public string Name { get; set; }
    }

    public class MoveToJob : Job
    {
        Vector3 target;
        int speed;
        List<Node2D> path;
        int currPathNodeDestination;

        public MoveToJob(Vector3 position)
        {
            target = position;
            Name = $"Move to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 3;
            Grid2D.onGridUpdated += onGridUpdated;
        }

        public override void StopJob()
        {
            Grid2D.onGridUpdated -= onGridUpdated;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            if (path == null)
            {
                var pathfinding = human.GetComponent<Pathfinding2D>();
                path = pathfinding.FindPath(human.transform.position, target);
            }

        }
        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            if (path == null) return;
            var nodePos = path[currPathNodeDestination];
            var diffVector = nodePos.worldPosition - human.transform.position;
            diffVector.z = 0; // Account for different z levels
            if (diffVector.magnitude < 0.05)
            {
                if (currPathNodeDestination + 1 == path.Count)
                {
                    rb.velocity = Vector2.zero;
                    OnStopJob?.Invoke();
                }
                else
                {
                    currPathNodeDestination++;
                }
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }

        void onGridUpdated(int x, int y)
        {
            for (int i = currPathNodeDestination; i < path.Count; i++)
            {
                if (path[i].GridX == x && path[i].GridY == y)
                {
                    path = null;
                    break;
                }
            }
        }
    }

    public class WorkJob : Job
    {
        ResourceBuilding building;
        double timeWorkedOnJob;

        public WorkJob(ResourceBuilding building)
        {
            this.building = building;
            Name = $"Work {building.HarvestedResouce} at {building.transform.position}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            building.CurrHumans++;

        }

        public override void StopJob()
        {
            building.CurrHumans--;

        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            timeWorkedOnJob += deltaTime;

            if (timeWorkedOnJob >= building.TimeForOneSkillPoint)
            {
                human.LevelUpSkill(building.HarvestedResouce, 1);
                timeWorkedOnJob = 0;
            }
        }
        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
        }
    }

    #region Outside Jobs
    public class Wander : Job
    {
        Vector3 target = Vector3.zero;
        float wanderDistance = 10;
        float timeOfLastTarget;
        float speed;
        float waitInterval = 1;
        float waitTimer = 0;
        // int tries;
        public Wander(Human human)
        {
            Debug.Log("Wander");
            Name = $"Wander to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 1;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            if (waitTimer <= 0)
            {
                var diffVector = target - human.transform.position;

                if (diffVector.magnitude < 0.05 || target == Vector3.zero)
                {
                    rb.velocity = Vector2.zero;
                    UpdateWanderTarget(human);
                    waitTimer = waitInterval;
                }
                else
                {
                    rb.velocity = speed * diffVector.normalized;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                waitTimer -= (float)fixedDeltaTime;
            }
        }
        void UpdateWanderTarget(Human human)
        {
            if (Time.time - timeOfLastTarget < 5) return;
            timeOfLastTarget = Time.time;
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            target = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-wanderDistance, wanderDistance), 0) + human.transform.position;
            Debug.DrawRay(human.transform.position, target - human.transform.position, Color.red, 1);

        }
    }
    public class FleeTarget : Job
    {
        Vector3 position;
        float fleeDistance = 20;
        float speed;
        public FleeTarget(Transform target)
        {
            Debug.Log("Flee");
            position = target.position;
            Name = $"Flee to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 2;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            var diffVector = human.transform.position - position;

            if (diffVector.magnitude > fleeDistance)
            {
                rb.velocity = Vector2.zero;
                human.AddJob(new Wander(human));
                OnStopJob?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class ApproachTarget : Job
    {
        Vector3 position;
        float speed;
        float range;
        public ApproachTarget(Transform target)
        {
            Debug.Log("Approach");
            position = target.position;
            Name = $"Approach {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 2;
            range = 5;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {

            var diffVector = position - human.transform.position;

            if (diffVector.magnitude > 10)
            {
                rb.velocity = Vector2.zero;
                //Enqueue wander
                OnStopJob?.Invoke();
            }
            else if (diffVector.magnitude < range)
            {
                //Enqueue attack
                OnStopJob?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class AttackTarget : Job
    {
        Transform target;
        float range;


        public AttackTarget(Transform target)
        {
            Debug.Log("Approach");
            this.target = target;
            Name = $"Approach {target}";
        }
        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            range = 5;
        }

        public override void StopJob()
        {

        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= range)
            {
                Debug.Log(human.transform + " is attacking");
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
            else if (diffVector.magnitude > range)
            {
                //enqueue approach target
                OnStopJob?.Invoke();
            }
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
        }
    }

    public class AggressiveMelee : Job
    {
        Transform target;
        float range;
        public AggressiveMelee(Transform target)
        {
            Debug.Log("AggressiveMelee");
            this.target = target;
            Name = $"AggressiveMelee {target}";
        }
        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            range = 5;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WeaponSelector.ActiveWeapon.TrailConfig.MissDistance)
            {
                Debug.Log(human.transform + " is attacking");
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
            else if (diffVector.magnitude > range)
            {
                //enqueue approach target
                OnStopJob?.Invoke();
            }
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            if (diffVector.magnitude > range)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * direction;
            }
            else if (diffVector.magnitude < range)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * -direction;
            }
        }
    }

    public class DefensiveAttack : Job
    {
        Transform target;
        public DefensiveAttack(Transform target)
        {
            Debug.Log("DefensiveAttack");
            this.target = target;
            Name = $"DefensiveAttack {target}";
        }
        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WildBehaviour.npcType.DisengageRange)
            {
                Debug.Log(human.transform + " is attacking");
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
            else
            {
                OnStopJob?.Invoke();
            }
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            if (diffVector.magnitude > human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * direction;
            }
            else if (diffVector.magnitude < human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * -direction;
            }
        }
    }

    public class CloseRangeAssault : Job
    {
        Transform target;

        float dodgeInterval = 2;
        float timeSinceLastDodge;
        public CloseRangeAssault(Transform target)
        {
            Debug.Log("CloseRangeTactics");
            this.target = target;
            Name = $"CloseRangeTactics {target}";
        }
        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {

        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            timeSinceLastDodge += (float)fixedDeltaTime;

            if (timeSinceLastDodge < 0.5f) return;
            rb.isKinematic = true;
            var diffVector = target.position - human.transform.position;

            if (diffVector.magnitude > human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * diffVector.normalized;
            }
            else if (diffVector.magnitude < human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * -diffVector.normalized;
            }
            if (timeSinceLastDodge > dodgeInterval && diffVector.magnitude < human.WildBehaviour.npcType.IdealCombatRange * .75f)
            {
                Dodge(target.position);
            }
            if (timeSinceLastDodge > 0.5f)
            {
                human.WeaponSelector.ActiveWeapon.Shoot((target.position - human.transform.position).normalized);
            }
        }

        void Dodge(Vector2 targetPos)
        {
            //dodge perpendicular to the target
            var diffVector = targetPos - rb.position;
            var dodgeDirection = new Vector2(diffVector.y, -diffVector.x).normalized;
            rb.isKinematic = false;
            rb.AddForce(dodgeDirection * 8, ForceMode2D.Impulse);
            timeSinceLastDodge = 0;
        }
    }

    public class Patrol : Job
    {
        Vector3 target;
        float speed;
        public Patrol(Vector3 position)
        {
            Debug.Log("Patrol");
            target = position;
            Name = $"Patrol to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 1;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            var diffVector = target - human.transform.position;

            if (diffVector.magnitude < 0.05)
            {
                rb.velocity = Vector2.zero;
                UpdatePatrolTarget(human);
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
        void UpdatePatrolTarget(Human human)
        {
            target = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0) + target;
        }
    }
    #endregion
}