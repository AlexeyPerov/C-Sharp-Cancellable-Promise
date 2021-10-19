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
            Case0();
            Console.ReadKey();
            Case1();
            Console.ReadKey();
            Case2();
            Console.ReadKey();
            Case3();
            Console.ReadKey();
            Case4();
            Console.ReadKey();
        }
        
        private static void Case0()
        {
            Console.WriteLine("Run Case 0. NONE of THEN will be called");
            
            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Fail("THEN 1"); });
                
            var handler11 = handler.Then(() => {Fail("THEN 1.1");}).Then(() => {Fail("THEN 1.1.1");});
            var handler12 = handler.Then(() => {Fail("THEN 1.2");}).Then(() => {Fail("THEN 1.2.1");});

            handler11.OnCancel(() => {Console.WriteLine("CANCELED 1.1");});
            handler12.OnCancel(() => {Console.WriteLine("CANCELED 1.2");});

            WaitFor(1, () =>
            {
                promise.TryResolve();
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            promise.Cancel();
        }

        private static void Case1()
        {
            Console.WriteLine("Run Case 1. BOTH THEN 1 & 2 will be called");
            
            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Fail("THEN 1"); })
                .Then(() => {Fail("THEN 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                promise.TryResolve();
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            handler.Cancel();
        }

        private static void Case2()
        {
            Console.WriteLine("Run Case 2. NONE of THEN 1 & 2 will be called");
            
            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Fail("THEN 1"); })
                .Then(() => {Fail("THEN 2");});
            
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
            Console.WriteLine("Run Case 3. NONE of THEN 1 & 2 will be called");

            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Fail("FAIL 1"); })
                .Then(() => {Fail("FAIL 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                if (promise.CanBeResolved)
                    promise.Resolve();
                
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            promise.Cancel();
        }

        private static void Case4()
        {
            Console.WriteLine("Run Case 4. NONE of THEN 1 & 2 will be called");

            var promise = new RSG.Promise();

            var handler = promise.Then(() => { Fail("THEN 1"); })
                .Then(() => {Fail("THEN 2");});
            
            handler.OnCancel(() => {Console.WriteLine("CANCELLED");});

            WaitFor(1, () =>
            {
                if (promise.CanBeResolved)
                    promise.Resolve();
                
                Console.WriteLine("TREE: " + promise.GetTreeDescription());
            });
            
            promise.Cancel();
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

        private static void Fail(string message)
        {
            throw new Exception(message);
        }
    }
}