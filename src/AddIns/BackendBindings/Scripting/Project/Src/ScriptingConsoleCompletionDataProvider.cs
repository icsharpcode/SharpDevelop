// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// Provides code completion for the Scripting Console window.
	/// </summary>
	public class ScriptingConsoleCompletionDataProvider
	{
		IMemberProvider memberProvider;
		
		public ScriptingConsoleCompletionDataProvider(IMemberProvider memberProvider)
		{
			this.memberProvider = memberProvider;
		}
				
		public ICompletionData[] GenerateCompletionData(IScriptingConsoleTextEditor textEditor)
 		{
			string line = textEditor.GetLine(textEditor.TotalLines - 1);
			return GenerateCompletionData(line);
 		}
		
		/// <summary>
		/// Generates completion data for the specified text. The text should be everything before
		/// the dot character that triggered the completion. The text can contain the command line prompt
		/// '>>>' as this will be ignored.
		/// </summary>
		public ICompletionData[] GenerateCompletionData(string line)
		{
			List<ScriptingConsoleCompletionData> items = new List<ScriptingConsoleCompletionData>();

			string name = GetName(line);
			if (!String.IsNullOrEmpty(name)) {
				try {
					foreach (string member in memberProvider.GetMemberNames(name)) {
						items.Add(new ScriptingConsoleCompletionData(member));
					}
				} catch { 
					// Do nothing.
				}
			}
			return items.ToArray();
		}
		
		string GetName(string text)
		{
			int startIndex = text.LastIndexOf(' ');
			return text.Substring(startIndex + 1);
		}
	}
}
