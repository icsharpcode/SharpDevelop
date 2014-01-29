// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
