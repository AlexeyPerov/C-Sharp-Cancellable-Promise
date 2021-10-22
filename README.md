# C-Sharp-Cancellable-Promise ![unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)                             

![stability-stable](https://img.shields.io/badge/stability-stable-green.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/Naereen/StrapDown.js/graphs/commit-activity)

That is a fork of [RSG.Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) 
with an addition of promises cancellation. 

## What is Promise?

The original documentation can be found [here](https://github.com/Real-Serious-Games/C-Sharp-Promise). There you can get familiar with Promise concept.

## What is added here

Promises cancellation via method

```cs
public void Cancel()
{
    if (CurState != PromiseState.Pending)
    {
        return;
    }

    CurState = PromiseState.Cancelled;
    if (EnablePromiseTracking)
    {
        PendingPromises.Remove(this);
    }
    InvokeCancelHandlers();
    ClearHandlers();
}
```

This method will cancel all promises that present within chain.


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

## Breaking changes

```cs
public void Finally(Action onComplete);
```

Finally() method changed to not return anything.
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

## Getting it in your project

Copy all sources from RSG.Promise to your Unity project.
