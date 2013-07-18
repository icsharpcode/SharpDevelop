// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
