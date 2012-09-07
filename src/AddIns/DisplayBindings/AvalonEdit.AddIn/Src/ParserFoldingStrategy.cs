// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Uses the NRefactory type system to create parsing information.
	/// </summary>
	[TextEditorService]
	public class ParserFoldingStrategy : IDisposable
	{
		readonly FoldingManager foldingManager;
		
		TextArea textArea;
		bool isFirstUpdate = true;
		
		public FoldingManager FoldingManager {
			get { return foldingManager; }
		}
		
		public ParserFoldingStrategy(TextArea textArea)
		{
			this.textArea = textArea;
			foldingManager = FoldingManager.Install(textArea);
		}
		
		public void Dispose()
		{
			if (textArea != null) {
				FoldingManager.Uninstall(foldingManager);
				textArea = null;
			}
		}
		
		public void UpdateFoldings(ParseInformation parseInfo)
		{
			IEnumerable<NewFolding> newFoldings = GetNewFoldings(parseInfo);
			foldingManager.UpdateFoldings(newFoldings, -1);
			
			isFirstUpdate = false;
		}
		
		IEnumerable<NewFolding> GetNewFoldings(ParseInformation parseInfo)
		{
			List<NewFolding> newFoldMarkers = new List<NewFolding>();
			if (parseInfo != null) {
				foreach (var c in parseInfo.UnresolvedFile.TopLevelTypeDefinitions) {
					AddClassMembers(c, newFoldMarkers);
				}
				#warning Additional folding regions
				/*
				foreach (FoldingRegion foldingRegion in parseInfo.FoldingRegions) {
					NewFolding f = new NewFoldingDefinition(GetOffset(foldingRegion.Region.BeginLine, foldingRegion.Region.BeginColumn),
					                                        GetOffset(foldingRegion.Region.EndLine, foldingRegion.Region.EndColumn));
					f.DefaultClosed = isFirstUpdate;
					f.Name = foldingRegion.Name;
					newFoldMarkers.Add(f);
				}*/
			}
			return newFoldMarkers.OrderBy(f => f.StartOffset);
		}
		
		public static bool IsDefinition(FoldingSection section)
		{
			return section.Tag is NewFoldingDefinition;
		}
		
		sealed class NewFoldingDefinition : NewFolding
		{
			public NewFoldingDefinition(int start, int end) : base(start, end) {}
		}
		
		void AddClassMembers(IUnresolvedTypeDefinition c, List<NewFolding> newFoldMarkers)
		{
			if (c.Kind == TypeKind.Delegate) {
				return;
			}
			DomRegion cRegion = c.BodyRegion;
			if (c.BodyRegion.BeginLine < c.BodyRegion.EndLine) {
				newFoldMarkers.Add(new NewFolding(GetStartOffset(c.BodyRegion), GetEndOffset(c.BodyRegion)));
			}
			foreach (var innerClass in c.NestedTypes) {
				AddClassMembers(innerClass, newFoldMarkers);
			}
			
			foreach (var m in c.Members) {
				if (m.BodyRegion.BeginLine < m.BodyRegion.EndLine) {
					newFoldMarkers.Add(new NewFoldingDefinition(GetStartOffset(m.BodyRegion), GetEndOffset(m.BodyRegion)));
				}
			}
		}
		
		int GetStartOffset(DomRegion bodyRegion)
		{
			var document = textArea.Document;
			if (bodyRegion.BeginLine < 1)
				return 0;
			if (bodyRegion.BeginLine > document.LineCount)
				return document.TextLength;
			var line = document.GetLineByNumber(bodyRegion.BeginLine);
			int lineStart = line.Offset;
			int bodyStartOffset = lineStart + bodyRegion.BeginColumn - 1;
			for (int i = lineStart; i < bodyStartOffset; i++) {
				if (!char.IsWhiteSpace(document.GetCharAt(i))) {
					// Non-whitespace in front of body start:
					// Use the body start as start offset
					return bodyStartOffset;
				}
			}
			// Only whitespace in front of body start:
			// Use the end of the previous line as start offset
			return line.PreviousLine != null ? line.PreviousLine.EndOffset : bodyStartOffset;
		}
		
		int GetEndOffset(DomRegion region)
		{
			if (region.EndLine < 1)
				return 0;
			var document = textArea.Document;
			if (region.EndLine > document.LineCount)
				return document.TextLength;
			return document.GetOffset(region.End);
		}
	}
}
