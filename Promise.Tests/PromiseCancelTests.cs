using System;
using RSG.Exceptions;
using Xunit;

// ReSharper disable once CheckNamespace
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

            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        [Fact]
        public void can_cancel_simple_promise_value_chain()
        {
            var promise = new Promise<int>();

            var completed = 0;
            promise.Then(v =>
            {
                ++completed;
                Assert.Equal(0, completed);
            }).Then(v =>
            {
                Assert.Equal(0, completed);
            });
            
            promise.Cancel();
            
            Assert.Equal(false, promise.CanBeResolved);
            Assert.Equal(false, promise.CanBeCanceled);

            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
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
            
            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
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
            
            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
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
            
            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
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
            
            foreach (var state in promise.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }

        [Fact]
        public void can_cancel_promise_all()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;

            promise1.Then(() =>
                {
                    then1Called = true;
                })
                .Then(() =>
                {
                    then2Called = true;
                }).OnCancel(() =>
            {
                cancel1Called = true;
            });

            promise2.Then(() =>
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

            var promiseAll = Promise.All(promise1, promise2);
            promiseAll.Cancel();
            
            var resolved = promise1.TryResolve();
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise1.CanBeCanceled);
            
            Assert.Equal(false, promise2.CanBeResolved);
            Assert.Equal(false, promise2.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            foreach (var state in promiseAll.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        [Fact]
        public void can_cancel_promise_all_valued()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;

            promise1.Then(x =>
                {
                    then1Called = true;
                })
                .Then(x =>
                {
                    then2Called = true;
                }).OnCancel(() =>
                {
                    cancel1Called = true;
                });

            promise2.Then(x =>
                {
                    then3Called = true;
                })
                .Then(x =>
                {
                    then4Called = true;
                })
                .OnCancel(() =>
                {
                    cancel2Called = true;
                });

            var promiseAll = Promise<int>.All(promise1, promise2);
            promiseAll.Cancel();
            
            var resolved = promise1.TryResolve(4);
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise1.CanBeCanceled);
            
            Assert.Equal(false, promise2.CanBeResolved);
            Assert.Equal(false, promise2.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            foreach (var state in promiseAll.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        [Fact]
        public void can_cancel_promise_all_by_cancelling_child()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;

            promise1.Then(() =>
                {
                    then1Called = true;
                })
                .Then(() =>
                {
                    then2Called = true;
                }).OnCancel(() =>
                {
                    cancel1Called = true;
                });

            promise2.Then(() =>
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

            var promiseAll = Promise.All(promise1, promise2);
            promise2.Cancel();
            
            var resolved = promise1.TryResolve();
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise1.CanBeCanceled);
            
            Assert.Equal(false, promiseAll.CanBeCanceled);
            
            Assert.Equal(false, promise2.CanBeResolved);
            Assert.Equal(false, promise2.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            foreach (var state in promiseAll.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        
        [Fact]
        public void can_cancel_promise_all_valued_by_cancelling_child()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            var then1Called = false;
            var then2Called = false;
            var then3Called = false;
            var then4Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;

            promise1.Then(x =>
                {
                    then1Called = true;
                })
                .Then(x =>
                {
                    then2Called = true;
                }).OnCancel(() =>
                {
                    cancel1Called = true;
                });

            promise2.Then(x =>
                {
                    then3Called = true;
                })
                .Then(x =>
                {
                    then4Called = true;
                })
                .OnCancel(() =>
                {
                    cancel2Called = true;
                });

            var promiseAll = Promise<int>.All(promise1, promise2);
            promise2.Cancel();
            
            var resolved = promise1.TryResolve(3);
            
            Assert.Equal(false, resolved);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise1.CanBeCanceled);
            
            Assert.Equal(false, promiseAll.CanBeCanceled);
            
            Assert.Equal(false, promise2.CanBeResolved);
            Assert.Equal(false, promise2.CanBeCanceled);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            Assert.Equal(false, then3Called);
            Assert.Equal(false, then4Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            foreach (var state in promiseAll.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }

        [Fact]
        public void can_cancel_promise_race()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();

            var then1Called = false;
            var then2Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;
            
            promise1.Then(() =>
            {
                then1Called = true;
            }).OnCancel(() =>
            {
                cancel1Called = true;
            });

            promise2.Then(() =>
            {
                then2Called = true;
            }).OnCancel(() =>
            {
                cancel2Called = true;
            });

            var race = Promise.Race(promise1, promise2);
            race.Cancel();

            promise1.TryResolve();
            promise2.TryResolve();
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise2.CanBeResolved);
            
            foreach (var state in race.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        [Fact]
        public void can_cancel_promise_race_valued()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();

            var then1Called = false;
            var then2Called = false;
            
            var cancel1Called = false;
            var cancel2Called = false;
            
            promise1.Then(x =>
            {
                then1Called = true;
            }).OnCancel(() =>
            {
                cancel1Called = true;
            });

            promise2.Then(x =>
            {
                then2Called = true;
            }).OnCancel(() =>
            {
                cancel2Called = true;
            });

            var race = Promise<int>.Race(promise1, promise2);
            race.Cancel();

            promise1.TryResolve(5);
            promise2.TryResolve(6);
            
            Assert.Equal(false, then1Called);
            Assert.Equal(false, then2Called);
            
            Assert.Equal(true, cancel1Called);
            Assert.Equal(true, cancel2Called);
            
            Assert.Equal(false, promise1.CanBeResolved);
            Assert.Equal(false, promise2.CanBeResolved);
            
            foreach (var state in race.GetAllTreeStates())
            {
                Assert.Equal(PromiseState.Cancelled, state);
            }
        }
        
        [Fact]
        public void cant_cancel_promise_first()
        {
            var promise1 = new Promise<int>();
            var promise2 = new Promise<int>();
            
            promise1.Then(x => { });
            promise2.Then(x => { });

            var first = Promise<int>.First(() => promise1, () => promise2);

            Exception exception = null;

            try
            {
                first.Cancel();
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            Assert.NotNull(exception);
            Assert.Equal(typeof(PromiseStateException), exception.GetType());
        }
        
        [Fact]
        public void cant_cancel_promise_sequence()
        {
            var promise1 = new Promise();
            var promise2 = new Promise();
            
            promise1.Then(() => { });
            promise2.Then(() => { });

            var first = Promise.Sequence(() => promise1, () => promise2);

            Exception exception = null;

            try
            {
                first.Cancel();
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