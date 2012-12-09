// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using System.Xml;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfPrimaryDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new WpfViewContent(file);
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
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
				IEditable editable = content as IEditable;
				if (editable != null) {
					try {
						XmlTextReader r = new XmlTextReader(new StringReader(editable.Text));
						r.XmlResolver = null;
						r.WhitespaceHandling = WhitespaceHandling.None;
						while (r.NodeType != XmlNodeType.Element && r.Read());
						if (r.LocalName == "ResourceDictionary" || r.LocalName == "Activity")
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
