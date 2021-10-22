using RSG;

namespace DisposeUtilities
{
    public class ExampleMonoBehaviour : DisposableMonoBehaviour
    {
        private void Start()
        {
            // here we indicate that this Promise must be cancelled with the end of lifecycle of this object.
            DoLongAction().CancelWith(this);
        }

        private IPromise DoLongAction()
        {
            // something long enough which may finish by the time the Example gameObject will be destroyed.
        }
    }

    public class ExampleState : DisposableFSMState // (custom class which inherits from IDisposeProvider)
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            // here we indicate that this Promise must be cancelled with the end of lifecycle of this state.
            DoLongAction().CancelWith(this);
        }

        private IPromise DoLongAction()
        {
            // something long enough which may finish by the time the ExampleState will be finished.
        }
    }
}