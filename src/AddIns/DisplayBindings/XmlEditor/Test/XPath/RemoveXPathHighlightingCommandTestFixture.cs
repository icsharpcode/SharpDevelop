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
using System.ComponentModel.Design;
using System.Linq;
using System.Xml.XPath;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Tests.Utils;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class RemoveXPathHighlightingCommandTestFixture
	{
		IWorkbench workbench;
		ITextMarkerService markerService;
		RemoveXPathHighlightingCommand command;
		MockViewContent nonTextEditorProviderView;
		MockTextEditorProviderViewContent textEditorView;
		
		[SetUp]
		public void Init()
		{
			IDocument doc = MockTextMarkerService.CreateDocumentWithMockService();
			markerService = doc.GetRequiredService<ITextMarkerService>();
			
			// Add xpath marker to document.
			doc.Text = "<Test/>";
			XPathNodeTextMarker xpathNodeMarker = new XPathNodeTextMarker(doc);
			XPathNodeMatch nodeMatch = new XPathNodeMatch("Test", "<Test/>", 0, 1, XPathNodeType.Element);
			xpathNodeMarker.AddMarker(nodeMatch);
			
			// Add non text editor provider view to workbench.
			workbench = MockRepository.GenerateStrictMock<IWorkbench>();
			workbench.Stub(w => w.ViewContentCollection).Return(new List<IViewContent>());
			
			nonTextEditorProviderView = new MockViewContent();
			workbench.ViewContentCollection.Add(nonTextEditorProviderView);

			// Add document to view content.
			textEditorView = new MockTextEditorProviderViewContent();
			textEditorView.MockTextEditor.SetDocument(doc);
			workbench.ViewContentCollection.Add(textEditorView);
			
			command = new RemoveXPathHighlightingCommand(workbench);
		}
		
		[Test]
		public void CommandRunRemovesAllXPathNodeTextMarkersRemovedFromAllTextEditorWindows()
		{
			command.Run();
			Assert.AreEqual(0, markerService.TextMarkers.Count());
		}
		
		[Test]
		public void WorkbenchTextEditorsHaveAtLeastOneTextMarker()
		{
			Assert.IsTrue(markerService.TextMarkers.Count() > 0);
		}
	}
}
