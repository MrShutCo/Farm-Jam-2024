using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Humans
{
    public abstract class Task
    {
        protected Animator animator;
        protected Rigidbody2D rb;
        public virtual void StartTask(Rigidbody2D rb, Animator animator)
        {
            this.rb = rb;
            this.animator = animator;
        }
        public abstract void UpdateTask(Human human, double deltaTime);
        public abstract void FixedUpdateTask(Human human, double fixedDeltaTime);
        public abstract void StopTask();
        public Action OnStopTask { get; set; }
        public string Name { get; set; }
    }

    public class MoveToTask : Task
    {
        Vector3 target;
        int speed;
        List<Node2D> path;
        int currPathNodeDestination;

        public MoveToTask(Vector3 position)
        {
            target = position;
            Name = $"Move to {target}";
        }

        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            base.StartTask(rb, animator);
            animator.SetTrigger("WalkTrigger");
            speed = 3;
            Grid2D.onGridUpdated += onGridUpdated;
            Debug.Log($"Started Move To");
        }

        public override void StopTask()
        {
            animator.SetTrigger("IdleTrigger");
            Grid2D.onGridUpdated -= onGridUpdated;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            if (path == null)
            {
                var pathfinding = human.GetComponent<Pathfinding2D>();
                path = pathfinding.FindPath(human.transform.position, target);
                if (path.Count == 0)
                {
                    OnStopTask?.Invoke();
                }
            }

        }
        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            if (path == null || path.Count == 0) return;
            var nodePos = path[currPathNodeDestination];
            var diffVector = nodePos.worldPosition - human.transform.position;
            
            diffVector.z = 0; // Account for different z levels
            if (diffVector.magnitude < 0.05)
            {
                if (currPathNodeDestination + 1 == path.Count)
                {
                    rb.velocity = Vector2.zero;
                    currPathNodeDestination = 0;
                    path = null;
                    OnStopTask?.Invoke();

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

    public class InstantTask : Task
    {
        Action action;
        private string _taskName;
        public InstantTask(string taskName, Action doAction)
        {
            action = doAction;
            _taskName = taskName;
        }

        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            action();
            Debug.Log($"Started Instant Task: {_taskName}");
            base.StartTask(rb, animator);
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime) { }

        public override void StopTask()
        {
            Debug.Log($"Stopped Instant Task: {_taskName}");
        }
        public override void UpdateTask(Human human, double deltaTime)
        {
            
            OnStopTask?.Invoke();
        }
    }

    public class GetFlayed : Task
    {

        public GetFlayed()
        {
            Name = "MFW flayed :(";
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
        }

        public override void StopTask()
        {
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }
    }

    public class Idle : Task
    {
        public Idle()
        {
            Name = "Idle";
        }

        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            base.StartTask(rb, animator);
            animator.SetTrigger("IdleTrigger");
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime) { }
        public override void StopTask() { }
        public override void UpdateTask(Human human, double deltaTime) { }
    }

    #region Outside Tasks
    public class Wander : Task
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
            Name = $"Wander to {target}";
        }

        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            base.StartTask(rb, animator);
            speed = 1;
        }

        public override void StopTask()
        {
            if (rb is null) return;
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
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

    public class AggressiveMelee : Task
    {
        Transform target;
        float range;

        public AggressiveMelee(Transform target)
        {
            this.target = target;
            Name = $"AggressiveMelee {target}";
        }
        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            base.StartTask(rb, animator);
            range = 5;
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WeaponSelector.ActiveWeapon.TrailConfig.MissDistance)
            {
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
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

    public class DefensiveIdle : Task
    {
        public DefensiveIdle()
        {
            Name = "DefensiveIdle";
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
        }
    }

    public class DefensiveAttack : Task
    {
        Transform target;
        public DefensiveAttack(Transform target)
        {
            this.target = target;
            Name = $"DefensiveAttack {target}";
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WildBehaviour.npcType.DisengageRange)
            {
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
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

    public class CloseRangeAssault : Task
    {
        Transform target;

        float dodgeInterval = 2;
        float timeSinceLastDodge;
        public CloseRangeAssault(Transform target)
        {
            this.target = target;
            Name = $"CloseRangeTactics {target}";
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {

        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            timeSinceLastDodge += (float)fixedDeltaTime;

            if (timeSinceLastDodge < 0.5f) return;
            rb.isKinematic = true;
            var diffVector = target.position - human.transform.position;

            human.WeaponSelector.ActiveWeapon.Flip(diffVector.normalized);

            if (diffVector.magnitude > human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * diffVector.normalized;
            }
            else if (diffVector.magnitude < human.WildBehaviour.npcType.IdealCombatRange)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * -diffVector.normalized;
            }
            if (timeSinceLastDodge > dodgeInterval && diffVector.magnitude < human.WildBehaviour.npcType.IdealCombatRange * .6f)
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

    public class Patrol : Task
    {
        Vector3 target;
        float speed;
        public Patrol(Vector3 position)
        {
            target = position;
            Name = $"Patrol to {target}";
        }

        public override void StartTask(Rigidbody2D rb, Animator animator)
        {
            base.StartTask(rb, animator);
            speed = 1;
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
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

    public class FleeAndFire : Task
    {
        Transform target;
        public FleeAndFire(Transform target)
        {
            this.target = target;
            Name = $"FleeAndFire {target}";
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WeaponSelector.ActiveWeapon.TrailConfig.MissDistance)
            {
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            var diffVector = target.transform.position - human.transform.position;
            rb.velocity = human.WildBehaviour.npcType.MoveSpeed * -diffVector.normalized;
        }
    }
    public class ApproachAndAttack : Task
    {
        Transform target;
        public ApproachAndAttack(Transform target)
        {
            this.target = target;
            Name = $"ApproachAndAttack {target}";
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            human.WeaponSelector.ActiveWeapon.Flip(direction);

            if (diffVector.magnitude <= human.WeaponSelector.ActiveWeapon.TrailConfig.MissDistance)
            {
                human.WeaponSelector.ActiveWeapon.Shoot(direction);
            }
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            var diffVector = target.position - human.transform.position;
            var direction = diffVector.normalized;

            if (diffVector.magnitude > human.WeaponSelector.ActiveWeapon.TrailConfig.MissDistance)
            {
                rb.velocity = human.WildBehaviour.npcType.MoveSpeed * direction;
            }
        }
    }
    #endregion
}