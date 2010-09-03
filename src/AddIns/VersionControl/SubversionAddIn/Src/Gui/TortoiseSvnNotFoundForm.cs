// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	public class TortoiseSvnNotFoundForm : ToolNotFoundDialog
	{
		public TortoiseSvnNotFoundForm()
			: base(StringParser.Parse("${res:AddIns.Subversion.TortoiseSVNRequired}"), "http://tortoisesvn.net/", null)
		{
		}
	}
}
