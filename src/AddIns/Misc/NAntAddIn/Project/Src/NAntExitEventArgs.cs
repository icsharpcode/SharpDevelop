// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// Represents the method that will handle the 
	/// <see cref="NAntRunner.NAntExit"/> event.
	/// </summary>
	public delegate void NAntExitEventHandler(object sender, NAntExitEventArgs e);
	
	/// <summary>
	/// The <see cref="NAntRunner.NAntExit"/> event arguments.
	/// </summary>
	public class NAntExitEventArgs : EventArgs
	{
		string output;
		int exitCode;
		string error;
		
		public NAntExitEventArgs(string output, string error, int exitCode)
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
		/// Gets the NAnt exit code.
		/// </summary>
		public int ExitCode {
			get {
				return exitCode;
			}
		}
		
	}
}
