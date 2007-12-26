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

namespace ICSharpCode.NAnt
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
