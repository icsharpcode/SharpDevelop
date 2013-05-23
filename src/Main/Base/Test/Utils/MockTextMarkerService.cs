// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
