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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Shows a message box if the portable library is not installed.
	/// </summary>
	public class CheckPortableLibraryInstalled : AbstractCommand
	{
		public static string CouldNotFindToolsDescription {
			get {
				return StringParser.Parse(
					"${res:PortableLibrary.CouldNotFindTools}" + Environment.NewLine + Environment.NewLine
					+ "${res:PortableLibrary.ToolsInstallationHelp}");
			}
		}
		
		public static string DownloadUrl {
			get { return "http://go.microsoft.com/fwlink/?LinkId=210823"; }
		}
		
		public override void Run()
		{
			if (!ProfileList.IsPortableLibraryInstalled()) {
				using (ToolNotFoundDialog dlg = new ToolNotFoundDialog(CouldNotFindToolsDescription, DownloadUrl)) {
					// our message is long, so make the window bigger than usual
					dlg.Width += 70;
					dlg.Height += 70;
					dlg.ShowDialog();
				}
			}
		}
	}
}
