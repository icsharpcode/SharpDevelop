// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using Debugger.AddIn.Pads;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn
{
	public class AddWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				pad.AddWatch(focus: true);
			}
		}
	}
	
	public class RemoveWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				pad.Items.Remove(pad.Tree.SelectedItem as SharpTreeNodeAdapter);
				WindowsDebugger.RefreshPads();
			}
		}
	}
	
	public class ClearWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				pad.Items.Clear();
			}
		}
	}

	public class AddWatchExpressionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor == null) return;
			var pad = SD.Workbench.GetPad(typeof(WatchPad));
			if (pad == null) return;
			pad.BringPadToFront();
			((WatchPad)pad.PadContent).AddWatch(editor.SelectedText);
		}
	}
}
