// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using System;

namespace UnitTesting.Tests
{
	public class MockTestTreeView : ITestTreeView
	{
		IMember selectedMethod;
		IClass selectedClass;
		IProject selectedProject;
		string selectedNamespace;
		
		public MockTestTreeView()
		{
		}
		
		public IMember SelectedMethod {
			get {
				return selectedMethod;
			}
			set {
				selectedMethod = value;
			}
		}
		
		public IClass SelectedClass {
			get {
				return selectedClass;
			}
			set {
				selectedClass = value;
			}
		}
		
		public IProject SelectedProject {
			get {
				return selectedProject;
			}
			set {
				selectedProject = value;
			}
		}
		
		public string SelectedNamespace {
			get {
				return selectedNamespace;
			}
			set {
				selectedNamespace = value;
			}
		}
	}
}
