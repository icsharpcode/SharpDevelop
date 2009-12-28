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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Mock IDesignerGenerator class.
	/// </summary>
	public class MockDesignerGenerator : IRubyDesignerGenerator
	{
		FormsDesignerViewContent viewContent;
		IDesignerHost mergeChangesHost;
		IDesignerSerializationManager mergeChangesSerializationManager;
		
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
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			designerCodeFile = this.viewContent.DesignerCodeFile;
			return new [] {designerCodeFile};
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		public void MergeRootComponentChanges(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			mergeChangesHost = host;
			mergeChangesSerializationManager = serializationManager;
		}
		
		public IDesignerHost MergeChangesHost { 
			get { return mergeChangesHost; } 
		}
		
		public IDesignerSerializationManager MergeChangesSerializationManager {
			get { return mergeChangesSerializationManager; }
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
