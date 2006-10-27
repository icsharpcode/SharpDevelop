// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Display binding for the xml editor.  
	/// </summary>
	public class XmlDisplayBinding : IDisplayBinding
	{
		public XmlDisplayBinding()
		{
		}		
			
		#region IDisplayBinding
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			XmlView view = new XmlView();
			view.LoadContent(content);
			return view;
		}
		
		/// <summary>
		/// Can create content for the 'XML' language.
		/// </summary>
		public bool CanCreateContentForLanguage(string languageName)
		{
			return XmlView.IsLanguageHandled(languageName);
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			XmlView view = new XmlView();
			view.Load(fileName);
			return view;
		}
		
		/// <summary>
		/// Can only create content for file with extensions that are 
		/// known to be xml files as specified in the SyntaxModes.xml file.
		/// </summary>
		public bool CanCreateContentForFile(string fileName)
		{
			return XmlView.IsFileNameHandled(fileName);
		}
		
		#endregion
	}
}
