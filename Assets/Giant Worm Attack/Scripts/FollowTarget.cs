using UnityEngine;

namespace DeveloperTools.GiantWormAttack
{
    /// <summary>
    /// Move gameobject towards a target at constant speed. Target could be moving itself
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        #region Definitions

        /// <summary>
        /// Definition of custom trigger names
        /// </summary>
        public struct AnimatorTrigger
        {
            public static readonly AnimatorTrigger Attack = new AnimatorTrigger("Attack");

            public string Name { get; private set; }

            private AnimatorTrigger(string name) { Name = name; }
        }

        /// <summary>
        /// Definition of animations used in code
        /// </summary>
        public struct GiantWormAnimation
        {
            public static readonly GiantWormAnimation JumpBite = new GiantWormAnimation("UndergroundJumpBiteToUnderground");

            public string Name { get; private set; }

            private GiantWormAnimation(string name) { Name = name; }

            public bool Matches(AnimatorStateInfo stateInfo) { return stateInfo.IsName(Name); }

        }

        /// <summary>
        /// The currently active animation sequence
        /// </summary>
        public enum Sequence
        {
            Follow,
            Attack
        }

        #endregion Definitions

        #region Inspector

        /// <summary>
        /// The target to follow and attack
        /// </summary>
        public GameObject target;

        /// <summary>
        /// The speed movement towards the target
        /// </summary>
        public float speed = 5f;

        /// <summary>
        /// The distance to stop movement towards the target
        /// </summary>
        public float stopDistance = 0.3f;

        [Tooltip("Gameobject which will be activated on Attack Enter")]
        public GameObject[] onAttackEnterActivate;

        [Tooltip("Gameobject which will be activated on Attack Exit")]
        public GameObject[] onAttackExitActivate;

        [Tooltip("Normalized time [0,1] at which the Attack Enter gameobjects should be activated. This depends on the animation frame")]
        [Range(0, 1)]
        public float onAttackEnterTime = 0.07f;

        [Tooltip("Normalized time [0,1] at which the Attack Exit gameobjects should be activated. This depends on the animation frame")]
        [Range(0, 1)]
        public float onAttackExitTime = 0.27f;

        #endregion Inspector

        #region Private members

        /// <summary>
        /// The currently active animation sequence
        /// </summary>
        private Sequence sequence = Sequence.Follow;

        /// <summary>
        /// The animator to be used
        /// </summary>
        private Animator animator;

        #endregion Private members

        void Start()
        {
            animator = GetComponent<Animator>();

            animator.StopPlayback();
        }

        #region State Behaviour

        void AttackTarget()
        {
            // change internal sequence state
            sequence = Sequence.Attack;

            // start the attack animation
            animator.SetTrigger(AnimatorTrigger.Attack.Name);

        }

        public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GiantWormAnimation.JumpBite.Matches(stateInfo))
            {
                Debug.Log("Attack Enter");
            }
        }

        public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GiantWormAnimation.JumpBite.Matches(stateInfo))
            {
                sequence = FollowTarget.Sequence.Follow;
            }
        }

        public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GiantWormAnimation.JumpBite.Matches(stateInfo))
            {
                if (stateInfo.normalizedTime >= onAttackEnterTime)
                {
                    OnAttackEnter();
                }

                if (stateInfo.normalizedTime >= onAttackExitTime)
                {
                    OnAttackExit();
                }
            }
        }

        private void OnAttackEnter()
        {
            foreach( GameObject go in onAttackEnterActivate) {
                if( !go.activeSelf)
                    go.SetActive(true);
            }
                
        }

        private void OnAttackExit()
        {
            foreach (GameObject go in onAttackExitActivate)
            {
                if (!go.activeSelf)
                    go.SetActive(true);
            }
        }

        private void DisableAllAttackGameObjects()
        {
            foreach (GameObject go in onAttackEnterActivate)
            {
                if( go.activeSelf)
                    go.SetActive(false);
            }
            foreach (GameObject go in onAttackExitActivate)
            {
                if (go.activeSelf)
                    go.SetActive(false);
            }
        }

        #endregion State Behaviour

        void LateUpdate()
        {
            if (sequence == Sequence.Attack)
            {
                // do nothing
            }
            else if (sequence == Sequence.Follow)
            {
                // ensure everything from the attack sequence is inactive
                // note: you might want to create a proper state machine instead of calling this all the time
                DisableAllAttackGameObjects();

                // follow or attack depending on the distance
                Vector3 distance = target.transform.position - this.transform.position;

                // perform action depending on the distance
                if (distance.magnitude < stopDistance)
                {
                    AttackTarget();
                }
                else
                {
                    MoveTowardsTarget();
                }
            }
        }

        private void MoveTowardsTarget()
        {
            Vector3 distance = target.transform.position - this.transform.position;
            Vector3 direction = distance.normalized;

            transform.position = this.transform.position + direction * speed * Time.deltaTime;

            // rotate towards the target
            // https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
            // -------------------------------------

            // Determine which direction to rotate towards
            Vector3 targetDirection = target.transform.position - transform.position;

            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}