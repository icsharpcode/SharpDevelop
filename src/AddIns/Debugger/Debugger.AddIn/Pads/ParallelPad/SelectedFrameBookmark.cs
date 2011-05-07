// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public class SelectedFrameBookmark : SDMarkerBookmark
	{
		public static readonly IImage SelectedFrameImage = new ResourceServiceImage("Icons.48x48.CurrentFrame");
		
		public SelectedFrameBookmark(FileName fileName, Location location) : base(fileName, location)
		{
			this.IsVisibleInBookmarkPad = false;
		}
		
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override bool CanToggle {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override IImage Image {
			get {
				return SelectedFrameImage;
			}
		}
		
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="markerService"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		protected override ITextMarker CreateMarker(ITextMarkerService markerService)
		{
			IDocumentLine line = this.Document.GetLine(this.LineNumber);
			ITextMarker marker = markerService.Create(line.Offset, line.Length);
			marker.BackgroundColor = Color.FromRgb(162, 208, 80);
			marker.ForegroundColor = Colors.White;
			return marker;
		}
	}
}
