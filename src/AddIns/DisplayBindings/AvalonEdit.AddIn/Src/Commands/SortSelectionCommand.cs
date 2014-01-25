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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public class SortSelectionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SortOptionsDialog dlg = new SortOptionsDialog();
			dlg.Owner = SD.Workbench.MainWindow;
			if (dlg.ShowDialog() == true) {
				StringComparer comparer = SortOptions.CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
				if (SortOptions.IgnoreTrailingWhitespaces)
					comparer = new IgnoreTrailingWhitespaceComparer(comparer);
				if (SortOptions.SortDirection == SortDirection.Descending)
					comparer = new DescendingStringComparer(comparer);
				
				ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
				if (editor != null) {
					if (editor.SelectionLength > 0) {
						int start = editor.Document.GetLineByOffset(editor.SelectionStart).LineNumber;
						int end = editor.Document.GetLineByOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
						SortLines(editor.Document, start, end, comparer, SortOptions.RemoveDuplicates);
					} else {
						SortLines(editor.Document, 1, editor.Document.LineCount, comparer, SortOptions.RemoveDuplicates);
					}
				}
			}
		}
		
		public void SortLines(IDocument document, int startLine, int endLine, StringComparer comparer, bool removeDuplicates)
		{
			List<string> lines = new List<string>();
			for (int i = startLine; i <= endLine; ++i) {
				IDocumentLine line = document.GetLineByNumber(i);
				lines.Add(document.GetText(line.Offset, line.Length));
			}
			
			lines.Sort(comparer);
			
			if (removeDuplicates) {
				lines = lines.Distinct(comparer).ToList();
			}
			
			using (document.OpenUndoGroup()) {
				for (int i = 0; i < lines.Count; ++i) {
					IDocumentLine line = document.GetLineByNumber(startLine + i);
					document.Replace(line.Offset, line.Length, lines[i]);
				}
				
				// remove removed duplicate lines
				for (int i = startLine + lines.Count; i <= endLine; ++i) {
					IDocumentLine line = document.GetLineByNumber(startLine + lines.Count);
					document.Remove(line.Offset, line.TotalLength);
				}
			}
		}
		
		sealed class DescendingStringComparer : StringComparer
		{
			StringComparer baseComparer;
			
			public DescendingStringComparer(StringComparer baseComparer)
			{
				this.baseComparer = baseComparer;
			}
			
			public override int Compare(string x, string y)
			{
				return -baseComparer.Compare(x, y);
			}
			
			public override bool Equals(string x, string y)
			{
				return baseComparer.Equals(x, y);
			}
			
			public override int GetHashCode(string obj)
			{
				return baseComparer.GetHashCode(obj);
			}
		}
		
		sealed class IgnoreTrailingWhitespaceComparer : StringComparer
		{
			StringComparer baseComparer;
			
			public IgnoreTrailingWhitespaceComparer(StringComparer baseComparer)
			{
				this.baseComparer = baseComparer;
			}
			
			public override int Compare(string x, string y)
			{
				if (x != null)
					x = x.TrimEnd();
				if (y != null)
					y = y.TrimEnd();
				return baseComparer.Compare(x, y);
			}
			
			public override bool Equals(string x, string y)
			{
				if (x != null)
					x = x.TrimEnd();
				if (y != null)
					y = y.TrimEnd();
				return baseComparer.Equals(x, y);
			}
			
			public override int GetHashCode(string obj)
			{
				if (obj != null)
					obj = obj.TrimEnd();
				return baseComparer.GetHashCode(obj);
			}
		}
	}
	
	public static class SortOptions
	{
		public static bool RemoveDuplicates {
			get { return PropertyService.Get("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.RemoveDuplicateLines", false); }
			set { PropertyService.Set("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.RemoveDuplicateLines", value); }
		}
		
		public static bool CaseSensitive {
			get { return PropertyService.Get("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.CaseSensitive", true); }
			set { PropertyService.Set("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.CaseSensitive", value); }
		}
		
		public static bool IgnoreTrailingWhitespaces {
			get { return PropertyService.Get("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.IgnoreWhitespaces", false); }
			set { PropertyService.Set("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.IgnoreWhitespaces", value); }
		}
		
		public static SortDirection SortDirection {
			get { return PropertyService.Get("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.SortDirection", SortDirection.Ascending); }
			set { PropertyService.Set("ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.SortDirection", value); }
		}
	}
	
	public enum SortDirection
	{
		Ascending,
		Descending
	}
}
