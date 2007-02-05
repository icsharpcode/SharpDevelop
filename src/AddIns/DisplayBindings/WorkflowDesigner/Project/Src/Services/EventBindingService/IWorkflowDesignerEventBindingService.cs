// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of IWorkflowDesignerGenerator.
	/// </summary>
	public interface IWorkflowDesignerEventBindingService : IEventBindingService
	{
		string CodeFileName {	get;}
		void UpdateCodeCompileUnit();
	}
}
