//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;

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
