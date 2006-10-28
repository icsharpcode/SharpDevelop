// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ICSharpCode.TextEditor.Document
{
	public class FoldingManager
	{
		List<FoldMarker>    foldMarker      = new List<FoldMarker>();
		IFoldingStrategy    foldingStrategy = null;
		IDocument document;
		
		public IList<FoldMarker> FoldMarker {
			get {
				return foldMarker.AsReadOnly();
			}
		}
		
		public IFoldingStrategy FoldingStrategy {
			get {
				return foldingStrategy;
			}
			set {
				foldingStrategy = value;
			}
		}
		
		public FoldingManager(IDocument document, ILineManager lineTracker)
		{
			this.document = document;
			document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
			
//			lineTracker.LineCountChanged  += new LineManagerEventHandler(LineManagerLineCountChanged);
//			lineTracker.LineLengthChanged += new LineLengthEventHandler(LineManagerLineLengthChanged);
//			foldMarker.Add(new FoldMarker(0, 5, 3, 5));
//			
//			foldMarker.Add(new FoldMarker(5, 5, 10, 3));
//			foldMarker.Add(new FoldMarker(6, 0, 8, 2));
//			
//			FoldMarker fm1 = new FoldMarker(10, 4, 10, 7);
//			FoldMarker fm2 = new FoldMarker(10, 10, 10, 14);
//			
//			fm1.IsFolded = true;
//			fm2.IsFolded = true;
//			
//			foldMarker.Add(fm1);
//			foldMarker.Add(fm2);
//			foldMarker.Sort();
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			int oldCount = foldMarker.Count;
			document.UpdateSegmentListOnDocumentChange(foldMarker, e);
			if (oldCount != foldMarker.Count) {
				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			}
		}
		
		public List<FoldMarker> GetFoldingsFromPosition(int line, int column)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				for (int i = 0; i < foldMarker.Count; ++i) {
					FoldMarker fm = foldMarker[i];
					if ((fm.StartLine == line && column > fm.StartColumn && !(fm.EndLine == line && column >= fm.EndColumn)) ||
					    (fm.EndLine == line && column < fm.EndColumn && !(fm.StartLine == line && column <= fm.StartColumn)) ||
					    (line > fm.StartLine && line < fm.EndLine)) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public List<FoldMarker> GetFoldingsWithStart(int lineNumber)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.StartLine == lineNumber) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public List<FoldMarker> GetFoldedFoldingsWithStart(int lineNumber)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.IsFolded && fm.StartLine == lineNumber) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public List<FoldMarker> GetFoldedFoldingsWithStartAfterColumn(int lineNumber, int column)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.IsFolded && fm.StartLine == lineNumber && fm.StartColumn > column) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public List<FoldMarker> GetFoldingsWithEnd(int lineNumber)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.EndLine == lineNumber) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public List<FoldMarker> GetFoldedFoldingsWithEnd(int lineNumber)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.IsFolded && fm.EndLine == lineNumber) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public bool IsFoldStart(int lineNumber)
		{
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.StartLine == lineNumber) {
						return true;
					}
				}
			}
			return false;
		}
		
		public bool IsFoldEnd(int lineNumber)
		{
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.EndLine == lineNumber) {
						return true;
					}
				}
			}
			return false;
		}
		
		public List<FoldMarker> GetFoldingsContainsLineNumber(int lineNumber)
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.StartLine < lineNumber && lineNumber < fm.EndLine) {
						foldings.Add(fm);
					}
				}
			}
			return foldings;
		}
		
		public bool IsBetweenFolding(int lineNumber)
		{
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.StartLine < lineNumber && lineNumber < fm.EndLine) {
						return true;
					}
				}
			}
			return false;
		}
		
		public bool IsLineVisible(int lineNumber)
		{
			if (foldMarker != null) {
				foreach (FoldMarker fm in foldMarker) {
					if (fm.IsFolded && fm.StartLine < lineNumber && lineNumber < fm.EndLine) {
						return false;
					}
				}
			}
			return true;
		}
		
		public List<FoldMarker> GetTopLevelFoldedFoldings()
		{
			List<FoldMarker> foldings = new List<FoldMarker>();
			if (foldMarker != null) {
				Point end = new Point(0, 0);
				foreach (FoldMarker fm in foldMarker) {
					if (fm.IsFolded && (fm.StartLine > end.Y || fm.StartLine == end.Y && fm.StartColumn >= end.X)) {
						foldings.Add(fm);
						end = new Point(fm.EndColumn, fm.EndLine);
					}
				}
			}
			return foldings;
		}
		
		public void UpdateFoldings(string fileName, object parseInfo)
		{
			UpdateFoldings(foldingStrategy.GenerateFoldMarkers(document, fileName, parseInfo));
		}
		
		public void UpdateFoldings(List<FoldMarker> newFoldings)
		{
			int oldFoldingsCount = foldMarker.Count;
			lock (this) {
				if (newFoldings != null && newFoldings.Count != 0) {
					newFoldings.Sort();
					if (foldMarker.Count == newFoldings.Count) {
						for (int i = 0; i < foldMarker.Count; ++i) {
							newFoldings[i].IsFolded = foldMarker[i].IsFolded;
						}
						foldMarker = newFoldings;
					} else {
						for (int i = 0, j = 0; i < foldMarker.Count && j < newFoldings.Count;) {
							int n = newFoldings[j].CompareTo(foldMarker[i]);
							if (n > 0) {
								++i;
							} else {
								if (n == 0) {
									newFoldings[j].IsFolded = foldMarker[i].IsFolded;
								}
								++j;
							}
						}
					}
				}
				if (newFoldings != null) {
					foldMarker = newFoldings;
				} else {
					foldMarker.Clear();
				}
			}
			if (oldFoldingsCount != foldMarker.Count) {
				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				document.CommitUpdate();
			}
		}
		
		public string SerializeToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (FoldMarker marker in this.foldMarker) {
				sb.Append(marker.Offset);sb.Append("\n");
				sb.Append(marker.Length);sb.Append("\n");
				sb.Append(marker.FoldText);sb.Append("\n");
				sb.Append(marker.IsFolded);sb.Append("\n");
			}
			return sb.ToString();
		}
		
		public void DeserializeFromString(string str)
		{
			try {
				string[] lines = str.Split('\n');
				for (int i = 0; i < lines.Length && lines[i].Length > 0; i += 4) {
					int    offset = Int32.Parse(lines[i]);
					int    length = Int32.Parse(lines[i + 1]);
					string text   = lines[i + 2];
					bool isFolded = Boolean.Parse(lines[i + 3]);
					bool found    = false;
					foreach (FoldMarker marker in foldMarker) {
						if (marker.Offset == offset && marker.Length == length) {
							marker.IsFolded = isFolded;
							found = true;
							break;
						}
					}
					if (!found) {
						foldMarker.Add(new FoldMarker(document, offset, length, text, isFolded));
					}
				}
				if (lines.Length > 0) {
					NotifyFoldingsChanged(EventArgs.Empty);
				}
			} catch (Exception) {
			}
		}
		
		public void NotifyFoldingsChanged(EventArgs e)
		{
			if (FoldingsChanged != null) {
				FoldingsChanged(this, e);
			}
		}
		
		
		public event EventHandler FoldingsChanged;
	}
}
