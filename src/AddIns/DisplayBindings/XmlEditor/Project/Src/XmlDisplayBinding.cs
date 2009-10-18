// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			get {
				return false;
			}
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			return content.PrimaryFileName != null && IsFileNameHandled(content.PrimaryFileName);
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
			if (string.IsNullOrEmpty(extension))
			    return false;
			
			foreach (string currentExtension in GetXmlFileExtensions()) {
				if (string.Compare(extension, currentExtension, StringComparison.OrdinalIgnoreCase) == 0) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Returns the extensions defined in the @extensions property of the first codon in the tree node.
		/// </summary>
		public static string[] GetXmlFileExtensions(AddInTreeNode node)
		{
			if (node != null) {			
				foreach (Codon codon in node.Codons) {
					if (codon.Id == "Xml") {
						List<string> extensions = new List<string>();
						foreach (string ext in codon.Properties["extensions"].Split(';')) {
							extensions.Add(ext.Trim());
						}
						return extensions.ToArray();								
					}
				}
			}
			return new string[0];
		}
		
		/// <summary>
		/// Gets the known xml file extensions.
		/// </summary>
		public static string[] GetXmlFileExtensions()
		{
			AddInTreeNode node = AddInTree.GetTreeNode("/AddIns/DefaultTextEditor/CodeCompletion", false);
			return GetXmlFileExtensions(node);
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return (new List<IViewContent>() {
			        	new XmlTreeView(viewContent)
			        }).ToArray();
		}
		
		public static bool XmlViewContentActive {
			get {
				if (WorkbenchSingleton.Workbench == null)
					return false;
				ITextEditorProvider view = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (view != null)
					return IsFileNameHandled(view.TextEditor.FileName);
				
				return false;
			}
		}
	}
}
