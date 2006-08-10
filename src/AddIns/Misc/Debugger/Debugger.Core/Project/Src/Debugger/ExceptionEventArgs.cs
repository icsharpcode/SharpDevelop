// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class ExceptionEventArgs: DebuggerEventArgs
	{
		bool @continue;
		Exception exception;
		
		public bool Continue {
			get {
				return @continue;
			}
			set {
				@continue = value;
			}
		}
		
		public Exception Exception {
			get {
				return exception;
			}
		}
		
		public ExceptionEventArgs(Process process, Exception exception):base(process)
		{
			this.exception = exception;
		}
	}
}
