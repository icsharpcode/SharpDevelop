// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using MSHelpSystem.Controls;
using MSHelpSystem.Core;

namespace MSHelpSystem.Commands
{
	public class ShowErrorHelpCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var view = (System.Windows.Controls.ListView)Owner;
			foreach (var t in view.SelectedItems.OfType<SDTask>().ToArray()) {
				if (t.BuildError == null)
					continue;

				string code = t.BuildError.ErrorCode;
				if (string.IsNullOrEmpty(code))
					return;

				if (Help3Environment.IsHelp3ProtocolRegistered) {
					LoggingService.Debug(string.Format("HelpViewer: Getting description of \"{0}\"", code));
					if (Help3Environment.IsLocalHelp)
						DisplayHelp.Keywords(code);
					else
						DisplayHelp.ContextualHelp(code);
				} else {
					LoggingService.Error("HelpViewer: Help system ist not initialized");
				}
			}
		}
	}
	
	public class DisplayContent : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!Help3Environment.IsHelp3ProtocolRegistered) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			if (Help3Service.Config.ExternalHelp) DisplayHelp.Catalog();
			else {
				PadDescriptor toc = SD.Workbench.GetPad(typeof(Help3TocPad));
				if (toc != null) toc.BringPadToFront();
			}
		}
	}

	public class DisplaySearch : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!Help3Environment.IsHelp3ProtocolRegistered) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			PadDescriptor search = SD.Workbench.GetPad(typeof(Help3SearchPad));
			if (search != null) search.BringPadToFront();
		}
	}
	
	public class LaunchHelpLibraryManager : AbstractMenuCommand
	{
		public override void Run()
		{
			if (string.IsNullOrEmpty(HelpLibraryManager.Manager)) {
				using (HelpLibraryManagerNotFoundForm form = new HelpLibraryManagerNotFoundForm()) {
					form.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			HelpLibraryManager.Start();
		}
	}
}
