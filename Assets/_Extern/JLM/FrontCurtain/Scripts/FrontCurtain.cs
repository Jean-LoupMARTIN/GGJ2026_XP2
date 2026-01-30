//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class FrontCurtain : Singleton<FrontCurtain>
{
    [SerializeField, Min(0)] float speed = 1;
    [SerializeField] State stateOnAwake = State.Opened;
    [SerializeField] string openedAnimName = "FrontCurtainOpened";
    [SerializeField] string closedAnimName = "FrontCurtainClosed";
    [SerializeField] string openingAnimName = "FrontCurtainOpening";
    [SerializeField] string closingAnimName = "FrontCurtainClosing";

    State state;
    Animator animator;
    UnityAction onAnimEnd;

    public State CrtState => state;

    public enum State
    {
        Opened,
        Closed,
        Opening,
        Closing,
    }

    protected override void Awake()
    {
        base.Awake();
        Cache();
        animator.SetFloat("speed", speed);
        SetState(stateOnAwake);
    }

    void Cache()
    {
        animator = GetComponent<Animator>();
    }

    //bool CheckAnimNames()
    //    => HasAnim(openedAnimName) &&
    //       HasAnim(closedAnimName) &&
    //       HasAnim(openingAnimName) &&
    //       HasAnim(closingAnimName);

    //bool HasAnim(string animName)
    //{
    //    if (!animator.runtimeAnimatorController)
    //        return false;

    //    AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;

    //    return controller.layers.Length > 0 &&
    //        controller.layers[0].stateMachine.states.Contains((s) => s.state.name == animName);
    //}

    string StateToAnim(State state)
    {
        return state switch
        {
            State.Opened  => openedAnimName,
            State.Closed  => closedAnimName,
            State.Opening => openingAnimName,
            _             => closingAnimName
        };
    }

    void SetState(State state)
    {
        animator.Play(StateToAnim(state), 0, 0f);
        this.state = state;

        if (onAnimEnd != null)
        {
            Debug.LogWarning("FrontCurtain.SetState : onAnimEnd canceled");
            onAnimEnd = null;
        }
    }

    public void Open() => SetState(State.Opening);
    public void Close() => SetState(State.Closing);
    public void SetOpened() => SetState(State.Opened);
    public void SetClosed() => SetState(State.Closed);

    public void Open(UnityAction onOpened)
    {
        Open();
        onAnimEnd = onOpened;
    }

    public void Close(UnityAction onClosed)
    {
        Close();
        onAnimEnd = onClosed;
    }

    public void OnOpeningAnimEnd()
    {
        state = State.Opened;

        if (onAnimEnd != null)
        {
            UnityAction action = onAnimEnd;
            onAnimEnd = null;
            action.Invoke();
        }
    }

    public void OnClosingAnimEnd()
    {
        state = State.Closed;

        if (onAnimEnd != null)
        {
            UnityAction action = onAnimEnd;
            onAnimEnd = null;
            action.Invoke();
        }
    }
}