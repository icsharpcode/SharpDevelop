// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Services;

namespace PackageManagement.Tests.Helpers
{
	public class FakeLoggingService : ILoggingService
	{
		public bool IsDebugEnabled { get; set; }
		
		public bool IsInfoEnabled { get; set; }
		
		public bool IsWarnEnabled { get; set; }
		
		public bool IsErrorEnabled { get; set; }
		
		public bool IsFatalEnabled { get; set; }
		
		public void Debug(object message)
		{
		}
		
		public void DebugFormatted(string format, params object[] args)
		{
		}
		
		public void Info(object message)
		{
		}
		
		public void InfoFormatted(string format, params object[] args)
		{
		}
		
		public void Warn(object message)
		{
		}
		
		public void Warn(object message, Exception exception)
		{
		}
		
		public void WarnFormatted(string format, params object[] args)
		{
		}
		
		public void Error(object message)
		{
		}
		
		public Exception ExceptionLoggedAsError;
		
		public void Error(object message, Exception exception)
		{
			ExceptionLoggedAsError = exception;
		}
		
		public void ErrorFormatted(string format, params object[] args)
		{
		}
		
		public void Fatal(object message)
		{
		}
		
		public void Fatal(object message, Exception exception)
		{
		}
		
		public void FatalFormatted(string format, params object[] args)
		{
		}
	}
}
