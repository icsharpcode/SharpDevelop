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
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class SDConsoleLogger : ConsoleLogger
	{
		IBuildFeedbackSink feedbackSink;
		
		public SDConsoleLogger(IBuildFeedbackSink feedbackSink, LoggerVerbosity verbosity)
			: base(verbosity)
		{
			if (feedbackSink == null)
				throw new ArgumentNullException("feedbackSink");
			this.feedbackSink = feedbackSink;
			this.ShowSummary = false;
			base.WriteHandler = Write;
		}
		
		StringBuilder buffer = new StringBuilder();
		
		void Write(string text)
		{
			// ReportMessage takes full lines, so we have to put stuff into a buffer until we get a whole line
			int sendUpTo = text.LastIndexOf('\n');
			int oldBufferLength = buffer.Length;
			buffer.Append(text);
			if (sendUpTo >= 0) {
				sendUpTo += oldBufferLength;
				Debug.Assert(buffer[sendUpTo] == '\n');
				int terminatorLength = 1;
				if (sendUpTo > 0 && buffer[sendUpTo - 1] == '\r') {
					sendUpTo--;
					terminatorLength++;
				}
				feedbackSink.ReportMessage(buffer.ToString(0, sendUpTo));
				buffer.Remove(0, sendUpTo + terminatorLength);
			}
		}
	}
}
