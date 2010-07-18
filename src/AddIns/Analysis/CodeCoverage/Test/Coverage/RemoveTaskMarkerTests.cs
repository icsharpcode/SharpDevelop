// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class RemoveTaskMarkerTests
	{
		ITextMarkerService markerService;
		IDocument document;
		
		[SetUp]
		public void Init()
		{
			document = MockTextMarkerService.CreateDocumentWithMockService();
			markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			document.Text = 
				"{\r\n" +
				"    int count = 0;\r\n" +
				"}\r\n";
		}
		
		[Test]
		public void CodeCoverageHighlighterRemoveMarkersDoesNotThrowInvalidCastExceptionWhenOneMarkerTagIsTask()
		{
			ITextMarker textMarker = markerService.Create(0, 2);
			textMarker.Tag = new Task(null, String.Empty, 1, 1, TaskType.Error);
			
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			Assert.DoesNotThrow(delegate { highlighter.RemoveMarkers(document); });
		}
	}
}
