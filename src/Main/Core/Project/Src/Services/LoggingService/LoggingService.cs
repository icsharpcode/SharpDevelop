// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core.Services;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class for easy logging.
	/// </summary>
	public static class LoggingService
	{
		public static void Debug(object message)
		{
			ServiceManager.LoggingService.Debug(message);
		}
		
		public static void DebugFormatted(string format, params object[] args)
		{
			ServiceManager.LoggingService.DebugFormatted(format, args);
		}
		
		public static void Info(object message)
		{
			ServiceManager.LoggingService.Info(message);
		}
		
		public static void InfoFormatted(string format, params object[] args)
		{
			ServiceManager.LoggingService.InfoFormatted(format, args);
		}
		
		public static void Warn(object message)
		{
			ServiceManager.LoggingService.Warn(message);
		}
		
		public static void Warn(object message, Exception exception)
		{
			ServiceManager.LoggingService.Warn(message, exception);
		}
		
		public static void WarnFormatted(string format, params object[] args)
		{
			ServiceManager.LoggingService.WarnFormatted(format, args);
		}
		
		public static void Error(object message)
		{
			ServiceManager.LoggingService.Error(message);
		}
		
		public static void Error(object message, Exception exception)
		{
			ServiceManager.LoggingService.Error(message, exception);
		}
		
		public static void ErrorFormatted(string format, params object[] args)
		{
			ServiceManager.LoggingService.ErrorFormatted(format, args);
		}
		
		public static void Fatal(object message)
		{
			ServiceManager.LoggingService.Fatal(message);
		}
		
		public static void Fatal(object message, Exception exception)
		{
			ServiceManager.LoggingService.Fatal(message, exception);
		}
		
		public static void FatalFormatted(string format, params object[] args)
		{
			ServiceManager.LoggingService.FatalFormatted(format, args);
		}
		
		public static bool IsDebugEnabled {
			get {
				return ServiceManager.LoggingService.IsDebugEnabled;
			}
		}
		
		public static bool IsInfoEnabled {
			get {
				return ServiceManager.LoggingService.IsInfoEnabled;
			}
		}
		
		public static bool IsWarnEnabled {
			get {
				return ServiceManager.LoggingService.IsWarnEnabled;
			}
		}
		
		public static bool IsErrorEnabled {
			get {
				return ServiceManager.LoggingService.IsErrorEnabled;
			}
		}
		
		public static bool IsFatalEnabled {
			get {
				return ServiceManager.LoggingService.IsFatalEnabled;
			}
		}
	}
}
