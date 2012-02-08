// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.UnitTesting
{
	public interface ITestTreeView
	{
		/// <summary>
		/// Gets the selected member in the test tree view.
		/// </summary>
		IMember SelectedMember {get;}
		
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
