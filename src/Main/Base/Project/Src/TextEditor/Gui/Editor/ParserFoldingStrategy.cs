// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class ParserFoldingStrategy : IFoldingStrategy
	{
		void AddClassMembers(IClass c, List<FoldMarker> foldMarkers, IDocument document)
		{
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			DomRegion cRegion = c.BodyRegion;
			if (cRegion.IsEmpty) cRegion = c.Region;
			if (cRegion.BeginLine < cRegion.EndLine) {
				FoldMarker newFoldMarker = new FoldMarker(document, cRegion.BeginLine - 1, cRegion.BeginColumn - 1,
				                                          cRegion.EndLine - 1, cRegion.EndColumn, c.ClassType == ClassType.Enum ? FoldType.MemberBody : FoldType.TypeBody);
				if (newFoldMarker.Length > 0) {
					foldMarkers.Add(newFoldMarker);
				}
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMembers(innerClass, foldMarkers, document);
			}
			
			foreach (IMethod m in c.Methods) {
				if (m.Region.EndLine < m.BodyRegion.EndLine) {
					foldMarkers.Add(new FoldMarker(document, m.Region.EndLine - 1, m.Region.EndColumn - 1,
					                               m.BodyRegion.EndLine - 1, m.BodyRegion.EndColumn - 1, FoldType.MemberBody));
				}
			}
			
			foreach (IProperty p in c.Properties) {
				if (p.Region.EndLine < p.BodyRegion.EndLine) {
					foldMarkers.Add(new FoldMarker(document, p.Region.EndLine - 1, p.Region.EndColumn - 1,
					                               p.BodyRegion.EndLine- 1, p.BodyRegion.EndColumn - 1, FoldType.MemberBody));
				}
			}
			
			foreach (IEvent evt in c.Events) {
				if (evt.Region.EndLine < evt.BodyRegion.EndLine) {
					if (!evt.BodyRegion.IsEmpty) {
						foldMarkers.Add(new FoldMarker(document, evt.Region.EndLine - 1, evt.Region.EndColumn - 1,
						                               evt.BodyRegion.EndLine- 1, evt.BodyRegion.EndColumn - 1, FoldType.MemberBody));
					}
				}
			}
		}
		
		/// <remarks>
		/// Calculates the fold level of a specific line.
		/// </remarks>
		public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInfo)
		{
			ParseInformation parseInformation = parseInfo as ParseInformation;
			if (parseInformation == null || parseInformation.MostRecentCompilationUnit == null) {
				return null;
			}
			List<FoldMarker> foldMarkers = GetFoldMarkers(document, parseInformation.MostRecentCompilationUnit);
			if (parseInformation.BestCompilationUnit != parseInformation.MostRecentCompilationUnit) {
				List<FoldMarker> oldFoldMarkers = GetFoldMarkers(document, parseInformation.BestCompilationUnit);
				int lastLine = (foldMarkers.Count == 0) ? 0 : foldMarkers[foldMarkers.Count - 1].EndLine;
				int totalNumberOfLines = document.TotalNumberOfLines;
				foreach (FoldMarker marker in oldFoldMarkers) {
					if (marker.StartLine > lastLine && marker.EndLine < totalNumberOfLines)
						foldMarkers.Add(marker);
				}
			}
			return foldMarkers;
		}
		
		List<FoldMarker> GetFoldMarkers(IDocument document, ICompilationUnit cu)
		{
			List<FoldMarker> foldMarkers = new List<FoldMarker>();
			
			bool firstTime = document.FoldingManager.FoldMarker.Count == 0;
			foreach (FoldingRegion foldingRegion in cu.FoldingRegions) {
				foldMarkers.Add(new FoldMarker(document, foldingRegion.Region.BeginLine - 1, foldingRegion.Region.BeginColumn - 1,
				                               foldingRegion.Region.EndLine - 1, foldingRegion.Region.EndColumn - 1, FoldType.Region, foldingRegion.Name, firstTime));
				
			}
			foreach (IClass c in cu.Classes) {
				AddClassMembers(c, foldMarkers, document);
			}
			
			if (cu.DokuComments != null) {
				foreach (IComment c in cu.DokuComments) {
					foldMarkers.Add(new FoldMarker(document, c.Region.BeginLine - 1, c.Region.BeginColumn - 1,
					                               c.Region.EndLine - 1, c.Region.EndColumn - 1));
				}
			}
			return foldMarkers;
		}
	}
}
