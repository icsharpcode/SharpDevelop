// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Description of PartCoverRunner.
	/// </summary>
	public class PartCoverRunner
	{
		ProcessRunner runner;
		string partCoverFileName = String.Empty;
		string workingDirectory = String.Empty;
		string target = String.Empty;
		string targetWorkingDirectory = String.Empty;
		string targetArguments = String.Empty;
		StringCollection include = new StringCollection();
		StringCollection exclude = new StringCollection();
		string output = String.Empty;
				
		/// <summary>
		/// Triggered when PartCover exits.
		/// </summary>
		public event PartCoverExitEventHandler Exited;
		
		/// <summary>
		/// The PartCover runner was started.
		/// </summary>
		public event EventHandler Started;
		
		/// <summary>
		/// The PartCover runner was stopped.  Being stopped is not the 
		/// same as PartCover exiting.
		/// </summary>
		public event EventHandler Stopped;
		
		/// <summary>
		/// Triggered when an output line is received from PartCover.
		/// </summary>
		public event LineReceivedEventHandler OutputLineReceived;
		
		public PartCoverRunner()
		{
		}

		/// <summary>
		/// Gets or sets the full path to the PartCover 
		/// executable.
		/// </summary>
		public string PartCoverFileName {
			get { return partCoverFileName; }
			set { partCoverFileName = value; }
		}

		/// <summary>
		/// Gets or sets the working directory to use when running
		/// PartCover.
		/// </summary>
		public string WorkingDirectory {
			get { return workingDirectory; }
			set { workingDirectory = value; }
		}

		/// <summary>
		/// Gets or sets the filename of the executable to profile with PartCover.
		/// </summary>
		public string Target {
			get { return target; }
			set { target = value; }
		}

		/// <summary>
		/// Gets or sets the working directory for the target executable.
		/// </summary>
		public string TargetWorkingDirectory {
			get { return targetWorkingDirectory; }
			set { targetWorkingDirectory = value; }
		}

		/// <summary>
		/// Gets or sets the arguments to pass to the target executable.
		/// </summary>
		public string TargetArguments {
			get { return targetArguments; }
			set { targetArguments = value; }
		}

		/// <summary>
		/// Gets or sets the regular expressions which specify the items to 
		/// include in the report whilst profiling the target executable.
		/// </summary>
		public StringCollection Include {
			get { return include; }
		}

		/// <summary>
		/// Gets or sets the regular expressions which specify the items to 
		/// exclude in the report whilst profiling the target executable.
		/// </summary>
		public StringCollection Exclude {
			get { return exclude; }
		}
		
		/// <summary>
		/// Gets or sets the filename for the code coverage results.
		/// </summary>
		public string Output {
			get { return output; }
			set { output = value; }
		}		
		
		/// <summary>
		/// Returns the full path used to run PartCover.
		/// Includes the path to the PartCover executable
		/// and the command line arguments.
		/// </summary>
		public string CommandLine {
			get {
				string arguments = GetArguments();
				if (arguments.Length > 0) {
					return String.Concat(partCoverFileName, " ", arguments);
				}
				return partCoverFileName;
			}
		}
		
		/// <summary>
		/// Returns the command line arguments used to run PartCover.
		/// </summary>
		/// <remarks>
		/// Note that the target arguments may itself contain double quotes
		/// so in order for this to be passed to PartCover as a single argument
		/// we need to prefix each double quote by a backslash. For example:
		/// 
		/// Target args: "C:\Projects\My Tests\Test.dll" /output "C:\Projects\My Tests\Output.xml" 
		/// 
		/// PartCover: --target-args "\"C:\Projects\My Tests\Test.dll\" /output \"C:\Projects\My Tests\Output.xml\"" 
		/// </remarks>
		public string GetArguments()
		{
			StringBuilder arguments = new StringBuilder();
			
			if (!String.IsNullOrEmpty(target)) {
				arguments.AppendFormat("--target \"{0}\" ", target);
			}
			if (!String.IsNullOrEmpty(targetWorkingDirectory)) {
				arguments.AppendFormat("--target-work-dir \"{0}\" ", targetWorkingDirectory);
			}
			if (!String.IsNullOrEmpty(targetArguments)) {
				arguments.AppendFormat("--target-args \"{0}\" ", targetArguments.Replace("\"", "\\\""));
			}
			if (!String.IsNullOrEmpty(output)) {
				arguments.AppendFormat("--output \"{0}\" ", output);
			}
			
			arguments.Append(GetArguments("--include", include));
			
			if (include.Count > 0) {
				// Add a space between include and exclude arguments.
				arguments.Append(' ');
			}
			
			arguments.Append(GetArguments("--exclude", exclude));
						
			return arguments.ToString().Trim();
		}		
		
		public void Start()
		{			
			string arguments = GetArguments();
			
			runner = new ProcessRunner();
			runner.WorkingDirectory = workingDirectory;
			runner.ProcessExited += ProcessExited;
			
			if (OutputLineReceived != null) {
				runner.OutputLineReceived += OnOutputLineReceived;
				runner.ErrorLineReceived += OnOutputLineReceived;
			}
			runner.Start(partCoverFileName, arguments);	
			OnStarted();
		}
		
		/// <summary>
		/// Stops the currently running PartCover instance.
		/// </summary>
		public void Stop()
		{
			if (runner != null) {
				runner.Kill();
				OnStopped();
			}
		}

		protected void OnExited(string output, string error, int exitCode)
		{
			if (Exited != null) {
				Exited(this, new PartCoverExitEventArgs(output, error, exitCode));
			}
		}
		
		protected void OnStarted()
		{
			if (Started != null) {
				Started(this, new EventArgs());
			}
		}
		
		protected void OnStopped()
		{
			if (Stopped != null) {
				Stopped(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Raises the <see cref="OutputLineReceived"/> event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		protected void OnOutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			if (OutputLineReceived != null) {
				OutputLineReceived(this, e);
			}
		}
		
		/// <summary>
		/// Handles the PartCover process exit event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void ProcessExited(object sender, EventArgs e)
		{
			ProcessRunner runner = (ProcessRunner)sender;		
			OnExited(runner.StandardOutput, runner.StandardError, runner.ExitCode);
		}		
	
		/// <summary>
		/// Gets the command line option that can have multiple items as specified
		/// in the string array. Each array item will have a separate command line
		/// argument (e.g. --include=A --include=B --include=B).
		/// </summary>
		static string GetArguments(string argumentName, StringCollection items)
		{
			StringBuilder arguments = new StringBuilder();
			foreach (string item in items) {
				arguments.Append(argumentName);
				arguments.Append(" ");
				arguments.Append(item);
				arguments.Append(" ");
			}
			return arguments.ToString().Trim();
		}
	}
}
