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
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
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
			document.TextChanged += DocumentChanged;
			
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
		
		public ITextSource CreateTextEditorSnapshot()
		{
			return document.CreateSnapshot();
		}
		
		public void Dispose()
		{
			document.TextChanged -= DocumentChanged;
			foldingManager.Dispose();
		}
		
		public IList<FoldingRegion> GetFolds(ITextSource textSource)
		{
			return xmlFoldParser.GetFolds(textSource);
		}

		
		public void UpdateFolds(IEnumerable<FoldingRegion> folds)
		{
			int firstErrorOffset = NoFirstErrorOffset;
			foldingManager.UpdateFoldings(ConvertFoldRegionsToNewFolds(folds), firstErrorOffset);
		}
		
		public void UpdateFolds()
		{
			ITextSource textSource = CreateTextEditorSnapshot();
			IList<FoldingRegion> folds = GetFolds(textSource);
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
