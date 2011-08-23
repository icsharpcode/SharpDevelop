// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Creates debuggers.
	/// </summary>
	/// <attribute name="class" use="required">
	/// Name of the IDebugger class.
	/// </attribute>
	/// <attribute name="supportsStart" use="optional">
	/// Specifies if the debugger supports the 'Start' command. Default: true
	/// </attribute>
	/// <attribute name="supportsStartWithoutDebugger" use="optional">
	/// Specifies if the debugger supports the 'StartWithoutDebugger' command. Default: true
	/// </attribute>
	/// <attribute name="supportsStop" use="optional">
	/// Specifies if the debugger supports the 'Stop' (kill running process) command. Default: true
	/// </attribute>
	/// <attribute name="supportsStepping" use="optional">
	/// Specifies if the debugger supports stepping. Default: false
	/// </attribute>
	/// <attribute name="supportsExecutionControl" use="optional">
	/// Specifies if the debugger supports execution control (break, resume). Default: false
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Services/DebuggerService/Debugger</usage>
	/// <returns>
	/// An DebuggerDescriptor object that exposes the attributes and the IDebugger object (lazy-loading).
	/// </returns>
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
		
		public object BuildItem(BuildItemArgs args)
		{
			return new DebuggerDescriptor(args.Codon);
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
		
		public bool SupportsAttaching {
			get {
				return codon.Properties["supportsAttaching"] == "true";
			}
		}

		public bool SupportsDetaching {
			get {
				return codon.Properties["supportsDetaching"] == "true";
			}
		}		
	}
}
