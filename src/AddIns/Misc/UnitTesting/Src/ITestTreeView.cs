// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.UnitTesting
{
	public interface ITestTreeView
	{
		/// <summary>
		/// Gets the selected method in the test tree view.
		/// </summary>
		IMember SelectedMethod {get;}
		
		/// <summary>
		/// Gets the selected class in the test tree view.
		/// </summary>
		IClass SelectedClass {get;}
		
		/// <summary>
		/// Gets the selected project for the selected node
		/// in the test tree view.
		/// </summary>
		IProject SelectedProject {get;}
		
		/// <summary>
		/// Gets the namespace for the selected namespace node.
		/// </summary>
		string SelectedNamespace {get;}
	}
}
