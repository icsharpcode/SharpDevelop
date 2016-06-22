// 
// TypeScriptCompletionItem.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Text;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptCompletionItem : DefaultCompletionItem
	{
		CompletionEntry entry;
		CompletionEntryDetailsProvider completionDetailsProvider;
		string description;
		
		public TypeScriptCompletionItem(CompletionEntry entry, CompletionEntryDetailsProvider completionDetailsProvider)
			: base(entry.name)
		{
			this.entry = entry;
			this.completionDetailsProvider = completionDetailsProvider;
			Image = GetImage(entry);
		}
		
		public override string Description {
			get {
				if (description == null) {
					description = GetDescription();
				}
				return description;
			}
			set { }
		}
		
		string GetDescription()
		{
			CompletionEntryDetails entryDetails = completionDetailsProvider.GetCompletionEntryDetails(entry.name);
			if (entryDetails == null) {
				return entry.name;
			}
			
			return GetDescription(entryDetails);
		}
		
		string GetDescription(CompletionEntryDetails entryDetails)
		{
			return String.Format(
				"{0} {1}",
				GetFullSymbolName(entryDetails),
				GetDocCommentPrecededByNewLine(entryDetails));
		}
		
		static string GetFullSymbolName(CompletionEntryDetails entryDetails)
		{
			if (entryDetails.displayParts == null) {
				return String.Empty;
			}
			
			var name = new StringBuilder();
			foreach (SymbolDisplayPart part in entryDetails.displayParts) {
				name.Append(part.text);
			}
			return name.ToString();
		}
		
		string GetDocCommentPrecededByNewLine(CompletionEntryDetails entryDetails)
		{
			if ((entryDetails.documentation == null) || (entryDetails.documentation.Length == 0)) {
				return String.Empty;
			}
			
			return String.Format("\r\n{0}", entryDetails.documentation[0].text);
		}
		
		IImage GetImage(CompletionEntry entry)
		{
			switch (entry.kind) {
				case "property":
					return ClassBrowserIconService.Property;
				case "constructor":
				case "getter":
				case "setter":
				case "method":
				case "function":
				case "local function":
					return ClassBrowserIconService.Method;
				case "keyword":
					return ClassBrowserIconService.Keyword;
				case "class":
					return ClassBrowserIconService.Class;
				case "var":
				case "local var":
					return ClassBrowserIconService.LocalVariable;
				case "interface":
					return ClassBrowserIconService.Interface;
				case "module":
					return ClassBrowserIconService.Namespace;
				default:
					return null;
			}
		}
	}
}
