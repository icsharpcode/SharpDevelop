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
using ICSharpCode.Core;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
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
	class TaskBoundAdditionalLoggerDoozer : IDoozer
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
			
			public ILogger CreateLogger(IMSBuildLoggerContext context)
			{
				context.InterestingTasks.Add(taskname);
				return new TaskBoundAdditionalLogger(this, context);
			}
		}
		
		private class TaskBoundAdditionalLogger : ILogger
		{
			TaskBoundAdditionalLoggerDescriptor desc;
			IMSBuildLoggerContext context;
			ILogger baseLogger;
			bool isActive;
			
			public TaskBoundAdditionalLogger(TaskBoundAdditionalLoggerDescriptor desc, IMSBuildLoggerContext context)
			{
				this.desc = desc;
				this.context = context;
			}
			
			void CreateBaseLogger()
			{
				if (baseLogger == null) {
					object obj = desc.addIn.CreateObject(desc.classname);
					baseLogger = obj as ILogger;
					IMSBuildAdditionalLogger addLog = obj as IMSBuildAdditionalLogger;
					if (addLog != null) {
						baseLogger = addLog.CreateLogger(context);
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
