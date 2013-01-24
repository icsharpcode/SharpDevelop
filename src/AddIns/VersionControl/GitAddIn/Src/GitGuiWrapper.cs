// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

namespace ICSharpCode.GitAddIn
{
	/// <summary>
	/// Wraps commands opening a dialog window.
	/// The current implementation launches TortoiseSVN.
	/// </summary>
	public static class GitGuiWrapper
	{
		static string GetPathFromRegistry(RegistryHive hive, string valueName)
		{
			RegistryView view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
			using (RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view)) {
				using (RegistryKey key = baseKey.OpenSubKey("SOFTWARE\\TortoiseGit")) {
					if (key != null)
						return key.GetValue(valueName) as string;
					else
						return null;
				}
			}
		}
		
		static string GetPathFromRegistry(string valueName)
		{
			return GetPathFromRegistry(RegistryHive.CurrentUser, valueName)
				?? GetPathFromRegistry(RegistryHive.LocalMachine, valueName);
		}
		
		static void Proc(string command, string fileName, Action callback)
		{
			Proc(command, fileName, callback, null);
		}
		
		static void Proc(string command, string fileName, Action callback, string argument)
		{
			string path = GetPathFromRegistry("ProcPath");
			if (path == null) {
				using (var dlg = new ToolNotFoundDialog(StringParser.Parse("${res:AddIns.Git.TortoiseGitRequired}"), "http://code.google.com/p/tortoisegit/")) {
					dlg.ShowDialog(WorkbenchSingleton.MainWin32Window);
				}
			} else {
				try {
					StringBuilder arguments = new StringBuilder();
					arguments.Append("/command:");
					arguments.Append(command);
					if (fileName != null) {
						arguments.Append(" /notempfile ");
						arguments.Append(" /path:\"");
						arguments.Append(fileName);
						arguments.Append('"');
					}
					if (argument != null) {
						arguments.Append(' ');
						arguments.Append(argument);
					}
					Process p = new Process();
					p.StartInfo.FileName = path;
					p.StartInfo.Arguments = arguments.ToString();
					//p.StartInfo.RedirectStandardError = true;
					//p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.UseShellExecute = false;
					p.EnableRaisingEvents = true;
					p.Exited += delegate {
						p.Dispose();
						if (callback != null) { ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(callback); }
					};
//					p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
//						SvnClient.Instance.SvnCategory.AppendText(e.Data);
//					};
//					p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
//						SvnClient.Instance.SvnCategory.AppendText(e.Data);
//					};
					p.Start();
				} catch (Exception ex) {
					MessageService.ShowError(ex.Message);
				}
			}
		}
		
		public static void Commit(string fileName, Action callback)
		{
			Proc("commit", fileName, callback);
		}
		
		public static void Diff(string fileName, Action callback)
		{
			Proc("diff", fileName, callback);
		}
		
		public static void Log(string fileName, Action callback)
		{
			Proc("log", fileName, callback);
		}
	}
}
