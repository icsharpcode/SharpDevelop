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
using ICSharpCode.FormsDesigner;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock IDesignerGenerator class.
	/// </summary>
	public class MockDesignerGenerator : IDesignerGenerator
	{
		CodeCompileUnit codeCompileUnit;
		FormsDesignerViewContent viewContent;
		
		public MockDesignerGenerator()
		{
		}
		
		/// <summary>
		/// Gets the code compile unit passed to the MergeFormChanges
		/// method.
		/// </summary>
		public CodeCompileUnit CodeCompileUnitMerged {
			get { return codeCompileUnit; }
		}
		
		public CodeDomProvider CodeDomProvider {
			get {
//				return new IronPython.CodeDom.PythonProvider();
				return null;
			}
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
		
		public ICSharpCode.SharpDevelop.OpenedFile DetermineDesignerCodeFile()
		{
			return this.viewContent.DesignerCodeFile;
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
			codeCompileUnit = unit;
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
