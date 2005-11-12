// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	[Serializable]
	public class ProcessEventArgs: DebuggerEventArgs
	{
		Process process;

		public Process Process {
			get {
				return process;
			}
		}

		public ProcessEventArgs(Process process): base(process.Debugger)
		{
			this.process = process;
		}
	}
}
