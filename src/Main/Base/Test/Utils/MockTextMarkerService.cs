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
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Description of MockTextEditor.
	/// </summary>
	public class MockTextMarkerService : ITextMarkerService
	{
		public static IDocument CreateDocumentWithMockService()
		{
			var document = new TextDocument();
			var container = (IServiceContainer)document.ServiceProvider.GetService(typeof(IServiceContainer));
			container.AddService(typeof(ITextMarkerService), new MockTextMarkerService());
			
			return document;
		}
		
		List<ITextMarker> markers;
		
		public MockTextMarkerService()
		{
			this.markers = new List<ITextMarker>();
		}
		
		public System.Collections.Generic.IEnumerable<ITextMarker> TextMarkers {
			get {
				return this.markers;
			}
		}
		
		public ITextMarker Create(int startOffset, int length)
		{
			ITextMarker m = new MockTextMarker(this.markers, startOffset, startOffset + length, length);
			this.markers.Add(m);
			return m;
		}
		
		public void Remove(ITextMarker marker)
		{
			marker.Delete();
		}
		
		public void RemoveAll(Predicate<ITextMarker> predicate)
		{
			foreach (ITextMarker m in markers.ToArray()) {
				if (predicate(m))
					m.Delete();
			}
		}
		
		public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
		{
			return markers.Where(m => m.StartOffset <= offset && offset <= m.EndOffset);
		}
	}
}
