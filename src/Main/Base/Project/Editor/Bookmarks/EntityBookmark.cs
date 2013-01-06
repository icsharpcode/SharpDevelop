// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// Bookmark used to give additional operations for entities.
	/// Does not derive from SDBookmark because it is not stored in the central BookmarkManager,
	/// but only in the document's BookmarkManager.
	/// </summary>
	public class EntityBookmark : IBookmark
	{
		IUnresolvedEntity entity;
		IDocumentLine line;
		
		public IUnresolvedEntity Entity {
			get {
				return entity;
			}
		}
		
		public EntityBookmark(IUnresolvedEntity entity, IDocument document)
		{
			this.entity = entity;
			int lineNr = entity.Region.BeginLine;
			if (document != null && lineNr > 0 && lineNr < document.LineCount) {
				this.line = document.GetLineByNumber(lineNr);
			}
		}
		
		public const string ContextMenuPath = "/SharpDevelop/EntityContextMenu";
		
		public virtual IImage Image {
			get { return ClassBrowserIconService.GetIcon(entity); }
		}
		
		public int LineNumber {
			get {
				if (line != null && !line.IsDeleted)
					return line.LineNumber;
				else
					return entity.Region.BeginLine;
			}
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) {
				var entityModel = entity.GetModel();
				if (entityModel == null) {
					SD.Log.Warn("Could not find model for entity");
				} else {
					var f = SD.AnalyticsMonitor.TrackFeature("ICSharpCode.SharpDevelop.Editor.Bookmarks.EntityBookmark.ShowContextMenu");
					var ctx = MenuService.ShowContextMenu(e.Source as UIElement, entityModel, ContextMenuPath);
					ctx.Closed += delegate { f.EndTracking(); };
					e.Handled = true;
				}
			}
		}
		
		public virtual void MouseUp(MouseButtonEventArgs e)
		{
		}
		
		int IBookmark.ZOrder {
			get { return -10; }
		}
		
		bool IBookmark.CanDragDrop {
			get { return false; }
		}
		
		void IBookmark.Drop(int lineNumber)
		{
			throw new NotSupportedException();
		}
	}
}
