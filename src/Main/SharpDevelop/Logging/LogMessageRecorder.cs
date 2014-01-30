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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;
using log4net.Appender;
using log4net.Core;

namespace ICSharpCode.SharpDevelop.Logging
{
	/// <summary>
	/// Records log4net log messages to a cyclic buffer for the purpose of creating better error reports.
	/// </summary>
	sealed class LogMessageRecorder : AppenderSkeleton
	{
		/// <summary>
		/// Gets the default buffer size used by the recorder.
		/// </summary>
		public const int DefaultBufferSize = 20;
		
		LoggingEvent[] buffer = new LoggingEvent[DefaultBufferSize];
		int bufferSize = DefaultBufferSize;
		int nextIndex;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LogMessageRecorder"/> class.
		/// </summary>
		public LogMessageRecorder() : base()
		{
		}
		
		/// <summary>
		/// Specifies how many log messages the recorder stores.
		/// </summary>
		/// <value>The size of the cyclic buffer for the recent log messages.</value>
		/// <remarks>Setting this property clears the buffer.</remarks>
		public int BufferSize {
			get { return bufferSize; }
			set {
				lock(this) {
					bufferSize = value;
					buffer = new LoggingEvent[bufferSize];
					nextIndex = 0;
				}
			}
		}
		
		/// <summary>
		/// Gets a read-only snapshot of the recorded events
		/// currently stored in the buffer.
		/// The returned collection contains the events
		/// in the same order as they have been appended.
		/// </summary>
		ICollection<LoggingEvent> RecordedEvents {
			get {
				lock(this) {
					List<LoggingEvent> events = new List<LoggingEvent>(bufferSize);
					int i = nextIndex;
					LoggingEvent e;
					do {
						e = buffer[i++];
						if (e != null) {
							events.Add(e);
						}
						if (i >= bufferSize) {
							i = 0;
						}
					} while (i != nextIndex);
					return events.AsReadOnly();
				}
			}
		}
		
		/// <summary>
		/// Records a logging event.
		/// </summary>
		/// <param name="loggingEvent">The event to append.</param>
		/// <remarks>
		/// This method is already thread-safe because the caller base.DoAppend
		/// holds a lock on this.
		/// </remarks>
		protected override void Append(LoggingEvent loggingEvent)
		{
			loggingEvent.Fix = FixFlags.Exception | FixFlags.Message | FixFlags.ThreadName;
			buffer[nextIndex] = loggingEvent;
			if (++nextIndex >= bufferSize) {
				nextIndex = 0;
			}
		}
		
		/// <summary>
		/// Appends the recent log messages recorded by the <see cref="LogMessageRecorder"/>
		/// to the specified <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> to append the rendered log messages to.</param>
		/// <param name="log">An <see cref="ILog"/> that points to a logger which is in the repository that contains the <see cref="LogMessageRecorder"/> appender.</param>
		public static void AppendRecentLogMessages(StringBuilder sb, ILog log)
		{
			LogMessageRecorder recorder = log.Logger.Repository.GetAppenders().OfType<LogMessageRecorder>().Single();
			foreach (LoggingEvent e in recorder.RecordedEvents) {
				sb.Append(e.TimeStamp.ToString(@"HH\:mm\:ss\.fff", CultureInfo.InvariantCulture));
				sb.Append(" [");
				sb.Append(e.ThreadName);
				sb.Append("] ");
				sb.Append(e.Level.Name);
				sb.Append(" - ");
				sb.Append(e.RenderedMessage);
				sb.AppendLine();
				
				if (e.ExceptionObject != null) {
					sb.AppendLine("--> Exception:");
					sb.AppendLine(e.GetExceptionString());
				}
			}
		}
	}
}
