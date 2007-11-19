// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>


using System;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Represents the method that will handle the 
	/// <see cref="PartCoverRunner.Exited"/> event.
	/// </summary>
	public delegate void PartCoverExitEventHandler(object sender, PartCoverExitEventArgs e);
	
	/// <summary>
	/// The <see cref="PartCoverRunner.Exited"/> event arguments.
	/// </summary>
	public class PartCoverExitEventArgs : EventArgs
	{
		string output;
		int exitCode;
		string error;
		
		public PartCoverExitEventArgs(string output, string error, int exitCode)
		{
			this.output = output;
			this.error = error;
			this.exitCode = exitCode;
		}
		
		/// <summary>
		/// Gets the command line output from PartCover.
		/// </summary>
		public string Output {
			get { return output; }
		}
	
		/// <summary>
		/// Gets the standard error output from PartCover.
		/// </summary>
		public string Error {
			get { return error; }
		}
		
		/// <summary>
		/// Gets the exit code.
		/// </summary>
		public int ExitCode {
			get { return exitCode; }
		}
	}
}
