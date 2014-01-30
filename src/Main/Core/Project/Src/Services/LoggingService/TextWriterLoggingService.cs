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
using System.IO;

namespace ICSharpCode.Core.Implementation
{
	/// <summary>
	/// LoggingService implementation that logs into a TextWriter.
	/// </summary>
	public class TextWriterLoggingService : ILoggingService
	{
		readonly TextWriter writer;
		
		public TextWriterLoggingService(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
			this.IsFatalEnabled = true;
			this.IsErrorEnabled = true;
			this.IsWarnEnabled = true;
			this.IsInfoEnabled = true;
			this.IsDebugEnabled = true;
		}
		
		void Write(object message, Exception exception)
		{
			if (message != null) {
				writer.WriteLine(message.ToString());
			}
			if (exception != null) {
				writer.WriteLine(exception.ToString());
			}
		}
		
		public bool IsDebugEnabled { get; set; }
		public bool IsInfoEnabled { get; set; }
		public bool IsWarnEnabled { get; set; }
		public bool IsErrorEnabled { get; set; }
		public bool IsFatalEnabled { get; set; }
		
		public void Debug(object message)
		{
			if (IsDebugEnabled) {
				Write(message, null);
			}
		}
		
		public void DebugFormatted(string format, params object[] args)
		{
			Debug(string.Format(format, args));
		}
		
		public void Info(object message)
		{
			if (IsInfoEnabled) {
				Write(message, null);
			}
		}
		
		public void InfoFormatted(string format, params object[] args)
		{
			Info(string.Format(format, args));
		}
		
		public void Warn(object message)
		{
			Warn(message, null);
		}
		
		public void Warn(object message, Exception exception)
		{
			if (IsWarnEnabled) {
				Write(message, exception);
			}
		}
		
		public void WarnFormatted(string format, params object[] args)
		{
			Warn(string.Format(format, args));
		}
		
		public void Error(object message)
		{
			Error(message, null);
		}
		
		public void Error(object message, Exception exception)
		{
			if (IsErrorEnabled) {
				Write(message, exception);
			}
		}
		
		public void ErrorFormatted(string format, params object[] args)
		{
			Error(string.Format(format, args));
		}
		
		public void Fatal(object message)
		{
			Fatal(message, null);
		}
		
		public void Fatal(object message, Exception exception)
		{
			if (IsFatalEnabled) {
				Write(message, exception);
			}
		}
		
		public void FatalFormatted(string format, params object[] args)
		{
			Fatal(string.Format(format, args));
		}
	}
}
