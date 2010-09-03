// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
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
				if (WorkbenchSingleton.Workbench == null) {
					return false;
				}
				ITextEditorProvider view = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (view != null) {
					return IsFileNameHandled(view.TextEditor.FileName);
				}
				return false;
			}
		}
	}
}
