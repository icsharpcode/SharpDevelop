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
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Display binding for the xml editor.
	/// </summary>
	public class XmlDisplayBinding : ISecondaryDisplayBinding
	{
		public bool ReattachWhenParserServiceIsReady {
			get { return false; }
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			return (content.PrimaryFileName != null) && IsFileNameHandled(content.PrimaryFileName);
		}
		
		/// <summary>
		/// Returns whether the view can handle the specified file.
		/// </summary>
		public static bool IsFileNameHandled(string fileName)
		{
			return IsXmlFileExtension(Path.GetExtension(fileName));
		}
		
		/// <summary>
		/// Checks that the file extension refers to an xml file as
		/// specified in the SyntaxModes.xml file.
		/// </summary>
		static bool IsXmlFileExtension(string extension)
		{
			if (String.IsNullOrEmpty(extension)) {
			    return false;
			}
			
			DefaultXmlFileExtensions fileExtensions = new DefaultXmlFileExtensions();
			foreach (string currentExtension in fileExtensions) {
				if (String.Compare(extension, currentExtension, StringComparison.OrdinalIgnoreCase) == 0) {
					return true;
				}
			}
			return false;
		}		
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			List<IViewContent> views = new List<IViewContent>();
			XmlSchemaCompletion defaultSchema = XmlEditorService.XmlSchemaFileAssociations.GetSchemaCompletion(viewContent.PrimaryFileName);
			views.Add(new XmlTreeView(viewContent, XmlEditorService.RegisteredXmlSchemas.Schemas, defaultSchema));
			return views.ToArray();
		}
		
		public static bool XmlViewContentActive {
			get {
				ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
				if (editor != null) {
					return IsFileNameHandled(editor.FileName);
				}
				return false;
			}
		}
	}
}
