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
