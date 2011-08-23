// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public abstract class XmlTreeNodeClipboardCommand : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get { 
				IClipboardHandler editable = GetClipboardHandler();
				if (editable != null) {
					return GetEnabled(editable);
				}
				return false;
			}
		}
		
		IClipboardHandler GetClipboardHandler()
		{
			return WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
		}
		
		protected abstract bool GetEnabled(IClipboardHandler editable);
		protected abstract void Run(IClipboardHandler editable);
		
		public override void Run()
		{
			if (IsEnabled) {
				IClipboardHandler editable = GetClipboardHandler();
				if (editable != null) {
					Run(editable);
				}
			}
		}
	}
}
