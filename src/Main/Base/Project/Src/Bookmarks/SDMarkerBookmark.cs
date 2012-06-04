// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions and has a text marker assigned to it.
	/// </summary>
	public abstract class SDMarkerBookmark : SDBookmark
	{
		public SDMarkerBookmark(FileName fileName, Location location) : base(fileName, location)
		{
			//SetMarker();
		}
		
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
