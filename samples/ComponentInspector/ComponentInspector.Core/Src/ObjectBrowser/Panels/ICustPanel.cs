// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace NoGoop.ObjBrowser.Panels
{
	internal interface ICustPanel
	{
		Size PreferredSize { get; }

		void BeforeShow();
		bool AfterShow();
	}
}
