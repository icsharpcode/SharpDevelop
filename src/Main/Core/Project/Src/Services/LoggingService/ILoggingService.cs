// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Implementation;

namespace ICSharpCode.Core
{
	[SDService("SD.Log", FallbackImplementation = typeof(FallbackLoggingService))]
	public interface ILoggingService
	{
		void Debug(object message);
		void DebugFormatted(string format, params object[] args);
		void Info(object message);
		void InfoFormatted(string format, params object[] args);
		void Warn(object message);
		void Warn(object message, Exception exception);
		void WarnFormatted(string format, params object[] args);
		void Error(object message);
		void Error(object message, Exception exception);
		void ErrorFormatted(string format, params object[] args);
		void Fatal(object message);
		void Fatal(object message, Exception exception);
		void FatalFormatted(string format, params object[] args);
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
	}
	
	sealed class FallbackLoggingService : TextWriterLoggingService
	{
		public FallbackLoggingService() : base(new TraceTextWriter()) {}
	}
}
