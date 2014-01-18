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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NAnt.Commands;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NAnt
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
		/// Gets whether the NAnt runner is currently running.
		/// </summary>
		public bool IsRunning {
			get {
				bool isRunning = false;
				
				if (runner != null) {
					isRunning = !runner.HasExited;
				}
				
				return isRunning;
			}
		}
		
		public void Start()
		{
			RunNAnt().FireAndForget();
		}
		
		async Task<int> RunNAnt()
		{
			string[] arguments = GetArguments();
			
			runner = new ProcessRunner();
			runner.WorkingDirectory = workingDirectory;
			
			runner.RedirectStandardOutputAndErrorToSingleStream = true;
			runner.Start(nantFileName, arguments);
			
			AbstractRunNAntCommand.Category.AppendLine(runner.CommandLine);
			
			var writer = new NAntMessageViewCategoryTextWriter(AbstractRunNAntCommand.Category);
			using (TextReader reader = runner.OpenStandardOutputReader()) {
				await reader.CopyToAsync(writer);
			}
			
			OnNAntStarted();
			
			await runner.WaitForExitAsync();
			AbstractRunNAntCommand.Category.AppendLine(StringParser.Parse("${res:XML.MainMenu.ToolMenu.ExternalTools.ExitedWithCode} " + runner.ExitCode));
			
			OnNAntExited(writer.Output, String.Empty, runner.ExitCode);
			
			return runner.ExitCode;
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
		/// Adds extra command line arguments to those specified
		/// by the user in the <see cref="Arguments"/> string.
		/// </summary>
		/// <returns></returns>
		string[] GetArguments()
		{
			var nantArguments = new List<string>();

			if (!showLogo) {
				nantArguments.Add("-nologo");
			}
			
			if (verbose) {
				nantArguments.Add("-v");
			}
			
			if (quiet) {
				nantArguments.Add("-q");
			}
			
			if (debugMode) {
				nantArguments.Add("-debug");
			}
			
			if (buildFileName.Length > 0) {
				nantArguments.Add(String.Format("-buildfile:{0}", buildFileName));
			}
			
			string[] extraArguments = this.arguments.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			if (extraArguments.Length > 0) {
				nantArguments.AddRange(extraArguments);
			}
			
			return nantArguments.ToArray();
		}
	}
}
