// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// IEventSource implementation. Used to forward events to submission-specific loggers.
	/// </summary>
	sealed class EventSource : IEventSource, IEventRedirector
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
		public void ForwardEvent(Microsoft.Build.Framework.BuildEventArgs e)
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
	}
}
