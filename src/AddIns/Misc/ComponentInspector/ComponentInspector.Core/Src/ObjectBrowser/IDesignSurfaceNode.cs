// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using NoGoop.Obj;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser
{
	// This node changes the context menu
	internal interface IDesignSurfaceNode : IBrowserNode 
	{
		bool OnDesignSurface { get; set; }

		void DesignSurfaceClick(object sender, EventArgs e);
	}
}
