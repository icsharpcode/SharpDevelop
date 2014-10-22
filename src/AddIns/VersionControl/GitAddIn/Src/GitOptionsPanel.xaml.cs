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
using System.IO;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

namespace ICSharpCode.GitAddIn
{
	/// <summary>
	/// Interaction logic for GitOptionsPanel.xaml
	/// </summary>
	public partial class GitOptionsPanel : OptionPanel
	{
		public GitOptionsPanel()
		{
			InitializeComponent();
			
			SetGitStatus();
		}

		void SetGitStatus()
		{
			string path = AddInOptions.PathToGit;
			if (path == null) {
				path = Git.FindGit();
				if (path == null) {
					path = SD.ResourceService.GetString("AddIns.Git.NoPathFoundStatus");
				} else {
					path += Environment.NewLine + SD.ResourceService.GetString("AddIns.Git.PathAutoDetectStatus");
				}
			}
			status.Text = path;
		}

		void FindGitPath_Click(object sender, RoutedEventArgs a)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = "exe";
			dlg.Filter = StringParser.Parse("Git|git.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			if (dlg.ShowDialog() == true) {
				string path = Path.GetDirectoryName(dlg.FileName);
				if (Git.IsGitPath(path)) {
					AddInOptions.PathToGit = Path.Combine(path, "git.exe");
				} else {
					MessageService.ShowError("${res:AddIns.Git.DirectoryDoesNotContainGit}");
				}
			}
			SetGitStatus();
		}
		
		void ResetGitPath_Click(object sender, RoutedEventArgs a)
		{
			AddInOptions.PathToGit = null;
			SetGitStatus();
		}
	}
}