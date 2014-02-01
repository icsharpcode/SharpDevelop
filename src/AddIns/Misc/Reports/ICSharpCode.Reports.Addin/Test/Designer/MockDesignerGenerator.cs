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

using System;
using ICSharpCode.Reports.Addin;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.Test.Designer
{
	
	public class MockDesignerGenerator:IDesignerGenerator
	{
		ReportDesignerView view;
		
		public MockDesignerGenerator()
		{
			
		}
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ReportDesignerView ViewContent {
			get {
				return this.view;
			}
		}
		
		public void Attach(ReportDesignerView viewContent)
		{
			this.view = viewContent;
		}
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			throw new NotImplementedException();
		}
		
		public void MergeFormChanges(System.CodeDom.CodeCompileUnit unit)
		{
			throw new NotImplementedException();
		}
		
		public bool InsertComponentEvent(System.ComponentModel.IComponent component, System.ComponentModel.EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
		
	}
}
