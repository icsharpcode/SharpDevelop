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
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
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
			
			ITextSource snapshot = xmlFoldingManager.CreateTextEditorSnapshot();
			
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
			
			ITextSource snapshot = xmlFoldingManager.CreateTextEditorSnapshot();
			
			IList<FoldingRegion> folds = xmlFoldingManager.GetFolds(snapshot);
			
			Assert.AreEqual(fakeXmlFoldParser.Folds, folds);
		}
		
		FoldingRegion CreateFoldingRegion()
		{
			DomRegion region = new DomRegion(1, 2);
			return new FoldingRegion("a", region);
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
