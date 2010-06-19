// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Provides code completion for the Ruby Console window.
	/// </summary>
	public class RubyConsoleCompletionDataProvider
	{
		IMemberProvider memberProvider;
		
		public RubyConsoleCompletionDataProvider(IMemberProvider memberProvider)
		{
			this.memberProvider = memberProvider;
			//DefaultIndex = 0;
		}
		
		public ICompletionData[] GenerateCompletionData(IConsoleTextEditor textEditor)
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
			List<RubyConsoleCompletionData> items = new List<RubyConsoleCompletionData>();

			string name = GetName(line);
			if (!String.IsNullOrEmpty(name)) {
				try {
					foreach (string member in memberProvider.GetMemberNames(name)) {
						items.Add(new RubyConsoleCompletionData(member));
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
