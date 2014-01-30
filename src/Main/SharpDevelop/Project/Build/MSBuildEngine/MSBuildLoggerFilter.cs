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
	class TaskBoundLoggerFilterDoozer : IDoozer
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
			
			public IMSBuildChainedLoggerFilter CreateFilter(IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextFilter)
			{
				if (nextFilter == null)
					throw new ArgumentNullException("nextFilter");
				// ensure the engine gets notified about start/end of this task
				context.InterestingTasks.Add(taskname);
				// Create a Filter that tracks whether the task is active.
				// If active, forward to 'baseFilter', otherwise forward to 'nextFilter'.
				return new TaskBoundLoggerFilter(this, context, nextFilter);
			}
		}
		
		sealed class TaskBoundLoggerFilter : IMSBuildChainedLoggerFilter
		{
			readonly TaskBoundLoggerFilterDescriptor desc;
			readonly IMSBuildLoggerContext context;
			readonly IMSBuildChainedLoggerFilter nextFilter;
			IMSBuildChainedLoggerFilter baseFilter = null;
			bool insideTask = false;
			
			public TaskBoundLoggerFilter(TaskBoundLoggerFilterDescriptor desc, IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextFilter)
			{
				this.desc = desc;
				this.context = context;
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
							baseFilter = baseLoggerFilter.CreateFilter(context, nextFilter) ?? nextFilter;
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
