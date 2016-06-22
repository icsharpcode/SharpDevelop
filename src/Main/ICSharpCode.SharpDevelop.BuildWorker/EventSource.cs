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
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	class EventSource : IEventSource, IEventRedirector
	{
		public event BuildMessageEventHandler MessageRaised;
		public event BuildErrorEventHandler ErrorRaised;
		public event BuildWarningEventHandler WarningRaised;
		public event BuildStartedEventHandler BuildStarted;
		public event BuildFinishedEventHandler BuildFinished;
		public event ProjectStartedEventHandler ProjectStarted;
		public event ProjectFinishedEventHandler ProjectFinished;
		public event TargetStartedEventHandler TargetStarted;
		public event TargetFinishedEventHandler TargetFinished;
		public event TaskStartedEventHandler TaskStarted;
		public event TaskFinishedEventHandler TaskFinished;
		public event CustomBuildEventHandler CustomEventRaised;
		public event BuildStatusEventHandler StatusEventRaised;
		
		public event AnyEventHandler AnyEventRaised;
		
		public static void EncodeEvent(BinaryWriter writer, BuildEventArgs e)
		{
			EventTypes type = GetEventType(e);
			writer.WriteInt32((int)type);
			writer.WriteNullableString(e.Message);
			writer.WriteNullableString(e.HelpKeyword);
			writer.WriteNullableString(e.SenderName);
			writer.WriteDateTime(e.Timestamp);
			switch (type) {
				case EventTypes.Error:
					BuildErrorEventArgs error = (BuildErrorEventArgs)e;
					writer.WriteNullableString(error.Subcategory);
					writer.WriteNullableString(error.Code);
					writer.WriteNullableString(error.File);
					writer.WriteInt32(error.LineNumber);
					writer.WriteInt32(error.ColumnNumber);
					writer.WriteInt32(error.EndLineNumber);
					writer.WriteInt32(error.EndColumnNumber);
					break;
				case EventTypes.Warning:
					BuildWarningEventArgs warning = (BuildWarningEventArgs)e;
					writer.WriteNullableString(warning.Subcategory);
					writer.WriteNullableString(warning.Code);
					writer.WriteNullableString(warning.File);
					writer.WriteInt32(warning.LineNumber);
					writer.WriteInt32(warning.ColumnNumber);
					writer.WriteInt32(warning.EndLineNumber);
					writer.WriteInt32(warning.EndColumnNumber);
					break;
				case EventTypes.Message:
					BuildMessageEventArgs message = (BuildMessageEventArgs)e;
					writer.WriteInt32((int)message.Importance);
					break;
				case EventTypes.BuildFinished:
					BuildFinishedEventArgs buildFinished = (BuildFinishedEventArgs)e;
					writer.Write(buildFinished.Succeeded);
					break;
				case EventTypes.BuildStarted:
					break;
				case EventTypes.ProjectFinished:
					ProjectFinishedEventArgs projectFinished = (ProjectFinishedEventArgs)e;
					writer.WriteNullableString(projectFinished.ProjectFile);
					writer.Write(projectFinished.Succeeded);
					break;
				case EventTypes.ProjectStarted:
					ProjectStartedEventArgs projectStarted = (ProjectStartedEventArgs)e;
					writer.WriteNullableString(projectStarted.ProjectFile);
					writer.WriteNullableString(projectStarted.TargetNames);
					break;
				case EventTypes.TargetFinished:
					TargetFinishedEventArgs targetFinished = (TargetFinishedEventArgs)e;
					writer.WriteNullableString(targetFinished.TargetName);
					writer.WriteNullableString(targetFinished.ProjectFile);
					writer.WriteNullableString(targetFinished.TargetFile);
					writer.Write(targetFinished.Succeeded);
					break;
				case EventTypes.TargetStarted:
					TargetStartedEventArgs targetStarted = (TargetStartedEventArgs)e;
					writer.WriteNullableString(targetStarted.TargetName);
					writer.WriteNullableString(targetStarted.ProjectFile);
					writer.WriteNullableString(targetStarted.TargetFile);
					#if MSBUILD35
					writer.WriteNullableString(null);
					#else
					writer.WriteNullableString(targetStarted.ParentTarget);
					#endif
					break;
				case EventTypes.TaskFinished:
					TaskFinishedEventArgs taskFinished = (TaskFinishedEventArgs)e;
					writer.WriteNullableString(taskFinished.ProjectFile);
					writer.WriteNullableString(taskFinished.TaskFile);
					writer.WriteNullableString(taskFinished.TaskName);
					writer.Write(taskFinished.Succeeded);
					break;
				case EventTypes.TaskStarted:
					TaskStartedEventArgs taskStarted = (TaskStartedEventArgs)e;
					writer.WriteNullableString(taskStarted.ProjectFile);
					writer.WriteNullableString(taskStarted.TaskFile);
					writer.WriteNullableString(taskStarted.TaskName);
					break;
				default: // unknown etc.
					break;
			}
			
		}
		
		#if !MSBUILD35
		public static BuildEventArgs DecodeEvent(BinaryReader reader)
		{
			EventTypes type = (EventTypes)reader.ReadInt32();
			string message = reader.ReadNullableString();
			string helpKeyword = reader.ReadNullableString();
			string senderName = reader.ReadNullableString();
			DateTime eventTimestamp = reader.ReadDateTime();
			switch (type) {
				case EventTypes.Error:
					return new BuildErrorEventArgs(reader.ReadNullableString(), reader.ReadNullableString(), reader.ReadNullableString(),
					                               reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(),
					                               message, helpKeyword, senderName, eventTimestamp);
				case EventTypes.Warning:
					return new BuildWarningEventArgs(reader.ReadNullableString(), reader.ReadNullableString(), reader.ReadNullableString(),
					                                 reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(),
					                                 message, helpKeyword, senderName, eventTimestamp);
				case EventTypes.Message:
					MessageImportance importance = (MessageImportance)reader.ReadInt32();
					return new BuildMessageEventArgs(message, helpKeyword, senderName, importance, eventTimestamp);
				case EventTypes.BuildFinished:
					return new BuildFinishedEventArgs(message, helpKeyword, reader.ReadBoolean(), eventTimestamp);
				case EventTypes.BuildStarted:
					return new BuildStartedEventArgs(message, helpKeyword, eventTimestamp);
				case EventTypes.ProjectFinished:
					return new ProjectFinishedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadBoolean(), eventTimestamp);
				case EventTypes.ProjectStarted:
					return new ProjectStartedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadNullableString(),
					                                   null, null, eventTimestamp);
				case EventTypes.TargetFinished:
					return new TargetFinishedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadNullableString(),
					                                   reader.ReadNullableString(), reader.ReadBoolean(), eventTimestamp, null);
				case EventTypes.TargetStarted:
					return new TargetStartedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadNullableString(),
					                                  reader.ReadNullableString(), reader.ReadNullableString(), eventTimestamp);
				case EventTypes.TaskFinished:
					return new TaskFinishedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadNullableString(),
					                                 reader.ReadNullableString(), reader.ReadBoolean(), eventTimestamp);
				case EventTypes.TaskStarted:
					return new TaskStartedEventArgs(message, helpKeyword, reader.ReadNullableString(), reader.ReadNullableString(),
					                                reader.ReadNullableString(), eventTimestamp);
				default:
					return new UnknownBuildEventArgs(message, helpKeyword, senderName, eventTimestamp);
			}
		}
		
		sealed class UnknownBuildEventArgs : CustomBuildEventArgs
		{
			public UnknownBuildEventArgs(string message, string helpKeyword, string senderName, DateTime eventTimestamp)
				: base(message, helpKeyword, senderName, eventTimestamp)
			{
			}
		}
		#endif
		
		public void ForwardEvent(BuildEventArgs e)
		{
			if (e is BuildStatusEventArgs) {
				if (e is TaskStartedEventArgs) {
					if (TaskStarted != null)
						TaskStarted(this, (TaskStartedEventArgs)e);
				} else if (e is TaskFinishedEventArgs) {
					if (TaskFinished != null)
						TaskFinished(this, (TaskFinishedEventArgs)e);
				} else if (e is TargetStartedEventArgs) {
					if (TargetStarted != null)
						TargetStarted(this, (TargetStartedEventArgs)e);
				} else if (e is TargetFinishedEventArgs) {
					if (TargetFinished != null)
						TargetFinished(this, (TargetFinishedEventArgs)e);
				} else if (e is ProjectStartedEventArgs) {
					if (ProjectStarted != null)
						ProjectStarted(this, (ProjectStartedEventArgs)e);
				} else if (e is ProjectFinishedEventArgs) {
					if (ProjectFinished != null)
						ProjectFinished(this, (ProjectFinishedEventArgs)e);
				} else if (e is BuildStartedEventArgs) {
					if (BuildStarted != null)
						BuildStarted(this, (BuildStartedEventArgs)e);
				} else if (e is BuildFinishedEventArgs) {
					if (BuildFinished != null)
						BuildFinished(this, (BuildFinishedEventArgs)e);
				}
				if (StatusEventRaised != null)
					StatusEventRaised(this, (BuildStatusEventArgs)e);
			} else if (e is BuildMessageEventArgs) {
				if (MessageRaised != null)
					MessageRaised(this, (BuildMessageEventArgs)e);
			} else if (e is BuildWarningEventArgs) {
				if (WarningRaised != null)
					WarningRaised(this, (BuildWarningEventArgs)e);
			} else if (e is BuildErrorEventArgs) {
				if (ErrorRaised != null)
					ErrorRaised(this, (BuildErrorEventArgs)e);
			} else if (e is CustomBuildEventArgs) {
				if (CustomEventRaised != null)
					CustomEventRaised(this, (CustomBuildEventArgs)e);
			}
			
			if (AnyEventRaised != null)
				AnyEventRaised(this, e);
		}
		
		public static EventTypes GetEventType(BuildEventArgs e)
		{
			if (e is TaskStartedEventArgs) {
				return EventTypes.TaskStarted;
			} else if (e is TaskFinishedEventArgs) {
				return EventTypes.TaskFinished;
			} else if (e is TargetStartedEventArgs) {
				return EventTypes.TargetStarted;
			} else if (e is TargetFinishedEventArgs) {
				return EventTypes.TargetFinished;
			} else if (e is BuildMessageEventArgs) {
				return EventTypes.Message;
			} else if (e is BuildWarningEventArgs) {
				return EventTypes.Warning;
			} else if (e is BuildErrorEventArgs) {
				return EventTypes.Error;
			} else if (e is ProjectStartedEventArgs) {
				return EventTypes.ProjectStarted;
			} else if (e is ProjectFinishedEventArgs) {
				return EventTypes.ProjectFinished;
			} else if (e is BuildStartedEventArgs) {
				return EventTypes.BuildStarted;
			} else if (e is BuildFinishedEventArgs) {
				return EventTypes.BuildFinished;
			} else if (e is CustomBuildEventArgs) {
				return EventTypes.Custom;
			} else {
				return EventTypes.Unknown;
			}
		}
	}
}
