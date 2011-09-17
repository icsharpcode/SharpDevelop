// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerSideBar : SharpDevelopSideBar
	{
		public FormsDesignerSideBar()
		{
			this.sideTabContextMenuPath = "/SharpDevelop/Workbench/SharpDevelopSideBar/SideTab/FormsDesignerContextMenu";
		}
	}
}
