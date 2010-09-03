// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Tests.Utils;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class RemoveXPathHighlightingCommandTestFixture
	{
		MockWorkbench workbench;
		ITextMarkerService markerService;
		RemoveXPathHighlightingCommand command;
		MockViewContent nonTextEditorProviderView;
		MockTextEditorProviderViewContent textEditorView;
		
		[SetUp]
		public void Init()
		{
			ServiceContainer container = new ServiceContainer();
			markerService = new MockTextMarkerService();
			container.AddService(typeof(ITextMarkerService), markerService);
			
			// Add xpath marker to document.
			AvalonEditDocumentAdapter doc = new AvalonEditDocumentAdapter(new ICSharpCode.AvalonEdit.Document.TextDocument(), container);
			doc.Text = "<Test/>";
			XPathNodeTextMarker xpathNodeMarker = new XPathNodeTextMarker(doc);
			XPathNodeMatch nodeMatch = new XPathNodeMatch("Test", "<Test/>", 0, 1, XPathNodeType.Element);
			xpathNodeMarker.AddMarker(nodeMatch);
			
			// Add non text editor provider view to workbench.
			workbench = new MockWorkbench();
			
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
			Assert.AreEqual(0, GetAllXPathTextMarkers().Count);
		}
		
		List<ITextMarker> GetAllXPathTextMarkers()
		{
			return new List<ITextMarker>(markerService.TextMarkers);
		}
		
		[Test]
		public void WorkbenchTextEditorsHaveAtLeastOneTextMarker()
		{
			Assert.IsTrue(GetAllXPathTextMarkers().Count > 0);
		}
		
		[Test]
		public void MockViewContentDoesNotImplementITextEditorProviderInterface()
		{
			Assert.IsNull(nonTextEditorProviderView as ITextEditorProvider);
		}
	}
}
