// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a root component in the designer.
	/// </summary>
	public class PythonDesignerRootComponent : PythonDesignerComponent
	{
		public PythonDesignerRootComponent(IComponent component)
			: base(component)
		{
		}
		
		public override string GetPropertyOwnerName()
		{
			return "self";
		}
		
		public override void AppendSuspendLayout(PythonCodeBuilder codeBuilder)
		{
			AppendMethodCalls(codeBuilder, suspendLayoutMethods);
		}
		
		public override void AppendResumeLayout(PythonCodeBuilder codeBuilder)
		{
			AppendMethodCalls(codeBuilder, resumeLayoutMethods);
		}		
		
		public override void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			// Add the child components first.
			foreach (PythonDesignerComponent component in GetChildComponents()) {
				component.AppendComponent(codeBuilder);
			}
			
			// Add root component
			AppendComponentProperties(codeBuilder, false, false, true);
		}
		
		/// <summary>
		/// Gets the child components in reverse order since the forms designer has them reversed.
		/// </summary>
		public override PythonDesignerComponent[] GetChildComponents()
		{
			PythonDesignerComponent[] components = base.GetChildComponents();
			Array.Reverse(components);
			return components;
		}		
	}
}
