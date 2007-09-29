// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using log4net;
using log4net.Config;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class for easy logging. Uses log4net as backend.
	/// </summary>
	public static class LoggingService
	{
		static ILog log = LogManager.GetLogger(typeof(LoggingService));
		
		static LoggingService()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
		}
		
		public static void Debug(object message)
		{
			log.Debug(message);
		}
		
		public static void DebugFormatted(string format, params object[] args)
		{
			log.DebugFormat(format, args);
		}
		
		public static void Info(object message)
		{
			log.Info(message);
		}
		
		public static void InfoFormatted(string format, params object[] args)
		{
			log.InfoFormat(format, args);
		}
		
		public static void Warn(object message)
		{
			log.Warn(message);
		}
		
		public static void Warn(object message, Exception exception)
		{
			log.Warn(message, exception);
		}
		
		public static void WarnFormatted(string format, params object[] args)
		{
			log.WarnFormat(format, args);
		}
		
		public static void Error(object message)
		{
			log.Error(message);
		}
		
		public static void Error(object message, Exception exception)
		{
			log.Error(message, exception);
		}
		
		public static void ErrorFormatted(string format, params object[] args)
		{
			log.ErrorFormat(format, args);
		}
		
		public static void Fatal(object message)
		{
			log.Fatal(message);
		}
		
		public static void Fatal(object message, Exception exception)
		{
			log.Fatal(message, exception);
		}
		
		public static void FatalFormatted(string format, params object[] args)
		{
			log.FatalFormat(format, args);
		}
		
		public static bool IsDebugEnabled {
			get {
				return log.IsDebugEnabled;
			}
		}
		
		public static bool IsInfoEnabled {
			get {
				return log.IsInfoEnabled;
			}
		}
		
		public static bool IsWarnEnabled {
			get {
				return log.IsWarnEnabled;
			}
		}
		
		public static bool IsErrorEnabled {
			get {
				return log.IsErrorEnabled;
			}
		}
		
		public static bool IsFatalEnabled {
			get {
				return log.IsFatalEnabled;
			}
		}
	}
}
