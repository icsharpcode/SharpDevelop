// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using ICSharpCode.NAntAddIn.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using System;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Commands
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
					if (String.Compare(project.Configuration, "debug", true) == 0) {
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
        				throw new NAntAddInException(StringParser.Parse("${res:ICSharpCode.NAntAddIn.AbstractRunNAntCommand.NoBuildFileErrorText}"));
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
        	TaskService.Clear();
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
        		throw new NAntAddInException(StringParser.Parse("${res:ICSharpCode.NAntAddIn.AbstractRunNAntCommand.BuildRunningErrorText}"));
        	}
        	
        	Category.ClearText();
      		
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
			
			CategoryWriteLine(StringParser.Parse("${res:ICSharpCode.NAntAddIn.AbstractRunNAntCommand.RunningNAntMessage}"));
			CategoryWriteLine(runner.CommandLine);
			
			runner.Start();
		}	
        
        /// <summary>
        /// Gets any extra arguments from the NAnt pad's text box.
        /// </summary>
        /// <returns></returns>
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
        		if (MessageService.AskQuestion(StringParser.Parse("${res:ICSharpCode.NAntAddIn.AbstractRunNAntCommand.TerminateNAntQuestion}"))) {
        			runner.Stop();
        			CategoryWriteLine(StringParser.Parse("${res:ICSharpCode.NAntAddIn.AbstractRunNAntCommand.NAntStoppedMessage}"));
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
					category = new MessageViewCategory("NAnt", "NAnt");
					CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					cmv.AddCategory(category);        		
	        	}
	        	
	        	return category;
        	}
        }
        
        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        /// <param name="message"></param>
        void CategoryWriteLine(string message)
        {
        	Category.AppendText(String.Concat(message, Environment.NewLine));
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
			ProjectItem matchedProjectItem = null;
			
			foreach (ProjectItem projectItem in project.Items) {
				
				string projectFileNameExtension = Path.GetExtension(projectItem.FileName);
				
				if (String.Compare(projectFileNameExtension, extension, true) == 0) {
				    	matchedProjectItem = projectItem;
				    	break;
				}
			}
			
			return matchedProjectItem;
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
				WorkbenchSingleton.SafeThreadCall(typeof(TaskService), "Add", new object[] {task});
			}
			
			// Bring task list to front.
			if (tasks.Count > 0) {				
				//if ((bool)PropertyService.Get("SharpDevelop.ShowTaskListAfterBuild", true)) {
					IWorkbench workbench = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench;
					PadDescriptor padDescriptor = workbench.GetPad(typeof(ErrorList));		
					if (padDescriptor != null) {
						WorkbenchSingleton.SafeThreadAsyncCall(padDescriptor, "BringPadToFront");
					}
				//}
			}			
        }
        
        void OutputLineReceived(object sender, LineReceivedEventArgs e)
        {
        	CategoryWriteLine(e.Line);
        }
	}
}
