// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using Microsoft.Win32;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Implements the methods in ICSharpCode.SharpDevelop.Dom.HostCallback
	/// </summary>
	internal static class DomHostCallback
	{
		internal static void Register()
		{
			HostCallback.RenameMember = Refactoring.FindReferencesAndRenameHelper.RenameMember;
			HostCallback.ShowMessage = MessageService.ShowMessage;
			
			HostCallback.GetCurrentProjectContent = delegate {
				return ParserService.CurrentProjectContent;
			};
			
			HostCallback.ShowError = delegate(string message, Exception ex) {
				MessageService.ShowError(ex, message);
			};
			
			HostCallback.BeginAssemblyLoad = delegate(string shortName) {
				StatusBarService.ProgressMonitor.BeginTask(
					StringParser.Parse("${res:ICSharpCode.SharpDevelop.LoadingFile}", new string[,] {{"Filename", shortName}}),
					100, false
				);
			};
			HostCallback.FinishAssemblyLoad = StatusBarService.ProgressMonitor.Done;
			
			HostCallback.ShowAssemblyLoadError = delegate(string fileName, string include, string message) {
				WorkbenchSingleton.SafeThreadAsyncCall(ShowAssemblyLoadError,
				                                       fileName, include, message);
			};
			
			HostCallback.InitializeCodeGeneratorOptions = AmbienceService.InitializeCodeGeneratorOptions;
			
			string dir = WinFXReferenceDirectory;
			if (!string.IsNullOrEmpty(dir))
				XmlDoc.XmlDocLookupDirectories.Add(dir);
			dir = XNAReferenceDirectory;
			if (!string.IsNullOrEmpty(dir))
				XmlDoc.XmlDocLookupDirectories.Add(dir);
		}
		
		static void ShowAssemblyLoadError(string fileName, string include, string message)
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			TaskService.BuildMessageViewCategory.AppendText(
				StringParser.Parse(
					"${res:ICSharpCode.SharpDevelop.ErrorLoadingCodeCompletionInformation}",
					new string[,] { {"Assembly", include}, {"Filename", fileName}}
				) + "\r\n" + message + "\r\n"
			);
		}
		
		static string WinFXReferenceDirectory {
			get {
				RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup\Windows Communication Foundation");
				if (k == null)
					return null;
				object o = k.GetValue("ReferenceInstallPath");
				k.Close();
				if (o == null)
					return null;
				else
					return o.ToString();
			}
		}
		
		static string XNAReferenceDirectory {
			get {
				RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\XNA\Game Studio Express\v1.0");
				if (k == null)
					return null;
				object o = k.GetValue("InstallPath");
				k.Close();
				if (o == null)
					return null;
				else
					return Path.Combine(o.ToString(), @"References\Windows\x86");
			}
		}
	}
}
