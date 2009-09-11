// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Uses SharpDevelop.Dom to create parsing information.
	/// </summary>
	public class ParserFoldingStrategy : IDisposable
	{
		readonly FoldingManager foldingManager;
		TextArea textArea;
		bool isFirstUpdate = true;
		
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
				foreach (IClass c in parseInfo.CompilationUnit.Classes) {
					AddClassMembers(c, newFoldMarkers);
				}
				foreach (FoldingRegion foldingRegion in parseInfo.CompilationUnit.FoldingRegions) {
					NewFolding f = new NewFolding(textArea.Document.GetOffset(foldingRegion.Region.BeginLine, foldingRegion.Region.BeginColumn),
					                              textArea.Document.GetOffset(foldingRegion.Region.EndLine, foldingRegion.Region.EndColumn));
					f.DefaultClosed = isFirstUpdate;
					f.Name = foldingRegion.Name;
					newFoldMarkers.Add(f);
				}
			}
			return newFoldMarkers.OrderBy(f => f.StartOffset);
		}
		
		void AddClassMembers(IClass c, List<NewFolding> newFoldMarkers)
		{
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			DomRegion cRegion = c.BodyRegion;
			if (cRegion.IsEmpty)
				cRegion = c.Region;
			if (cRegion.BeginLine < cRegion.EndLine) {
				newFoldMarkers.Add(new NewFolding(textArea.Document.GetOffset(cRegion.BeginLine, cRegion.BeginColumn),
				                                  textArea.Document.GetOffset(cRegion.EndLine, cRegion.EndColumn)));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMembers(innerClass, newFoldMarkers);
			}
			
			foreach (IMember m in c.AllMembers) {
				if (m.Region.EndLine < m.BodyRegion.EndLine) {
					newFoldMarkers.Add(new NewFolding(textArea.Document.GetOffset(m.Region.EndLine, m.Region.EndColumn),
					                                  textArea.Document.GetOffset(m.BodyRegion.EndLine, m.BodyRegion.EndColumn)));
				}
			}
		}
	}
}
