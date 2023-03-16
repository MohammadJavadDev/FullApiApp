using NLog;
using NLog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJ.NLog.Mongo
{
    public static class ExceptionHelper
    {
        private const string LoggedKey = "NLog.ExceptionLoggedToInternalLogger";
 
        public static void MarkAsLoggedToInternalLogger(this Exception exception)
        {
            if (exception != null)
            {
                exception.Data[LoggedKey] = true;
            }
        }
         public static bool IsLoggedToInternalLogger(this Exception exception)
        {
            if (exception != null)
            {
                return exception.Data[LoggedKey] as bool? ?? false;
            }
            return false;
        }


 
        public static bool MustBeRethrown(this Exception exception)
        {
            if (exception.MustBeRethrownImmediately())
                return true;

            var isConfigError = exception is NLogConfigurationException;

            //we throw always configuration exceptions (historical)
            if (!exception.IsLoggedToInternalLogger())
            {
                var level = isConfigError ? LogLevel.Warn : LogLevel.Error;
                InternalLogger.Log(exception, level, "Error has been raised.");
            }

            //if ThrowConfigExceptions == null, use  ThrowExceptions
            var shallRethrow = isConfigError ? (LogManager.ThrowConfigExceptions ?? LogManager.ThrowExceptions) : LogManager.ThrowExceptions;
            return shallRethrow;
        }

 
        public static bool MustBeRethrownImmediately(this Exception exception)
        {

#if !NETSTANDARD1_5
            if (exception is StackOverflowException)
                return true;

            if (exception is ThreadAbortException)
                return true;
#endif

            if (exception is OutOfMemoryException)
                return true;

            return false;
        }
    }
}
