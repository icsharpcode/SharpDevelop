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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Xml;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfPrimaryDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new WpfViewContent(file);
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			throw new NotImplementedException();
		}
	}
	
	public class WpfSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		public bool ReattachWhenParserServiceIsReady {
			get {
				return false;
			}
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			if (Path.GetExtension(content.PrimaryFileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase)) {
				IEditable editable = content.GetService<IEditable>();
				if (editable != null) {
					try {
						XmlTextReader r = new XmlTextReader(editable.CreateSnapshot().CreateReader());
						r.XmlResolver = null;
						r.WhitespaceHandling = WhitespaceHandling.None;
						while (r.NodeType != XmlNodeType.Element && r.Read());
						if (r.LocalName == "ResourceDictionary" || r.LocalName == "Application" || r.LocalName == "Activity")
							return false;
					} catch (XmlException) {
						return true;
					}
					return true;
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new WpfViewContent(viewContent.PrimaryFile) };
		}
	}
}
