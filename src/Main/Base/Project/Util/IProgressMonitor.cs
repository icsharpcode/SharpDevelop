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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Represents a target where an active task reports progress to.
	/// </summary>
	/// <remarks>
	/// This interface is not thread-safe, but it can be used to report progress from background threads which will be
	/// automatically sent to the correct GUI thread.
	/// Using a progress monitor from multiple threads is possible if the user synchronizes the access.
	/// </remarks>
	public interface IProgressMonitor : IDisposable, IProgress<double>
	{
		/// <summary>
		/// Gets/Sets the amount of work already done within this task.
		/// Always uses a scale from 0 to 1 local to this progress monitor.
		/// </summary>
		double Progress { get; set; }
		
		/// <summary>
		/// Creates a nested task.
		/// </summary>
		/// <param name="workAmount">The amount of work this sub-task performs in relation to the work of this task.
		/// That means, this parameter is used as a scaling factor for work performed within the subtask.</param>
		/// <returns>A new progress monitor representing the sub-task.
		/// Multiple child progress monitors can be used at once; even concurrently on multiple threads.</returns>
		IProgressMonitor CreateSubTask(double workAmount);
		
		/// <summary>
		/// Creates a nested task.
		/// </summary>
		/// <param name="workAmount">The amount of work this sub-task performs in relation to the work of this task.
		/// That means, this parameter is used as a scaling factor for work performed within the subtask.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the sub-task.
		/// Note: cancelling the main task will not cancel the sub-task.
		/// </param>
		/// <returns>A new progress monitor representing the sub-task.
		/// Multiple child progress monitors can be used at once; even concurrently on multiple threads.</returns>
		IProgressMonitor CreateSubTask(double workAmount, CancellationToken cancellationToken);
		
		/// <summary>
		/// Gets/Sets the name to show while the task is active.
		/// </summary>
		string TaskName { get; set; }
		
		/// <summary>
		/// Gets/sets if the task current shows a modal dialog. Set this property to true to make progress
		/// dialogs windows temporarily invisible while your modal dialog is showing.
		/// </summary>
		bool ShowingDialog { // TODO: get rid of this. Don't mix calculations and UI!
			get;
			set;
		}
		
		/// <summary>
		/// Gets the cancellation token.
		/// </summary>
		CancellationToken CancellationToken { get; }
		
		/// <summary>
		/// Gets/Sets the operation status.
		/// 
		/// Note: the status of the whole operation is the most severe status of all nested monitors.
		/// The more severe value persists even if the child monitor gets disposed.
		/// </summary>
		OperationStatus Status { get; set; }
	}
	
	/// <summary>
	/// Represents the status of a operation with progress monitor.
	/// </summary>
	public enum OperationStatus : byte
	{
		/// <summary>
		/// Everything is normal.
		/// </summary>
		Normal,
		/// <summary>
		/// There was at least one warning.
		/// </summary>
		Warning,
		/// <summary>
		/// There was at least one error.
		/// </summary>
		Error
	}
	
	/// <summary>
	/// Dummy progress monitor implementation that does not report the progress anywhere.
	/// </summary>
	public sealed class DummyProgressMonitor : IProgressMonitor
	{
		public string TaskName { get; set; }
		
		public bool ShowingDialog { get; set; }
		
		public OperationStatus Status { get; set; }
		
		public CancellationToken CancellationToken { get; set; }
		
		public double Progress { get; set; }
		
		public IProgressMonitor CreateSubTask(double workAmount)
		{
			return new DummyProgressMonitor() { CancellationToken = this.CancellationToken };
		}
		
		public IProgressMonitor CreateSubTask(double workAmount, CancellationToken cancellationToken)
		{
			return new DummyProgressMonitor() { CancellationToken = cancellationToken };
		}
		
		void IProgress<double>.Report(double value)
		{
			this.Progress = value;
		}
		
		public void Dispose()
		{
		}
	}
}
