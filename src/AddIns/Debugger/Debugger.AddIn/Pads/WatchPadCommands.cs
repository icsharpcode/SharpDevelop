// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Linq;
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
				pad.AddWatch();
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
}
