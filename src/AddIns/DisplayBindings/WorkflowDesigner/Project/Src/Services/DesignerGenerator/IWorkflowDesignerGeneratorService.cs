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
		string CodeSeparationFileName {	get;}
		ICollection GetCompatibleMethods(EventDescriptor edesc);
		bool ShowCode();
		bool ShowCode(int lineNumber);
		bool ShowCode(System.ComponentModel.IComponent component, System.ComponentModel.EventDescriptor e, string methodName);
		void UseMethod(System.ComponentModel.IComponent component, System.ComponentModel.EventDescriptor e, string methodName);
	}
}
