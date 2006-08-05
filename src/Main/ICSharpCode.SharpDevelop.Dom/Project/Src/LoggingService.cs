// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using log4net;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// We don't reference ICSharpCode.Core but still need the logging interface.
	/// </summary>
	internal static class LoggingService
	{
		static ILog log = LogManager.GetLogger(typeof(LoggingService));
		
		public static void Debug(object message)
		{
			log.Debug(message);
		}
		
		public static void Info(object message)
		{
			log.Info(message);
		}
		
		public static void Warn(object message)
		{
			log.Warn(message);
		}
		
		public static void Warn(object message, Exception exception)
		{
			log.Warn(message, exception);
		}
		
		public static void Error(object message)
		{
			log.Error(message);
		}
		
		public static void Error(object message, Exception exception)
		{
			log.Error(message, exception);
		}
		
		public static bool IsDebugEnabled {
			get {
				return log.IsDebugEnabled;
			}
		}
	}
}
