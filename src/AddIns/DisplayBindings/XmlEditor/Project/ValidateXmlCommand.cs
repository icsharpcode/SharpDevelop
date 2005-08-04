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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Validates the xml in the xml editor against the known schemas.
	/// </summary>
	public class ValidateXmlCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Validate the xml.
		/// </summary>
		public override void Run()
		{
			// Find active XmlView.
			XmlView xmlView = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as XmlView;
			if (xmlView != null) {
				// Validate the xml.
				xmlView.ValidateXml();
			}
		}
	}
}
