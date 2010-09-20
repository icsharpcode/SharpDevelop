// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class CutProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableCut;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Cut();
		}
	}
	
	public class CopyProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableCopy;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Copy();
		}
	}
	
	public class PasteProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnablePaste;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Paste();
		}
	}
	
	public class DeleteProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableDelete;
			}
		}
		public override void Run()
		{
			ProjectBrowserPad.Instance.Delete();
		}
	}
	
}
