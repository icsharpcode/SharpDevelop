// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Runs the Python console passing the filename of the 
	/// active python script open in SharpDevelop.
	/// </summary>
	public class RunPythonCommand : AbstractMenuCommand
	{
		IProcessRunner processRunner;
		AddInOptions options;
		IWorkbench workbench;
		MessageViewCategory category;
		IPadDescriptor outputWindowPad;
		static MessageViewCategory categorySingleton;
		static RunPythonCommand runningCommand;
		
		public RunPythonCommand()
			: this(WorkbenchSingleton.Workbench, new AddInOptions(), new PythonProcessRunner(), PythonMessageViewCategory, new PythonOutputWindowPadDescriptor())
		{
		}
		
		public RunPythonCommand(IWorkbench workbench, AddInOptions options, IProcessRunner processRunner, MessageViewCategory category, IPadDescriptor outputWindowPad)
		{
			this.processRunner = processRunner;
			this.options = options;
			this.workbench = workbench;
			this.category = category;
			this.outputWindowPad = outputWindowPad;
			
			processRunner.OutputLineReceived += OutputLineReceived;
			processRunner.ProcessExited += ProcessExited;
		}
		
		/// <summary>
		/// Indicates whether the command is still running.
		/// </summary>
		public static bool IsRunning {
			get {
				return runningCommand != null;
			}
		}
		
		/// <summary>
		/// Gets the command that is currently running.
		/// </summary>
		public static RunPythonCommand RunningCommand {
			get {
				return runningCommand;
			}
		}
		
		/// <summary>
		/// Runs the python console passing the filename to the currently
		/// active python script.
		/// </summary>
		public override void Run()
		{
			// Get the python script filename.
			string fileName = workbench.ActiveWorkbenchWindow.ActiveViewContent.PrimaryFileName;
			
			// Clear the output window.
			outputWindowPad.BringPadToFront();
			category.ClearText();
			
			// Start the python console app passing the python script filename.
			CategoryWriteLine("Running Python...");
			string args = String.Concat('\"', fileName, '\"');
			CategoryWriteLine(String.Concat(options.PythonFileName, " ", args));
			processRunner.Start(options.PythonFileName, args);
			
			runningCommand = this;
		}
		
		/// <summary>
		/// Stops the python console.
		/// </summary>
		public void Stop()
		{
			if (runningCommand != null) {
				runningCommand = null;
				processRunner.Kill();
			}
		}
		
		/// <summary>
		/// Creates the single instance of the 
		/// </summary>
		static MessageViewCategory PythonMessageViewCategory {
			get {
				if (categorySingleton == null) {
					MessageViewCategory.Create(ref categorySingleton, "Python");
				}
				return categorySingleton;
			}
		}
		
		/// <summary>
		/// Handler for the ProcessRunner's output line received event.
		/// </summary>
		void OutputLineReceived(object source, LineReceivedEventArgs e)
		{
			CategoryWriteLine(e.Line);
		}
		
		/// <summary>
		/// Handler for the process exit event.
		/// </summary>
		void ProcessExited(object source, EventArgs e)
		{
			runningCommand = null;
		}
		
		/// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        void CategoryWriteLine(string message)
        {
        	category.AppendText(String.Concat(message, Environment.NewLine));
        }
	}
}
