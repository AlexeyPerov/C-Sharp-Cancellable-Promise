using System;
using System.Threading;
using System.Threading.Tasks;
using RSG;

namespace Promise.Examples
{
    public class Example6
    {
        public static void Run()
        {
            Case1();
            Console.ReadKey();
            Case2();
            Console.ReadKey();
            Case3();
            Console.ReadKey();
            Case4();
            Console.ReadKey();
        }

        private static void Case1()
        {
            Console.WriteLine("Run Case 1. BOTH THEN 1 & 2 will be called");
            
            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Console.WriteLine("THEN 1"); })
                .Then(() => {Console.WriteLine("THEN 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                promise.TryResolve();
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            handler.CancelSelf();
        }

        private static void Case2()
        {
            Console.WriteLine("Run Case 2. NONE of FAIL 1 & 2 will be called");
            
            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Console.WriteLine("FAIL 1"); })
                .Then(() => {Console.WriteLine("FAIL 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                if (promise.CanBeResolved)
                    promise.Resolve();
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            handler.Cancel();
        }

        private static void Case3()
        {
            Console.WriteLine("Run Case 3. NONE of FAIL 1 & 2 will be called");

            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Console.WriteLine("FAIL 1"); })
                .Then(() => {Console.WriteLine("FAIL 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                if (promise.CanBeResolved)
                    promise.Resolve();
                
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            promise.CancelSelfAndAllChildren();
        }

        private static void Case4()
        {
            Console.WriteLine("Run Case 4. NONE of FAIL 1 & 2 will be called");

            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Console.WriteLine("FAIL 1"); })
                .Then(() => {Console.WriteLine("FAIL 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                if (promise.CanBeResolved)
                    promise.Resolve();
                
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            promise.CancelSelf();
        }

        private static void WaitFor(float seconds, Action callback)
        {
            var source = new CancellationTokenSource();
            
            Task.Run(async delegate
            {
                await Task.Delay((int)(seconds * 1000f), source.Token);
                callback();
            }, source.Token);
        }
    }
}