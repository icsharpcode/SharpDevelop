// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	[Flags]
	public enum EventTypes
	{
		None            = 0,
		Message         = 0x0001,
		Error           = 0x0002,
		Warning         = 0x0004,
		BuildStarted    = 0x0008,
		BuildFinished   = 0x0010,
		ProjectStarted  = 0x0020,
		ProjectFinished = 0x0040,
		TargetStarted   = 0x0080,
		TargetFinished  = 0x0100,
		TaskStarted     = 0x0200,
		TaskFinished    = 0x0400,
		Custom          = 0x0800,
		Unknown         = 0x1000,
		All             = 0x1fff
	}
	
	class EventSource : IEventSource
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
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public void RaiseEvent(BuildEventArgs e)
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
