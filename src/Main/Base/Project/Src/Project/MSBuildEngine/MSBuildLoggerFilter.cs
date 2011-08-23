// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface for elements in /SharpDevelop/MSBuildEngine/LoggerFilters
	/// </summary>
	public interface IMSBuildLoggerFilter
	{
		IMSBuildChainedLoggerFilter CreateFilter(MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter);
	}
	
	/// <summary>
	/// Element in the logger filter chain.
	/// Receives build events and errors and forwards them to the next element in the chain (possibly after modifying the event).
	/// </summary>
	public interface IMSBuildChainedLoggerFilter
	{
		void HandleError(BuildError error);
		void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e);
	}
	
	/// <summary>
	/// Creates <see cref="IMSBuildLoggerFilter"/> objects that are only
	/// activated when a specific MSBuild task is running.
	/// </summary>
	/// <attribute name="class" use="required">
	/// Name of the IMSBuildLoggerFilter class.
	/// </attribute>
	/// <attribute name="taskname" use="required">
	/// Specifies the name of the MSBuild task that must be running for
	/// this logger to be active.
	/// </attribute>
	/// <example>
	/// &lt;TaskBoundLoggerFilter
	/// 	id = "FxCopLogger"
	/// 	taskname = "FxCop"
	/// 	class = "ICSharpCode.CodeAnalysis.FxCopLoggerFilter"/&gt;
	/// </example>
	/// <usage>Only in /SharpDevelop/MSBuildEngine/LoggerFilters</usage>
	/// <returns>
	/// A IMSBuildLoggerFilter object that lazy-loads the specified
	/// IMSBuildLoggerFilter when the specified task is running.
	/// </returns>
	public class TaskBoundLoggerFilterDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new TaskBoundLoggerFilterDescriptor(args.Codon);
		}
		
		sealed class TaskBoundLoggerFilterDescriptor : IMSBuildLoggerFilter
		{
			internal string taskname;
			internal string classname;
			internal AddIn addIn;
			
			public TaskBoundLoggerFilterDescriptor(Codon codon)
			{
				classname = codon.Properties["class"];
				taskname = codon.Properties["taskname"];
				addIn = codon.AddIn;
			}
			
			public IMSBuildChainedLoggerFilter CreateFilter(MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter)
			{
				if (nextFilter == null)
					throw new ArgumentNullException("nextFilter");
				// ensure the engine gets notified about start/end of this task
				engine.InterestingTasks.Add(taskname);
				// Create a Filter that tracks whether the task is active.
				// If active, forward to 'baseFilter', otherwise forward to 'nextFilter'.
				return new TaskBoundLoggerFilter(this, engine, nextFilter);
			}
		}
		
		sealed class TaskBoundLoggerFilter : IMSBuildChainedLoggerFilter
		{
			readonly TaskBoundLoggerFilterDescriptor desc;
			readonly MSBuildEngine engine;
			readonly IMSBuildChainedLoggerFilter nextFilter;
			IMSBuildChainedLoggerFilter baseFilter = null;
			bool insideTask = false;
			
			public TaskBoundLoggerFilter(TaskBoundLoggerFilterDescriptor desc, MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter)
			{
				this.desc = desc;
				this.engine = engine;
				this.nextFilter = nextFilter;
			}
			
			public void HandleError(BuildError error)
			{
				if (insideTask)
					baseFilter.HandleError(error);
				else
					nextFilter.HandleError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				TaskStartedEventArgs start = e as TaskStartedEventArgs;
				if (start != null && string.Equals(start.TaskName, desc.taskname, StringComparison.OrdinalIgnoreCase)) {
					insideTask = true;
					if (baseFilter == null) {
						IMSBuildLoggerFilter baseLoggerFilter = (IMSBuildLoggerFilter)desc.addIn.CreateObject(desc.classname);
						if (baseLoggerFilter != null)
							baseFilter = baseLoggerFilter.CreateFilter(engine, nextFilter) ?? nextFilter;
						else
							baseFilter = nextFilter;
					}
				}
				if (insideTask)
					baseFilter.HandleBuildEvent(e);
				else
					nextFilter.HandleBuildEvent(e);
				if (insideTask && e is TaskFinishedEventArgs) {
					insideTask = false;
				}
			}
		}
	}
}
