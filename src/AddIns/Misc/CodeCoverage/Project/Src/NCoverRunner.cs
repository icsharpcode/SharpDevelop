// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Util;
using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Runs NCover.
	/// </summary>
	public class NCoverRunner
	{
		string ncoverFileName = String.Empty;
		string workingDirectory = String.Empty;
		string coverageResultsFileName = String.Empty;
		string profiledApplicationCommand = String.Empty;
		string profiledApplicationCommandLineArguments = String.Empty;
		string assemblyList = String.Empty;
		string logFileName = String.Empty;
		ProcessRunner runner;
		
		/// <summary>
		/// Triggered when NCover exits.
		/// </summary>
		public event NCoverExitEventHandler NCoverExited;
		
		/// <summary>
		/// The NCover runner was started.
		/// </summary>
		public event EventHandler NCoverStarted;
		
		/// <summary>
		/// The NCover runner was stopped.  Being stopped is not the 
		/// same as NCover exiting.
		/// </summary>
		public event EventHandler NCoverStopped;
		
		/// <summary>
		/// Triggered when an output line is received from NCover.
		/// </summary>
		public event LineReceivedEventHandler OutputLineReceived;
				
		public NCoverRunner()
		{
		}
		
		/// <summary>
		/// Gets or sets the NCover executable path.
		/// </summary>
		public string NCoverFileName {
			get {
				return ncoverFileName;
			}
			set {
				ncoverFileName = value;
			}
		}		
		
		public string WorkingDirectory {
			get {
				return workingDirectory;
			}
			set {
				workingDirectory = value;
			}
		}		
		
		/// <summary>
		/// The list of assemblies that will be profiled.
		/// </summary>
		public string AssemblyList {
			get {
				return assemblyList;
			}
			set {
				// Remove any spaces from the assembly list since
				// NCover will ignore any items after the space.
				assemblyList = value.Replace(" ", String.Empty);
			}
		}	
		
		/// <summary>
		/// Gets the full NCover command line that will be used by
		/// the runner.
		/// </summary>
		public string CommandLine {
			get {
				return String.Concat(ncoverFileName, " ", GetArguments());
			}
		}
		
		/// <summary>
		/// Gets whether the NCover runner is currently running.
		/// </summary>
		public bool IsRunning {
			get {
				if (runner != null) {
					return runner.IsRunning;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Coverage output results file.
		/// </summary>
		public string CoverageResultsFileName {
			get {
				return coverageResultsFileName;
			}
			set {
				coverageResultsFileName = value;
			}
		}
		
		/// <summary>
		/// Profiler log file.
		/// </summary>
		public string LogFileName {
			get {
				return logFileName;
			}
			set {
				logFileName = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the command that will be used to run the 
		/// profiled application.
		/// </summary>
		public string ProfiledApplicationCommand {
			get {
				return profiledApplicationCommand;
			}
			set {
				profiledApplicationCommand = value;
			}
		}
		
		/// <summary>
		/// The arguments that will be used with the profiled application.
		/// </summary>
		public string ProfiledApplicationCommandLineArguments {
			get {
				return profiledApplicationCommandLineArguments;
			}
			set {
				profiledApplicationCommandLineArguments = value;
			}
		}
		
		public void Start()
		{			
			string arguments = GetArguments();
			
			runner = new ProcessRunner();
			runner.WorkingDirectory = workingDirectory;
			runner.ProcessExited += new EventHandler(ProcessExited);
			
			if (OutputLineReceived != null) {
				runner.OutputLineReceived += new LineReceivedEventHandler(OnOutputLineReceived);
			}
			runner.Start(ncoverFileName, arguments);	
			OnNCoverStarted();
		}
		
		/// <summary>
		/// Stops the currently running NCover instance.
		/// </summary>
		public void Stop()
		{
			if (runner != null) {
				runner.Kill();
				OnNCoverStopped();
			}
		}

		protected void OnNCoverExited(string output, string error, int exitCode)
		{
			if (NCoverExited != null) {
				NCoverExited(this, new NCoverExitEventArgs(output, error, exitCode));
			}
		}
		
		protected void OnNCoverStarted()
		{
			if (NCoverStarted != null) {
				NCoverStarted(this, new EventArgs());
			}
		}
		
		protected void OnNCoverStopped()
		{
			if (NCoverStopped != null) {
				NCoverStopped(this, new EventArgs());
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
		/// Handles the NCover process exit event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void ProcessExited(object sender, EventArgs e)
		{
			ProcessRunner runner = (ProcessRunner)sender;		
			OnNCoverExited(runner.StandardOutput, runner.StandardError, runner.ExitCode);
		}
		
		/// <summary>
		/// Adds extra command line arguments to those specified
		/// by the user in the <see cref="Arguments"/> string.
		/// </summary>
		string GetArguments()
		{
			StringBuilder ncoverArguments = new StringBuilder();
			
			if (coverageResultsFileName.Length > 0) {
				ncoverArguments.AppendFormat("//x \"{0}\" ", coverageResultsFileName);
			}
			
			if (assemblyList.Length > 0) {
				ncoverArguments.AppendFormat("//a \"{0}\" ", assemblyList);
			}
			
			if (logFileName.Length > 0) {
				ncoverArguments.AppendFormat("//l \"{0}\" ", logFileName);
			}
			
			ncoverArguments.AppendFormat("\"{0}\" ", profiledApplicationCommand);
			
			//ncoverArguments.Append(profiledApplicationCommandLineArguments);
			// HACK: Work around NCover bug: http://ncover.org/SITE/forums/thread/266.aspx
			ncoverArguments.Append(profiledApplicationCommandLineArguments.Replace("\"", "\\\""));
			
			return ncoverArguments.ToString();
		}
	}
}
