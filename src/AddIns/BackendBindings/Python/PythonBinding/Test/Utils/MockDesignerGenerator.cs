// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock IDesignerGenerator class.
	/// </summary>
	public class MockDesignerGenerator : IPythonDesignerGenerator
	{
		FormsDesignerViewContent viewContent;
		IComponent mergeChangesRootComponent;
		
		public MockDesignerGenerator()
		{
		}
		
		public CodeDomProvider CodeDomProvider {
			get { return null; }
		}
		
		public FormsDesignerViewContent ViewContent {
			get { return this.viewContent; }
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.OpenedFile> GetSourceFiles(out ICSharpCode.SharpDevelop.OpenedFile designerCodeFile)
		{
			designerCodeFile = this.viewContent.DesignerCodeFile;
			return new [] {designerCodeFile};
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		public void MergeRootComponentChanges(IComponent component)
		{
			mergeChangesRootComponent = component;
		}
		
		public IComponent MergeChangesRootComponent { 
			get { return mergeChangesRootComponent; } 
		}
		
		public bool InsertComponentEvent(IComponent component, System.ComponentModel.EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			throw new NotImplementedException();
		}
		
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			throw new NotImplementedException();
		}
	}
}
