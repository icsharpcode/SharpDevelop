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
using System.Globalization;
using System.IO;

using ICSharpCode.Core;
using log4net;
using log4net.Config;

namespace ICSharpCode.SharpDevelop.Logging
{
	sealed class log4netLoggingService : ILoggingService
	{
		ILog log;
		
		public log4netLoggingService()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
			log = LogManager.GetLogger(typeof(log4netLoggingService));
		}
		
		public void Debug(object message)
		{
			log.Debug(message);
		}
		
		public void DebugFormatted(string format, params object[] args)
		{
			log.DebugFormat(CultureInfo.InvariantCulture, format, args);
		}
		
		public void Info(object message)
		{
			log.Info(message);
		}
		
		public void InfoFormatted(string format, params object[] args)
		{
			log.InfoFormat(CultureInfo.InvariantCulture, format, args);
		}
		
		public void Warn(object message)
		{
			log.Warn(message);
		}
		
		public void Warn(object message, Exception exception)
		{
			log.Warn(message, exception);
		}
		
		public void WarnFormatted(string format, params object[] args)
		{
			log.WarnFormat(CultureInfo.InvariantCulture, format, args);
		}
		
		public void Error(object message)
		{
			log.Error(message);
		}
		
		public void Error(object message, Exception exception)
		{
			log.Error(message, exception);
		}
		
		public void ErrorFormatted(string format, params object[] args)
		{
			log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
		}
		
		public void Fatal(object message)
		{
			log.Fatal(message);
		}
		
		public void Fatal(object message, Exception exception)
		{
			log.Fatal(message, exception);
		}
		
		public void FatalFormatted(string format, params object[] args)
		{
			log.FatalFormat(CultureInfo.InvariantCulture, format, args);
		}
		
		public bool IsDebugEnabled {
			get {
				return log.IsDebugEnabled;
			}
		}
		
		public bool IsInfoEnabled {
			get {
				return log.IsInfoEnabled;
			}
		}
		
		public bool IsWarnEnabled {
			get {
				return log.IsWarnEnabled;
			}
		}
		
		public bool IsErrorEnabled {
			get {
				return log.IsErrorEnabled;
			}
		}
		
		public bool IsFatalEnabled {
			get {
				return log.IsFatalEnabled;
			}
		}
	}
}
