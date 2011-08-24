// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface for elements in /SharpDevelop/MSBuildEngine/AdditionalLoggers
	/// </summary>
	public interface IMSBuildAdditionalLogger
	{
		ILogger CreateLogger(MSBuildEngine engine);
	}
	
	/// <summary>
	/// Creates <see cref="IMSBuildAdditionalLogger"/> objects that are only
	/// activated when a specific MSBuild task is running.
	/// </summary>
	/// <attribute name="class" use="required">
	/// Name of the IMSBuildAdditionalLogger class.
	/// </attribute>
	/// <attribute name="taskname" use="required">
	/// Specifies the name of the MSBuild task that must be running for
	/// this logger to be active.
	/// </attribute>
	/// <example>
	/// &lt;TaskBoundAdditionalLogger
	/// 	id = "FxCopLogger"
	/// 	taskname = "FxCop"
	/// 	class = "ICSharpCode.CodeAnalysis.FxCopLogger"/&gt;
	/// </example>
	/// <usage>Only in /SharpDevelop/MSBuildEngine/AdditionalLoggers</usage>
	/// <returns>
	/// A IMSBuildAdditionalLogger object that lazy-loads the specified
	/// IMSBuildAdditionalLogger when the specified task is running.
	/// </returns>
	public class TaskBoundAdditionalLoggerDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new TaskBoundAdditionalLoggerDescriptor(args.Codon);
		}
		
		private class TaskBoundAdditionalLoggerDescriptor : IMSBuildAdditionalLogger
		{
			internal string taskname;
			internal string classname;
			internal AddIn addIn;
			
			public TaskBoundAdditionalLoggerDescriptor(Codon codon)
			{
				classname = codon.Properties["class"];
				taskname = codon.Properties["taskname"];
				addIn = codon.AddIn;
			}
			
			public ILogger CreateLogger(MSBuildEngine engine)
			{
				engine.InterestingTasks.Add(taskname);
				return new TaskBoundAdditionalLogger(this, engine);
			}
		}
		
		private class TaskBoundAdditionalLogger : ILogger
		{
			TaskBoundAdditionalLoggerDescriptor desc;
			MSBuildEngine engine;
			ILogger baseLogger;
			bool isActive;
			
			public TaskBoundAdditionalLogger(TaskBoundAdditionalLoggerDescriptor desc, MSBuildEngine engine)
			{
				this.desc = desc;
				this.engine = engine;
			}
			
			void CreateBaseLogger()
			{
				if (baseLogger == null) {
					object obj = desc.addIn.CreateObject(desc.classname);
					baseLogger = obj as ILogger;
					IMSBuildAdditionalLogger addLog = obj as IMSBuildAdditionalLogger;
					if (addLog != null) {
						baseLogger = addLog.CreateLogger(engine);
					}
				}
			}
			
			void OnTaskStarted(object sender, TaskStartedEventArgs e)
			{
				if (desc.taskname.Equals(e.TaskName, StringComparison.OrdinalIgnoreCase)) {
					CreateBaseLogger();
					if (baseLogger != null) {
						baseLogger.Initialize(eventSource);
						isActive = true;
					}
				}
			}
			
			void OnTaskFinished(object sender, TaskFinishedEventArgs e)
			{
				if (isActive) {
					baseLogger.Shutdown();
					isActive = false;
				}
			}
			
			#region ILogger interface implementation
			LoggerVerbosity verbosity = LoggerVerbosity.Minimal;
			
			public LoggerVerbosity Verbosity {
				get {
					return verbosity;
				}
				set {
					verbosity = value;
				}
			}
			
			string parameters;
			
			public string Parameters {
				get {
					return parameters;
				}
				set {
					parameters = value;
				}
			}
			
			IEventSource eventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				eventSource.TaskStarted  += OnTaskStarted;
				eventSource.TaskFinished += OnTaskFinished;
			}
			
			public void Shutdown()
			{
				OnTaskFinished(null, null);
				if (eventSource != null) {
					eventSource.TaskStarted  -= OnTaskStarted;
					eventSource.TaskFinished -= OnTaskFinished;
					eventSource = null;
				}
			}
			#endregion
		}
	}
}
