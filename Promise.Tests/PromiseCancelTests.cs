using System;
using RSG.Exceptions;
using Xunit;

namespace RSG.Tests
{
    public class PromiseCancelTests
    {
        [Fact]
        public void can_cancel_simple_promise()
        {
            var promise = new Promise();

            var completed = 0;
            promise.Then(() =>
            {
                ++completed;
                Assert.Equal(0, completed);
            });
            
            promise.Cancel();
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);
        }
        
        [Fact]
        public void can_cancel_simple_value_promise()
        {
            var promise = new Promise<int>();

            var completed = 0;
            promise.Then(v =>
            {
                ++completed;
                Assert.Equal(0, completed);
            });
            
            promise.Cancel();
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);
        }
        
        [Fact]
        public void cancelled_promise_resolve_causes_error()
        {
            var promise = new Promise<int>();

            var completed = 0;
            promise.Then(v =>
            {
                ++completed;
                Assert.Equal(0, completed);
            });
            
            promise.Cancel();
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);

            Exception exception = null;
            
            try
            {
                promise.Resolve(5);
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            Assert.NotNull(exception);
            Assert.Equal(typeof(PromiseStateException), exception.GetType());
        }
    }
}