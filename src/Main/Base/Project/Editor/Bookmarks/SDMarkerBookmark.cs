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
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions and has a text marker assigned to it.
	/// </summary>
	public abstract class SDMarkerBookmark : SDBookmark
	{
		ITextMarker marker;
		
		protected abstract ITextMarker CreateMarker(ITextMarkerService markerService);
		
		public void SetMarker()
		{
			RemoveMarker();
			if (this.Document != null) {
				ITextMarkerService markerService = this.Document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
				if (markerService != null) {
					marker = CreateMarker(markerService);
				}
			}
		}
		
		protected override void OnDocumentChanged(EventArgs e)
		{
			base.OnDocumentChanged(e);
			SetMarker();
		}
		
		public virtual void RemoveMarker()
		{
			if (marker != null) {
				marker.Delete();
				marker = null;
			}
		}
	}
}
