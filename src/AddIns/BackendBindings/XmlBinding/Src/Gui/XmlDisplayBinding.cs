// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlBinding.Gui
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
			foreach (string currentExtension in GetXmlFileExtensions()) {
				if (String.Compare(extension, currentExtension, true) == 0) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets the known xml file extensions.
		/// </summary>
		public static string[] GetXmlFileExtensions()
		{			
			foreach (ParserDescriptor parser in AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false)) {
				if (parser.Codon.Id == "XmlFoldingParser") {
					return parser.Supportedextensions;
				}
			}

//			// Did not find the XmlFoldingParser so default to those files defined by the
//			// HighlightingManager.
//			IHighlightingStrategy strategy = HighlightingManager.Manager.FindHighlighter("XML");
//			if (strategy != null) {
//				return strategy.Extensions;
//			}
			
			return new string[] { ".xml", ".addin" };
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return (new List<IViewContent>() {
			        	new XmlTreeView(viewContent)
			        }).ToArray();
		}
	}
}
