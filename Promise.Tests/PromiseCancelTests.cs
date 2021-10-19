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
        
        [Fact]
        public void can_cancel_chains_with_cancel_of_main_promise()
        {
            var promise = new Promise();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;
            
            promise.Then(() =>
            {
                then1Called = true;
            })
            .Then(() =>
            {
                then2Called = true;
            })
            .OnCancel(() =>
            {
                cancel1Called = true;
            });
            
            promise.Then(() =>
            {
                then3Called = true;
            })
            .Then(() =>
            {
                then4Called = true;
            })
            .OnCancel(() =>
            {
                cancel2Called = true;
            });
            
            promise.Cancel();

            var resolved = promise.TryResolve();
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
        }
        
        [Fact]
        public void can_cancel_chains_with_cancel_of_handler()
        {
            var promise = new Promise();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;

            var handler = promise.Then(() =>
                {
                    then1Called = true;
                })
                .Then(() =>
                {
                    then2Called = true;
                });
            
            handler.OnCancel(() =>
            {
                cancel1Called = true;
            });

            promise.Then(() =>
                {
                    then3Called = true;
                })
                .Then(() =>
                {
                    then4Called = true;
                })
                .OnCancel(() =>
                {
                    cancel2Called = true;
                });
            
            handler.Cancel();

            var resolved = promise.TryResolve();
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
        }
    }
}