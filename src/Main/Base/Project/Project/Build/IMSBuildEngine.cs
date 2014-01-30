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
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface to static MSBuildEngine methods.
	/// </summary>
	[SDService]
	public interface IMSBuildEngine
	{
		/// <summary>
		/// Gets a list of the task names that cause a "Compiling ..." log message.
		/// You can add items to this set by putting strings into
		/// "/SharpDevelop/MSBuildEngine/CompileTaskNames".
		/// </summary>
		ISet<string> CompileTaskNames { get; }
		
		/// <summary>
		/// Gets the global MSBuild properties.
		/// You can add items to this dictionary by putting strings into
		/// "/SharpDevelop/MSBuildEngine/AdditionalProperties".
		/// </summary>
		IEnumerable<KeyValuePair<string, string>> GlobalBuildProperties { get; }
		
		/// <summary>
		/// Gets a list of additional target files that are automatically loaded into all projects.
		/// You can add items into this list by putting strings into
		/// "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles"
		/// </summary>
		IList<FileName> AdditionalTargetFiles { get; }
		
		/// <summary>
		/// Gets a list of additional MSBuild loggers.
		/// You can register your loggers by putting them into
		/// "/SharpDevelop/MSBuildEngine/AdditionalLoggers"
		/// </summary>
		IList<IMSBuildAdditionalLogger> AdditionalMSBuildLoggers { get; }
		
		/// <summary>
		/// Gets a list of MSBuild logger filter.
		/// You can register your loggers by putting them into
		/// "/SharpDevelop/MSBuildEngine/LoggerFilters"
		/// </summary>
		IList<IMSBuildLoggerFilter> MSBuildLoggerFilters { get; }
		
		/// <summary>
		/// Resolves the location of the reference files.
		/// </summary>
		IList<ReferenceProjectItem> ResolveAssemblyReferences(
			MSBuildBasedProject baseProject,
			ReferenceProjectItem[] additionalReferences = null, bool resolveOnlyAdditionalReferences = false,
			bool logErrorsToOutputPad = true);
		
		/// <summary>
		/// Compiles the specified project using MSBuild.
		/// </summary>
		/// <param name="project">The project to be built.</param>
		/// <param name="options">The options to use for building this project.</param>
		/// <param name="feedbackSink">Callback for errors/warning and log messages</param>
		/// <param name="cancellationToken">Cancellation token for aborting the build</param>
		/// <param name="additionalTargetFiles">Additional MSBuild target files that should be included in the build.
		/// Note: target files specified in the AddInTree path "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles" are always included
		/// and do not have to be specified.
		/// </param>
		/// <returns>True if the build completes successfully; false otherwise.</returns>
		Task<bool> BuildAsync(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, CancellationToken cancellationToken, IEnumerable<string> additionalTargetFiles = null);
	}
	
	public interface IMSBuildLoggerContext
	{
		/// <summary>
		/// The project being built.
		/// </summary>
		IProject Project { get; }
		
		/// <summary>
		/// Gets the name of the project file being compiled by this engine.
		/// </summary>
		FileName ProjectFileName { get; }
		
		/// <summary>
		/// Controls whether messages should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportMessageEvents { get; set; }
		
		/// <summary>
		/// Controls whether the TargetStarted event should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportTargetStartedEvents { get; set; }
		
		/// <summary>
		/// Controls whether the TargetStarted event should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportTargetFinishedEvents { get; set; }
		
		/// <summary>
		/// Controls whether all TaskStarted events should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportAllTaskStartedEvents { get; set; }
		
		/// <summary>
		/// Controls whether all TaskFinished events should be made available to loggers.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportAllTaskFinishedEvents { get; set; }
		
		/// <summary>
		/// Controls whether the AnyEventRaised and StatusEventRaised events should
		/// be called for unknown events.
		/// Logger AddIns should set this property in their CreateLogger method.
		/// </summary>
		bool ReportUnknownEvents { get; set; }
		
		/// <summary>
		/// The list of task names for which TaskStarted and TaskFinished events should be
		/// made available to loggers.
		/// Logger AddIns should add entries in their CreateLogger method.
		/// </summary>
		ISet<string> InterestingTasks { get; }
		
		/// <summary>
		/// Outputs a text line into the message log.
		/// </summary>
		void OutputTextLine(string text);
		
		/// <summary>
		/// Reports an error. This method bypasses the logger filter chain.
		/// </summary>
		void ReportError(BuildError error);
	}
	
	/// <summary>
	/// Interface for elements in /SharpDevelop/MSBuildEngine/AdditionalLoggers
	/// </summary>
	public interface IMSBuildAdditionalLogger
	{
		Microsoft.Build.Framework.ILogger CreateLogger(IMSBuildLoggerContext context);
	}
	
	/// <summary>
	/// Interface for elements in /SharpDevelop/MSBuildEngine/LoggerFilters
	/// </summary>
	public interface IMSBuildLoggerFilter
	{
		IMSBuildChainedLoggerFilter CreateFilter(IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextFilter);
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
}
