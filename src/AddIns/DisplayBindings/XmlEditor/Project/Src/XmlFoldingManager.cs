// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class XmlFoldingManager : IDisposable
	{
		bool documentHasChangedSinceLastFoldUpdate;
		IDocument document;
		IXmlFoldParser xmlFoldParser;
		IFoldingManager foldingManager;
		const int NoFirstErrorOffset = -1;
		DispatcherTimer timer;
		bool updating;
		
		public XmlFoldingManager(ITextEditor textEditor)
			: this(textEditor, new FoldingManagerAdapter(textEditor), new XmlFoldParser())
		{
		}
		
		public XmlFoldingManager(ITextEditor textEditor, IFoldingManager foldingManager, IXmlFoldParser xmlFoldParser)
		{
			document = textEditor.Document;
			document.Changed += DocumentChanged;
			
			this.foldingManager = foldingManager;
			this.xmlFoldParser = xmlFoldParser;
		}
		
		void DocumentChanged(object source, TextChangeEventArgs e)
		{
			documentHasChangedSinceLastFoldUpdate = true;
		}
		
		public bool DocumentHasChangedSinceLastFoldUpdate {
			get { return documentHasChangedSinceLastFoldUpdate; }
			set { documentHasChangedSinceLastFoldUpdate = value; }
		}
		
		public ITextBuffer CreateTextEditorSnapshot()
		{
			return document.CreateSnapshot();
		}
		
		public void Dispose()
		{
			document.Changed -= DocumentChanged;
			foldingManager.Dispose();
		}
		
		public IList<FoldingRegion> GetFolds(ITextBuffer textBuffer)
		{
			return xmlFoldParser.GetFolds(textBuffer);
		}
		
		public void UpdateFolds(IEnumerable<FoldingRegion> folds)
		{
			IList<NewFolding> newFolds = ConvertFoldRegionsToNewFolds(folds);
			UpdateFolds(newFolds);			
		}
		
		public void UpdateFolds(IEnumerable<NewFolding> folds)
		{
			int firstErrorOffset = NoFirstErrorOffset;
			foldingManager.UpdateFoldings(folds, firstErrorOffset);
		}
		
		public void UpdateFolds()
		{
			ITextBuffer textBuffer = CreateTextEditorSnapshot();
			IList<FoldingRegion> folds = GetFolds(textBuffer);
			if (folds != null) {
				UpdateFolds(folds);
			}
		}
		
		public IList<NewFolding> ConvertFoldRegionsToNewFolds(IEnumerable<FoldingRegion> folds)
		{
			List<NewFolding> newFolds = new List<NewFolding>();
			foreach (FoldingRegion foldingRegion in folds) {
				NewFolding newFold = ConvertToNewFold(foldingRegion);
				newFolds.Add(newFold);
			}
			return newFolds;
		}
		
		NewFolding ConvertToNewFold(FoldingRegion foldingRegion)
		{
			NewFolding newFold = new NewFolding();
			
			newFold.Name = foldingRegion.Name;
			newFold.StartOffset = GetStartOffset(foldingRegion.Region);
			newFold.EndOffset = GetEndOffset(foldingRegion.Region);
			
			return newFold;
		}
		
		int GetStartOffset(DomRegion region)
		{
			return GetOffset(region.BeginLine, region.BeginColumn);
		}
		
		int GetEndOffset(DomRegion region)
		{
			return GetOffset(region.EndLine, region.EndColumn);
		}
		
		int GetOffset(int line, int column)
		{
			return document.PositionToOffset(line, column);
		}
		
		public void Start()
		{
			timer = new DispatcherTimer(DispatcherPriority.Background);
			timer.Interval = new TimeSpan(0, 0, 2);
			timer.Tick += TimerTick;
			timer.Start();
		}
		
		void TimerTick(object source, EventArgs e)
		{
			if (DocumentHasChangedSinceLastFoldUpdate) {
				if (!updating) {
					updating = true;
					DocumentHasChangedSinceLastFoldUpdate = false;
					UpdateFolds();
					updating = false;
				}
			}
		}
		
		public void Stop()
		{
			timer.Tick -= TimerTick;
			timer.Stop();
		}
	}
}
