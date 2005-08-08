// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	public class DebuggerDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new DebuggerDescriptor(codon);
		}
	}
	
	public class DebuggerDescriptor
	{
		Codon codon;
		
		public DebuggerDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
		IDebugger debugger;
		
		public IDebugger Debugger {
			get {
				if (debugger == null)
					debugger = (IDebugger)codon.AddIn.CreateObject(codon.Properties["class"]);
				return debugger;
			}
		}
		
		public bool SupportsStart {
			get {
				return codon.Properties["supportsStart"] != "false";
			}
		}
		
		public bool SupportsStartWithoutDebugging {
			get {
				return codon.Properties["supportsStartWithoutDebugger"] != "false";
			}
		}
		
		public bool SupportsStop {
			get {
				return codon.Properties["supportsStop"] != "false";
			}
		}
		
		public bool SupportsStepping {
			get {
				return codon.Properties["supportsStepping"] == "true";
			}
		}
		
		public bool SupportsExecutionControl {
			get {
				return codon.Properties["supportsExecutionControl"] == "true";
			}
		}
	}
}
