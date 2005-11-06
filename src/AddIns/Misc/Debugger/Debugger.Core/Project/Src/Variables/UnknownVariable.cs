// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public class UnknownVariable: Variable
	{
		public override object Value {
			get {
				return "<unknown>"; 
			} 
		}
		
		public override string Type { 
			get {
				return "<unknown>"; 
			} 
		}

		internal unsafe UnknownVariable(NDebugger debugger, ICorDebugValue corValue, string name):base(debugger, corValue, name)
		{
			
		}

		public override bool MayHaveSubVariables {
			get {
				return false;
			}
		}
	}
}
