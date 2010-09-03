// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public class SortSelectionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SortOptionsDialog dlg = new SortOptionsDialog();
			dlg.Owner = WorkbenchSingleton.MainWindow;
			if (dlg.ShowDialog() == true) {
				StringComparer comparer = SortOptions.CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
				if (SortOptions.IgnoreTrailingWhitespaces)
					comparer = new IgnoreTrailingWhitespaceComparer(comparer);
				if (SortOptions.SortDirection == SortDirection.Descending)
					comparer = new DescendingStringComparer(comparer);
				
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (provider != null) {
					ITextEditor editor = provider.TextEditor;
					if (editor.SelectionLength > 0) {
						int start = editor.Document.GetLineForOffset(editor.SelectionStart).LineNumber;
						int end = editor.Document.GetLineForOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
						SortLines(editor.Document, start, end, comparer, SortOptions.RemoveDuplicates);
					} else {
						SortLines(editor.Document, 1, editor.Document.TotalNumberOfLines, comparer, SortOptions.RemoveDuplicates);
					}
				}
			}
		}
		
		public void SortLines(IDocument document, int startLine, int endLine, StringComparer comparer, bool removeDuplicates)
		{
			List<string> lines = new List<string>();
			for (int i = startLine; i <= endLine; ++i) {
				IDocumentLine line = document.GetLine(i);
				lines.Add(document.GetText(line.Offset, line.Length));
			}
			
			lines.Sort(comparer);
			
			if (removeDuplicates) {
				lines = lines.Distinct(comparer).ToList();
			}
			
			using (document.OpenUndoGroup()) {
				for (int i = 0; i < lines.Count; ++i) {
					IDocumentLine line = document.GetLine(startLine + i);
					document.Replace(line.Offset, line.Length, lines[i]);
				}
				
				// remove removed duplicate lines
				for (int i = startLine + lines.Count; i <= endLine; ++i) {
					IDocumentLine line = document.GetLine(startLine + lines.Count);
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
