// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Display binding for the xml editor.
	/// </summary>
	public class XmlDisplayBinding : IDisplayBinding
	{
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new XmlView(file);
		}
		
		/// <summary>
		/// Can only create content for file with extensions that are
		/// known to be xml files as specified in the SyntaxModes.xml file.
		/// </summary>
		public bool CanCreateContentForFile(string fileName)
		{
			return XmlView.IsFileNameHandled(fileName);
		}
	}
}
