// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Collections;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of IWorkflowDesignerGenerator.
	/// </summary>
	public interface IWorkflowDesignerGeneratorService
	{
		string CodeFileName {	get;}
		ICollection GetCompatibleMethods(EventDescriptor edesc);
		bool ShowCode();
		bool ShowCode(int lineNumber);
		bool ShowCode(IComponent component, EventDescriptor e);
		bool ShowCode(IComponent component, EventDescriptor e, string methodName);
		bool UseMethod(IComponent component, EventDescriptor e, string methodName);
		void UpdateCCU();
	}
}
