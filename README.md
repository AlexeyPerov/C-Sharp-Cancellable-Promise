# C-Sharp-Cancellable-Promise ![unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)                             

![stability-stable](https://img.shields.io/badge/stability-stable-green.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/Naereen/StrapDown.js/graphs/commit-activity)

That is a fork of [RSG.Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) 
with an addition of promises cancellation. 

## What is Promise?

A nice explanation of Promise concept can be found in this [thread](https://stackoverflow.com/questions/37364973/what-is-the-difference-between-promises-and-observables).

Shortly: there are basically two well-known abstractions that help us deal with the asynchronous code: Promises and Observables.
- Use Promises when you have a single async operation of which you want to process the result.
- Use Observables when there is a stream (of data) over time which you need to be handled.

## .. and what is added here comparing to the original repository?

Promises in most libraries (as of ECMAScript 6) are uncancellable by default. There are discussions about how to cancel promises in a right way going.
There is also a wide variety of approaches & libs in different langs that provide cancellation.   

Original [repository](https://github.com/Real-Serious-Games/C-Sharp-Promise) doesn't support it either. 
**This fork does add Cancellation support to it.** 

It is pretty straightforward: Cancel() cancels all the connected chain of Then & other calls (value & non-value promises; All & Race - all supports cancellation).
Promises now know about parents & children of each other. That is used to perform total cancellation whether you call Cancel like that:

```cs
var promise = new Promise();
promise.Then(...).Then(...);
promise.Cancel();
```

or like that

```cs
var promise = new Promise();
promise.Then(...);
promise.Then(...).Then(...).CancelWith(provider);
```

The only callbacks that are called upon Promise cancellation are:

```cs
public void Finally(Action onComplete);
public void OnCancel(Action onCancel);
```

See examples in [Example6.cs](./Promise.Examples/Example6.cs) 
and tests in [PromiseCancelTests.cs](./Promise.Tests/PromiseCancelTests.cs).

PS:
The original documentation can be found [here](https://github.com/Real-Serious-Games/C-Sharp-Promise). 
There you can get familiar with the Promise concept in more details.

## Breaking changes

- Finally() method changed to not return anything.

```cs
public void Finally(Action onComplete);
```

So you won't be able to call something like:

 ```cs
 var promise = new Promise();
 promise.Then(...).Finally(...).Then(...).CancelWith(provider);
 ```

which seems to be pretty ambigious.

Alternative would be:

```cs
var promise = new Promise();
promise.Then(...).Then(...).CancelWith(provider);
promive.Finally(...);
```

- Catch() rejects promise but there is CatchAsResolved() for backward compatibility.
- Generic promise now has a covariant modifier
- Dropped support of Catch(Func<Exception, TPromised>) tu support covariant modifier
- Code style fixed to be more .NET-style

## Cancellation utilities

For an additional code see branch dispose_service where you can find DisposeService project with tools to deal with cancellation and objects disposal in a neat way.
See examples:

```cs
public class ExampleMonoBehaviour : DisposableMonoBehaviour // plain MonoBehaviour will also be fine. CancelWith can deal with it too
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
```

## Getting it in your project

The simplest way would be to copy all sources from [/src/Promises folder](./src/Promises) to your Unity project. 

Otherwise, see the RSG.Promise project for the all code you may need.


