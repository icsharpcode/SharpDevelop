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
	/// <see cref="NCoverRunner.NCoverExit"/> event.
	/// </summary>
	public delegate void NCoverExitEventHandler(object sender, NCoverExitEventArgs e);
	
	/// <summary>
	/// The <see cref="NCoverRunner.NCoverExit"/> event arguments.
	/// </summary>
	public class NCoverExitEventArgs : EventArgs
	{
		string output;
		int exitCode;
		string error;
		
		public NCoverExitEventArgs(string output, string error, int exitCode)
		{
			this.output = output;
			this.error = error;
			this.exitCode = exitCode;
		}
		
		/// <summary>
		/// Gets the command line output from NAnt.
		/// </summary>
		public string Output {
			get {
				return output;
			}
		}
		
		public string Error {
			get {
				return error;
			}
		}
		
		/// <summary>
		/// Gets the exit code.
		/// </summary>
		public int ExitCode {
			get {
				return exitCode;
			}
		}
	}
}
