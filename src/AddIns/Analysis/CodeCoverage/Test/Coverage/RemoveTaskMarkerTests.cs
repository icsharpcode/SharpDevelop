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
using System.IO;

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
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
			textMarker.Tag = new SDTask(null, String.Empty, 1, 1, TaskType.Error);
			
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			Assert.DoesNotThrow(delegate { highlighter.RemoveMarkers(document); });
		}
	}
}
