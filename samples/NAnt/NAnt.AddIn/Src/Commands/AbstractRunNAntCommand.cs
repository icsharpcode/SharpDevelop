// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NAnt.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.NAnt.Commands
{
	/// <summary>
	/// The base class for all commands that run NAnt.
	/// </summary>
	public abstract class AbstractRunNAntCommand : AbstractMenuCommand
	{
		/// <summary>
		/// The default NAnt build filename.
		/// </summary>
		public static readonly string DefaultBuildFileName = "default.build";
		
		/// <summary>
		/// The default NAnt build file extension.
		/// </summary>
		public static readonly string NAntBuildFileExtension = ".build";
		
		const int Win32FileNotFoundErrorCode = 2;
		const int Win32PathNotFoundErrorCode = 3;
		
		static MessageViewCategory category;
		static NAntRunner runner;
		
		public AbstractRunNAntCommand()
		{
			if (runner == null) {
				runner = NAntRunnerSingleton.Runner;
				runner.NAntExited += new NAntExitEventHandler(NAntExited);
				runner.OutputLineReceived += new LineReceivedEventHandler(OutputLineReceived);
			}
		}
		
		public override void Run()
		{
		}
		
		public static bool IsActiveConfigurationDebug {
			get {
				
				bool isDebug = false;
				
				IProject project = ProjectService.CurrentProject;
				if (project != null) {
					if (String.Compare(project.ActiveConfiguration, "debug", true) == 0) {
						isDebug = true;
					}
				}
				
				return isDebug;
			}
		}
		
        /// <summary>
        /// Gets the NAnt build filename from the selected project. 
        /// </summary>
        /// <remarks>
        /// <para>The basic logic is:</para>
        /// <para>Look for a file called "default.build".</para>
        /// <para>Look for a file named after the project 
        /// "<projectName>.build".</para>
        /// <para>Look for the first ".build" file in the project.</para>
        /// <para>If multiple ".build" files exist then, like NAnt,
        /// this is an error, but currently we ignore this.</para>
        /// <para>Note that this does not look in the project folder 
        /// for a .build file that is not added to the project.
		/// </para>
        /// </remarks>
        /// <returns>The build filename for the project.</returns>
        protected string GetProjectBuildFileName()
        {
        	string fileName = String.Empty;
        	
        	IProject project = ProjectService.CurrentProject;
        	
        	// Look for "default.build".
        	string projectFileName = project.FileName;
        	
        	string buildFileName = Path.Combine(Path.GetDirectoryName(projectFileName), DefaultBuildFileName);
        	if (project.IsFileInProject(buildFileName)) {
        		fileName = buildFileName;
        	} else {
        		
        		// Look for <projectname>.build
        		buildFileName = Path.ChangeExtension(projectFileName, NAntBuildFileExtension);
        		if (project.IsFileInProject(buildFileName)) {
        	    	fileName = buildFileName;
        		} else {
        			
        			// Look for the first matching .build file.
        			ProjectItem projectItem = GetFirstMatchingFile(project, NAntBuildFileExtension);
        			if (projectItem != null) {
        				fileName = projectItem.FileName;
        			} else {
        				throw new NAntAddInException("Project does not contain a '.build' file.");
        			}
        		}
        	}
        
        	return fileName;
        }
        
        /// <summary>
        /// Runs any pre-build steps such as saving changed files.
        /// </summary>
        protected void RunPreBuildSteps()
        {
        	//ProjectService.DoBeforeCompileAction();
        	TaskService.ClearExceptCommentTasks();
        }
 
        /// <summary>
        /// Runs the default target in the NAnt build.
        /// </summary>
        /// <param name="buildFileName">The build file to run.</param>
        /// <param name="workingDirectory">The working folder for NAnt.</param>
        /// <param name="debug">Flag indicating whether to set the NAnt debug property.</param>
        protected void RunBuild(string buildFileName, string workingDirectory, bool debug) 
        {
        	RunBuild(buildFileName, workingDirectory, debug, String.Empty, String.Empty);
        }
        
        protected void RunBuild(string buildFileName, string workingDirectory, bool debug, string target)
        {
        	RunBuild(buildFileName, workingDirectory, debug, target, String.Empty);
        }
        
        /// <summary>
        /// Runs the specified target in the NAnt build.
        /// </summary>
        /// <param name="buildFileName">The build file to run.</param>
        /// <param name="workingDirectory">The working folder for NAnt.</param>
        /// <param name="debug">Flag indicating whether to set the NAnt debug property.</param>
        /// <param name="target">The NAnt target to run.</param>
        /// <param name="args">Command line arguments to pass to NAnt.</param>
        protected void RunBuild(string buildFileName, string workingDirectory, bool debug, string target, string args)
        {
        	if (IsBuildRunning) {
        		throw new NAntAddInException("A NAnt build is currently running.");
        	}
        	
        	Category.ClearText();
        	ShowOutputPad();
      		
			runner.BuildFileName = buildFileName;
			runner.NAntFileName = AddInOptions.NAntFileName;
			runner.Verbose = AddInOptions.Verbose;			
			runner.WorkingDirectory = workingDirectory;
			runner.Quiet = AddInOptions.Quiet;
			runner.ShowLogo = AddInOptions.ShowLogo;
			runner.DebugMode = AddInOptions.DebugMode;
			
			if (debug) {
				runner.Arguments = String.Concat("-D:debug=true ", AddInOptions.NAntArguments, " ", args, " ", target);
			} else {
				runner.Arguments = String.Concat(AddInOptions.NAntArguments, " ", args, " ", target);
			}
			
			CategoryWriteLine(StringParser.Parse("Running NAnt."));
			CategoryWriteLine(runner.CommandLine);
			
			try {
				runner.Start();
			} catch (Win32Exception ex) {
				if (ex.NativeErrorCode == Win32FileNotFoundErrorCode || ex.NativeErrorCode == Win32PathNotFoundErrorCode) {
					throw new NAntAddInException(GetNAntNotFoundErrorMessage(AddInOptions.NAntFileName), ex);
				} else {
					throw;
				}
			}
		}	
        
        /// <summary>
        /// Gets any extra arguments from the NAnt pad's text box.
        /// </summary>
        protected string GetPadTextBoxArguments()
        {
        	string arguments = String.Empty;

       		IWorkbench Workbench = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench;
			PadDescriptor padDescriptor = Workbench.GetPad(typeof(NAntPadContent));
					
			if (padDescriptor != null && padDescriptor.PadContent != null) {
				arguments = ((NAntPadContent)padDescriptor.PadContent).Arguments;
			}
			
			return arguments;
        }
        
        /// <summary>
        /// Stops the currently running build.
        /// </summary>
        protected void StopBuild()
        {
        	if (IsBuildRunning) {
        		if (MessageService.AskQuestion(StringParser.Parse("This will terminate the NAnt process.  Are you sure?"))) {
        			runner.Stop();
        			CategoryWriteLine(StringParser.Parse("NAnt build stopped."));
        		}
        	}
        }
        
        protected bool IsBuildRunning {
        	get {
        		return runner.IsRunning;
        	}
        }
        
        /// <summary>
        /// Gets the NAnt message view output window.
        /// </summary>
        MessageViewCategory Category {
        	get {
	        	if (category == null) {
					category = new MessageViewCategory("NAnt");
					CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					cmv.AddCategory(category);        		
	        	}
	        	
	        	return category;
        	}
        }
        
        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        void CategoryWriteLine(string message)
        {
        	Category.AppendText(String.Concat(message, Environment.NewLine));
        }
        
        /// <summary>
		/// Brings output pad to the front.
		/// </summary>
		void ShowOutputPad()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();	
		}
        
        /// <summary>
		/// Looks for the first file that matches the specified
		/// file extension.
		/// </summary>
		/// <param name="extension">A filename extension.</param>
		/// <returns>A ProjectItem that has the specified extension,
		/// or null.</returns>
		ProjectItem GetFirstMatchingFile(IProject project, string extension)
		{
			foreach (ProjectItem projectItem in project.Items) {
				string projectFileNameExtension = Path.GetExtension(projectItem.FileName);
				if (String.Compare(projectFileNameExtension, extension, true) == 0) {
					return projectItem;
				}
			}
			return null;
		}        
		
		/// <summary>
        /// Displays the output from NAnt after it has exited.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The NAnt exit event arguments.</param>
        void NAntExited(object sender, NAntExitEventArgs e)
		{  
        	// Update output window.
        	string outputText = String.Empty;
        	
        	System.Diagnostics.Debug.Assert(e.Error.Length == 0);

        	outputText = String.Concat(outputText, e.Output);
			
			// Update task list.
			TaskCollection tasks = NAntOutputParser.Parse(outputText);
			foreach (Task task in tasks) {
				WorkbenchSingleton.SafeThreadAsyncCall(TaskService.Add, task);
			}
			
			// Bring task list to front.
			if (tasks.Count > 0 && ErrorListPad.ShowAfterBuild) {				
				IWorkbench workbench = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench;
				PadDescriptor padDescriptor = workbench.GetPad(typeof(ErrorListPad));		
				if (padDescriptor != null) {
					WorkbenchSingleton.SafeThreadAsyncCall(padDescriptor.BringPadToFront);
				}
			}			
        }
        
        void OutputLineReceived(object sender, LineReceivedEventArgs e)
        {
        	CategoryWriteLine(e.Line);
        }
        
        string GetNAntNotFoundErrorMessage(string fileName)
        {
        	string formatString = "Unable to find NAnt '{0}'.\r\n\r\n" +
				"Please configure the NAnt executable's location in the SharpDevelop Options.";
        	
        	return String.Format(formatString, fileName);
        }
	}
}
