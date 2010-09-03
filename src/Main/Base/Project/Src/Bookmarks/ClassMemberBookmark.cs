// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Bookmark used to give additional operations for class members.
	/// Does not derive from SDBookmark because it is not stored in the central BookmarkManager,
	/// but only in the document's BookmarkManager.
	/// </summary>
	public class ClassMemberBookmark : IBookmark
	{
		IMember member;
		
		public IMember Member {
			get {
				return member;
			}
		}
		
		public ClassMemberBookmark(IMember member)
		{
			this.member = member;
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassMemberContextMenu";
		
		public virtual IImage Image {
			get { return ClassBrowserIconService.GetIcon(member); }
		}
		
		public int LineNumber {
			get { return member.Region.BeginLine; }
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) {
				var f = AnalyticsMonitorService.TrackFeature("ICSharpCode.SharpDevelop.Bookmarks.ClassMemberBookmark.ShowContextMenu");
				var ctx = MenuService.ShowContextMenu(e.Source as UIElement, this, ContextMenuPath);
				ctx.Closed += delegate { f.EndTracking(); };
				e.Handled = true;
			}
		}
	}
	
	public class ClassBookmark : IBookmark
	{
		IClass @class;
		
		public IClass Class {
			get {
				return @class;
			}
			set {
				@class = value;
			}
		}
		
		public ClassBookmark(IClass @class)
		{
			this.@class = @class;
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassBookmarkContextMenu";
		
		public virtual IImage Image {
			get {
				return ClassBrowserIconService.GetIcon(@class);
			}
		}
		
		public int LineNumber {
			get { return @class.Region.BeginLine; }
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) {
				var f = AnalyticsMonitorService.TrackFeature("ICSharpCode.SharpDevelop.Bookmarks.ClassBookmark.ShowContextMenu");
				var ctx = MenuService.ShowContextMenu(e.Source as UIElement, this, ContextMenuPath);
				ctx.Closed += delegate { f.EndTracking(); };
				e.Handled = true;
			}
		}
	}
}
