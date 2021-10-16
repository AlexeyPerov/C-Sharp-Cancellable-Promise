using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace RSG
{
    public static class CancelableExtensions
    {
        public static ICancelable FindLastPendingParent(this ICancelable cancelable)
        {
            ICancelable result;
            var next = cancelable;
            
            do
            {
                result = next;
                next = result.Parent;
            } while (next != null && next.CanBeCanceled);

            return result;
        }

        public static List<ICancelable> GetCancelSequenceFromParentToThis(this ICancelable cancelable)
        {
            var result = new List<ICancelable>();

            var next = cancelable;
            
            do
            {
                result.Insert(0, next);
                next = next.Parent;
            } while (next != null && next.CanBeCanceled);
            
            return result;
        }
        
        public static List<ICancelable> CollectSelfAndAllPendingChildren(this ICancelable cancelable)
        {
            var result = new List<ICancelable>();
            AddSelfAndChildren(result, cancelable);
            return result;
        }

        private static void AddSelfAndChildren(List<ICancelable> result, ICancelable cancelable)
        {
            if (cancelable.CanBeCanceled)
            {
                result.Add(cancelable);
            }

            foreach (var child in cancelable.Children.Where(x => x.CanBeCanceled))
            {
                AddSelfAndChildren(result, child);
            }
        }
        
        public static string GetTreeDescription(this ICancelable promise)
        {
            var result = string.Empty;
            RecordSelfAndChildren(ref result, promise, 0);
            return result;
        }
        
        private static void RecordSelfAndChildren(ref string result, ICancelable promise, int indent)
        {
            var indentString = "\n";

            for (var i = 0; i < indent; i++)
            {
                indentString += " ";
            }
            
            result += indentString + promise.CanBeCanceled;

            foreach (var child in promise.Children)
            {
                RecordSelfAndChildren(ref result, child, indent + 1);
            }
        }
    }
}