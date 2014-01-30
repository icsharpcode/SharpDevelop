// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class for easy logging.
	/// </summary>
	public static class LoggingService
	{
		static ILoggingService Service {
			get { return ServiceSingleton.GetRequiredService<ILoggingService>(); }
		}
		
		public static void Debug(object message)
		{
			Service.Debug(message);
		}
		
		public static void DebugFormatted(string format, params object[] args)
		{
			Service.DebugFormatted(format, args);
		}
		
		public static void Info(object message)
		{
			Service.Info(message);
		}
		
		public static void InfoFormatted(string format, params object[] args)
		{
			Service.InfoFormatted(format, args);
		}
		
		public static void Warn(object message)
		{
			Service.Warn(message);
		}
		
		public static void Warn(object message, Exception exception)
		{
			Service.Warn(message, exception);
		}
		
		public static void WarnFormatted(string format, params object[] args)
		{
			Service.WarnFormatted(format, args);
		}
		
		public static void Error(object message)
		{
			Service.Error(message);
		}
		
		public static void Error(object message, Exception exception)
		{
			Service.Error(message, exception);
		}
		
		public static void ErrorFormatted(string format, params object[] args)
		{
			Service.ErrorFormatted(format, args);
		}
		
		public static void Fatal(object message)
		{
			Service.Fatal(message);
		}
		
		public static void Fatal(object message, Exception exception)
		{
			Service.Fatal(message, exception);
		}
		
		public static void FatalFormatted(string format, params object[] args)
		{
			Service.FatalFormatted(format, args);
		}
		
		public static bool IsDebugEnabled {
			get {
				return Service.IsDebugEnabled;
			}
		}
		
		public static bool IsInfoEnabled {
			get {
				return Service.IsInfoEnabled;
			}
		}
		
		public static bool IsWarnEnabled {
			get {
				return Service.IsWarnEnabled;
			}
		}
		
		public static bool IsErrorEnabled {
			get {
				return Service.IsErrorEnabled;
			}
		}
		
		public static bool IsFatalEnabled {
			get {
				return Service.IsFatalEnabled;
			}
		}
	}
}
