// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockDesignerGenerator : IScriptingDesignerGenerator
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
		
		public void NotifyComponentRenamed(object component, string newName, string oldName)
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
