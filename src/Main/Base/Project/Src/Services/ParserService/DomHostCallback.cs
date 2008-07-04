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
			
			HostCallback.ShowAssemblyLoadError = delegate(string fileName, string include, string message) {
				WorkbenchSingleton.SafeThreadAsyncCall(ShowAssemblyLoadError,
				                                       fileName, include, message);
			};
			
			HostCallback.InitializeCodeGeneratorOptions = AmbienceService.InitializeCodeGeneratorOptions;
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
	}
}
