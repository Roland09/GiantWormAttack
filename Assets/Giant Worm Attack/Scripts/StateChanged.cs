using UnityEngine;

/// Wrapper class for state machine behaviour.
/// 
/// See also
///   https://docs.unity3d.com/ScriptReference/StateMachineBehaviour.html
///   https://docs.unity3d.com/Manual/StateMachineBehaviours.html
/// </summary>
namespace DeveloperTools.GiantWormAttack
{
    public class StateChanged : StateMachineBehaviour
    {
        /// <summary>
        /// CallBack class which will be used for method invocation on animator state changes
        /// </summary>
        private FollowTarget callBack;

        private FollowTarget GetCallBack(Animator animator)
        {
            if (!callBack)
                callBack = animator.gameObject.GetComponent<FollowTarget>();

            return callBack;
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCallBack(animator).OnStateEnter(animator, stateInfo, layerIndex);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCallBack(animator).OnStateExit(animator, stateInfo, layerIndex);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCallBack(animator).OnStateUpdate(animator, stateInfo, layerIndex);

        }

        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // not implemented
        }

        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // not implemented
        }
    }
}