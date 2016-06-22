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

using ICSharpCode.Core;
using System;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WorkflowDesigner
{
	/// <summary>
	/// Display binding that attaches the workflow designer to .xaml files.
	/// </summary>
	public class WorkflowDisplayBinding : ISecondaryDisplayBinding
	{
		public bool CanAttachTo(IViewContent content)
		{
			ITextEditorProvider p = content as ITextEditorProvider;
			if (p != null) {
				try {
					using (XmlTextReader r = new XmlTextReader(p.TextEditor.Document.CreateReader())) {
						r.XmlResolver = null;
						// find the opening of the root element:
						while (r.Read() && r.NodeType != XmlNodeType.Element);
						// attach if this is a workflow node
						return r.NamespaceURI == "http://schemas.microsoft.com/netfx/2009/xaml/activities";
					}
				} catch (XmlException e) {
					LoggingService.Debug("WorkflowDisplayBinding got exception: " + e.Message);
					return false;
				}
			} else {
				return false;
			}
		}
		
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get { return false; }
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new WorkflowDesignerViewContent(viewContent.PrimaryFile) };
		}
	}
}
