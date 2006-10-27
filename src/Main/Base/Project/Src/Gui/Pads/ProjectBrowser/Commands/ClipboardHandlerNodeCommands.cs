// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
