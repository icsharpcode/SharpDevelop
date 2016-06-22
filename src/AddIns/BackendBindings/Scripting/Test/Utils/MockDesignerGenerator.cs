// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using System.CodeDom;
//using System.CodeDom.Compiler;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.ComponentModel.Design.Serialization;
//using System.Reflection;
//using System.Windows.Forms;
//
//using ICSharpCode.FormsDesigner;
//using ICSharpCode.SharpDevelop;
//
//namespace ICSharpCode.Scripting.Tests.Utils
//{
//	public class MockDesignerGenerator : IScriptingDesignerGenerator
//	{
//		FormsDesignerViewContent viewContent;
//		IDesignerHost mergeChangesHost;
//		IDesignerSerializationManager mergeChangesSerializationManager;
//		
//		public MockDesignerGenerator()
//		{
//		}
//		
//		public CodeDomProvider CodeDomProvider {
//			get { return null; }
//		}
//		
//		public FormsDesignerViewContent ViewContent {
//			get { return this.viewContent; }
//		}
//		
//		public void Attach(FormsDesignerViewContent viewContent)
//		{
//			this.viewContent = viewContent;
//		}
//		
//		public void Detach()
//		{
//			this.viewContent = null;
//		}
//		
//		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
//		{
//			designerCodeFile = this.viewContent.DesignerCodeFile;
//			return new [] {designerCodeFile};
//		}
//		
//		public void MergeFormChanges(CodeCompileUnit unit)
//		{
//		}
//		
//		public void NotifyComponentRenamed(object component, string newName, string oldName)
//		{
//		}
//		
//		public void MergeRootComponentChanges(IDesignerHost host, IDesignerSerializationManager serializationManager)
//		{
//			mergeChangesHost = host;
//			mergeChangesSerializationManager = serializationManager;
//		}
//		
//		public IDesignerHost MergeChangesHost { 
//			get { return mergeChangesHost; } 
//		}
//		
//		public IDesignerSerializationManager MergeChangesSerializationManager {
//			get { return mergeChangesSerializationManager; }
//		}
//		
//		public bool InsertComponentEvent(IComponent component, System.ComponentModel.EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public ICollection GetCompatibleMethods(EventDescriptor edesc)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public ICollection GetCompatibleMethods(EventInfo edesc)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
