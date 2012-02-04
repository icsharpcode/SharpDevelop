// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				MessageService.ShowException(ex, message);
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
					new StringTagPair("Assembly", include), new StringTagPair("Filename", fileName)
				) + "\r\n" + message + "\r\n"
			);
		}
	}
}
