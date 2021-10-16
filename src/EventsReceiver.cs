using System;
using RSG.Exceptions;

namespace RSG
{
    public static class EventsReceiver
    {
        private static IEventsReceiver Receiver { get; set; }= new DefaultEventsReceiver();

        public static void SetLogger(IEventsReceiver receiver)
        {
            Receiver = receiver;
        }

        public static void OnVerbose(string message)
        {
            Receiver?.OnVerbose(message);
        }

        public static void OnWarningMinor(string message)
        {
            Receiver?.OnWarningMinor(message);
        }

        public static void OnWarning(string message)
        {
            Receiver?.OnWarning(message);
        }

        public static void OnStateException(PromiseStateException exception)
        {
            Receiver?.OnStateException(exception);
        }

        public static void OnException(Exception exception)
        {
            Receiver?.OnException(exception);
        }
    }

    public interface IEventsReceiver
    {
        void OnVerbose(string message);
        void OnWarningMinor(string message);
        void OnWarning(string message);
        void OnStateException(PromiseStateException exception);
        void OnException(Exception exception);
    }

    public class DefaultEventsReceiver : IEventsReceiver
    {
        public void OnVerbose(string message)
        {
            
        }

        public void OnWarningMinor(string message)
        {
            
        }

        public void OnWarning(string message)
        {
            
        }

        public void OnStateException(PromiseStateException exception)
        {
            throw exception;
        }

        public void OnException(Exception exception)
        {
            // throw exception;
        }
    }

}