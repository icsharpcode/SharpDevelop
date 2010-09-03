// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class XmlFoldingManagerTests
	{
		XmlFoldingManager xmlFoldingManager;
		MockTextEditor fakeTextEditor;
		FakeXmlFoldParser fakeXmlFoldParser;
		FakeFoldingManager fakeFoldingManager;
		
		[Test]
		public void DocumentHasChangedSinceLastFoldUpdate_DocumentHasNotChanged_ReturnsFalse()
		{
			CreateXmlFoldingManager();
			bool result = xmlFoldingManager.DocumentHasChangedSinceLastFoldUpdate;
			
			Assert.IsFalse(result);
		}
		
		void CreateXmlFoldingManager()
		{
			fakeTextEditor = new MockTextEditor();
			fakeXmlFoldParser = new FakeXmlFoldParser();
			fakeFoldingManager = new FakeFoldingManager();
			xmlFoldingManager = new XmlFoldingManager(fakeTextEditor, fakeFoldingManager, fakeXmlFoldParser);
		}
		
		[Test]
		public void DocumentHasChangedSinceLastFoldUpdate_DocumentChangedEventHasFired_ReturnsTrue()
		{
			CreateXmlFoldingManager();
			fakeTextEditor.MockDocument.RaiseChangedEvent();
			bool result = xmlFoldingManager.DocumentHasChangedSinceLastFoldUpdate;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CreateTextEditorSnapshot_DocumentHasFakeSnapshot_ReturnsFakeSnapshot()
		{
			CreateXmlFoldingManager();
			
			MockDocument expectedSnapshot = new MockDocument();
			fakeTextEditor.MockDocument.SetSnapshot(expectedSnapshot);
			
			ITextBuffer snapshot = xmlFoldingManager.CreateTextEditorSnapshot();
			
			Assert.AreEqual(expectedSnapshot, snapshot);
		}
		
		[Test]
		public void Dispose_DocumentChangedEventFiredAfterDispose_DocumentHasChangedSinceLastFoldUpdateReturnsFalse()
		{
			CreateXmlFoldingManager();
			xmlFoldingManager.Dispose();
			fakeTextEditor.MockDocument.RaiseChangedEvent();
			bool result = xmlFoldingManager.DocumentHasChangedSinceLastFoldUpdate;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetFolds_FakeXmlFoldParserUsed_ReturnsFoldsFromFakeXmlFoldParser()
		{
			CreateXmlFoldingManager();
			FoldingRegion fold = CreateFoldingRegion();
			fakeXmlFoldParser.Folds.Add(fold);
			
			MockDocument expectedSnapshot = new MockDocument();
			fakeTextEditor.MockDocument.SetSnapshot(expectedSnapshot);
			
			ITextBuffer snapshot = xmlFoldingManager.CreateTextEditorSnapshot();
			
			IList<FoldingRegion> folds = xmlFoldingManager.GetFolds(snapshot);
			
			Assert.AreEqual(fakeXmlFoldParser.Folds, folds);
		}
		
		FoldingRegion CreateFoldingRegion()
		{
			DomRegion region = new DomRegion(1, 2);
			return new FoldingRegion("a", region);
		}
		
		[Test]
		public void UpdateFolds_FakeFoldingManagerUsed_FoldsPassedToFakeFoldingManager()
		{
			CreateXmlFoldingManager();
			List<NewFolding> folds = NewFoldingHelper.CreateFoldListWithOneFold();
			xmlFoldingManager.UpdateFolds(folds);
			
			NewFoldingHelper.AssertAreEqual(folds, fakeFoldingManager.NewFoldsPassedToUpdateFoldings);
		}
		
		[Test]
		public void UpdateFolds_FakeFoldingManagerUsed_FirstErrorOffsetPassedToFakeFoldingManagerIsMinusOne()
		{
			CreateXmlFoldingManager();
			List<NewFolding> folds = NewFoldingHelper.CreateFoldListWithOneFold();
			xmlFoldingManager.UpdateFolds(folds);
			
			Assert.AreEqual(-1, fakeFoldingManager.FirstErrorOffset);
		}
		
		[Test]
		public void UpdateFolds_NoParameterPassedToMethod_FakeXmlFoldParserFoldsUsedToUpdateFakeFoldingManager()
		{
			CreateXmlFoldingManager();
			AddDocumentPositionToOffsetReturnValue(5);
			AddDocumentPositionToOffsetReturnValue(10);
			
			MockDocument expectedSnapshot = new MockDocument();
			fakeTextEditor.MockDocument.SetSnapshot(expectedSnapshot);
			
			FoldingRegion fold = CreateTestFoldingRegion();
			fakeXmlFoldParser.Folds.Add(fold);
			
			xmlFoldingManager.UpdateFolds();			

			IList<NewFolding> newFolds = fakeFoldingManager.NewFoldsPassedToUpdateFoldings;
			
			List<NewFolding> expectedFolds = CreateTestNewFoldingList();
			
			NewFoldingHelper.AssertAreEqual(expectedFolds, newFolds);
		}
		
		void AddDocumentPositionToOffsetReturnValue(int offset)
		{
			fakeTextEditor.MockDocument.PositionToOffsetReturnValues.Add(offset);
		}
		
		List<FoldingRegion> CreateTestFoldingRegionList()
		{
			FoldingRegion fold = CreateTestFoldingRegion();			
			List<FoldingRegion> folds = new List<FoldingRegion>();
			folds.Add(fold);
			return folds;
		}
		
		FoldingRegion CreateTestFoldingRegion()
		{
			int beginColumn = 1;
			int endColumn = 5;
			int beginLine = 2;
			int endLine = 3;
			DomRegion region = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			return new FoldingRegion("test", region);
		}
		
		List<NewFolding> CreateTestNewFoldingList()
		{
			NewFolding fold = CreateTestNewFolding();
			List<NewFolding> folds = new List<NewFolding>();
			folds.Add(fold);
			return folds;
		}
		
		NewFolding CreateTestNewFolding()
		{
			NewFolding expectedFold = new NewFolding();
			expectedFold.DefaultClosed = false;
			expectedFold.StartOffset = 5;
			expectedFold.EndOffset = 10;
			expectedFold.Name = "test";
			
			return expectedFold;
		}
		
		[Test]
		public void UpdateFolds_NoParameterPassedToMethod_SnapshotPassedToXmlFoldParser()
		{
			CreateXmlFoldingManager();
			AddDocumentPositionToOffsetReturnValue(5);
			AddDocumentPositionToOffsetReturnValue(10);
						
			MockDocument expectedSnapshot = new MockDocument();
			fakeTextEditor.MockDocument.SetSnapshot(expectedSnapshot);
						
			FoldingRegion fold = CreateTestFoldingRegion();
			fakeXmlFoldParser.Folds.Add(fold);
			
			xmlFoldingManager.UpdateFolds();			
			
			Assert.AreEqual(expectedSnapshot, fakeXmlFoldParser.TextBufferPassedToGetFolds);
		}
		
		[Test]
		public void ConvertFoldRegionsToNewFolds_OneFoldingRegionInList_CreatesNewFoldUsingDocumentPositionToOffset()
		{
			CreateXmlFoldingManager();
			AddDocumentPositionToOffsetReturnValue(5);
			AddDocumentPositionToOffsetReturnValue(10);

			List<FoldingRegion> folds = CreateTestFoldingRegionList();
			
			IList<NewFolding> convertedFolds = xmlFoldingManager.ConvertFoldRegionsToNewFolds(folds);
			
			List<NewFolding> expectedFolds = CreateTestNewFoldingList();
			
			NewFoldingHelper.AssertAreEqual(expectedFolds, convertedFolds);
		}
		
		[Test]
		public void UpdateFolds_XmlFoldParserReturnsNullFromGetFolds_FoldingManagerFoldsAreNotUpdated()
		{
			CreateXmlFoldingManager();
			
			MockDocument expectedSnapshot = new MockDocument();
			fakeTextEditor.MockDocument.SetSnapshot(expectedSnapshot);
			fakeXmlFoldParser.Folds = null;
			
			xmlFoldingManager.UpdateFolds();
			
			bool result = fakeFoldingManager.IsUpdateFoldingsCalled;
			
			Assert.IsFalse(result);
		}
	}
}
