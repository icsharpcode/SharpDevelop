// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// Runs NAnt.
	/// </summary>
	public class NAntRunner
	{
		string arguments = String.Empty;
		string buildFileName = String.Empty;
		string nantFileName = String.Empty;
		string workingDirectory = String.Empty;
		bool verbose;
		bool quiet;
		bool showLogo;
		bool debugMode;
		ProcessRunner runner;
		
		/// <summary>
		/// Triggered when NAnt exits.
		/// </summary>
		public event NAntExitEventHandler NAntExited;
		
		/// <summary>
		/// The NAnt runner was started.
		/// </summary>
		public event EventHandler NAntStarted;
		
		/// <summary>
		/// The NAnt runner was stopped.  Being stopped is not the 
		/// same as NAnt exiting.
		/// </summary>
		public event EventHandler NAntStopped;
		
		/// <summary>
		/// Triggered when an output line is received from NAnt.
		/// </summary>
		public event LineReceivedEventHandler OutputLineReceived;
				
		public NAntRunner()
		{
		}
		
		/// <summary>
		/// Gets or sets the NAnt -buildfile parameter.
		/// </summary>
		public string BuildFileName {
			get {
				return buildFileName;
			}
			
			set {
				buildFileName = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the NAnt executable path.
		/// </summary>
		public string NAntFileName {
			get {
				return nantFileName;
			}
			
			set {
				nantFileName = value;
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
		/// Gets or sets the NAnt -verbose option.
		/// </summary>
		public bool Verbose {
			get {
				return verbose;
			}
			
			set {
				verbose = value;
			}
		}
		
		public string Arguments {
			get {
				return arguments;
			}
			
			set {
				arguments = value;
			}
		}

		/// <summary>
		/// Gets or sets the NAnt -quiet option.
		/// </summary>
		public bool Quiet {
			get {
				return quiet;
			}
			
			set {
				quiet = value;
			}
		}	
		
		/// <summary>
		/// Maps to the NAnt -nologo option.
		/// </summary>
		public bool ShowLogo {
			get {
				return showLogo;
			}
			
			set {
				showLogo = value;
			}
		}	
		
		/// <summary>
		/// Gets or sets the NAnt -debug option.
		/// </summary>
		public bool DebugMode {
			get {
				return debugMode;
			}
			
			set {
				debugMode = value;
			}
		}			
		
		/// <summary>
		/// Gets the full NAnt command line that will be used by
		/// the runner.
		/// </summary>
		public string CommandLine {
			get {
				return String.Concat(nantFileName, " ", GetArguments());
			}
		}
		
		/// <summary>
		/// Gets whether the NAnt runner is currently running.
		/// </summary>
		public bool IsRunning {
			get {
				bool isRunning = false;
				
				if (runner != null) {
					isRunning = runner.IsRunning;
				}
				
				return isRunning;
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
			runner.Start(nantFileName, arguments);	
			OnNAntStarted();
		}
		
		/// <summary>
		/// Stops the currently running NAnt instance.
		/// </summary>
		public void Stop()
		{
			if (runner != null) {
				runner.Kill();
				OnNAntStopped();
			}
		}

		protected void OnNAntExited(string output, string error, int exitCode)
		{
			if (NAntExited != null) {
				NAntExited(this, new NAntExitEventArgs(output, error, exitCode));
			}
		}
		
		protected void OnNAntStarted()
		{
			if (NAntStarted != null) {
				NAntStarted(this, new EventArgs());
			}
		}
		
		protected void OnNAntStopped()
		{
			if (NAntStopped != null) {
				NAntStopped(this, new EventArgs());
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
		/// Handles the NAnt process exit event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void ProcessExited(object sender, EventArgs e)
		{
			ProcessRunner runner = (ProcessRunner)sender;		
			OnNAntExited(runner.StandardOutput, runner.StandardError, runner.ExitCode);
		}
		
		/// <summary>
		/// Adds extra command line arguments to those specified
		/// by the user in the <see cref="Arguments"/> string.
		/// </summary>
		/// <returns></returns>
		string GetArguments()
		{
			StringBuilder nantArguments = new StringBuilder();

			if (!showLogo) {
				nantArguments.Append("-nologo ");
			}
			
			if (verbose) {
				nantArguments.Append("-v ");
			}
			
			if (quiet) {
				nantArguments.Append("-q ");
			}
			
			if (debugMode) {
				nantArguments.Append("-debug ");
			}
			
			if (buildFileName.Length > 0) {
				nantArguments.Append("-buildfile:");
				nantArguments.Append(buildFileName);
				nantArguments.Append(" ");
			}
			
			nantArguments.Append(this.arguments);
			
			return nantArguments.ToString();
		}
	}
}
