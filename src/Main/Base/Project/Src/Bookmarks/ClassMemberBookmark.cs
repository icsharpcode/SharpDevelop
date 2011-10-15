// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
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
		DocumentLine line;
		
		public IMember Member {
			get {
				return member;
			}
		}
		
		public ClassMemberBookmark(IMember member)
		{
			this.member = member;
		}
		
		public ClassMemberBookmark(IMember member, TextDocument document)
		{
			this.member = member;
			int lineNr = member.Region.BeginLine;
			if (document != null && lineNr > 0 && lineNr <= document.LineCount)
				this.line = document.GetLineByNumber(lineNr);
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassMemberContextMenu";
		
		public virtual IImage Image {
			get { return ClassBrowserIconService.GetIcon(member); }
		}
		
		public int LineNumber {
			get {
				if (line != null && !line.IsDeleted)
					return line.LineNumber;
				else
					return member.Region.BeginLine;
			}
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
	
	public class ClassBookmark : IBookmark
	{
		IClass @class;
		DocumentLine line;
		
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
		
		public ClassBookmark(IClass @class, TextDocument document)
		{
			this.@class = @class;
			int lineNr = @class.Region.BeginLine;
			if (document != null && lineNr > 0 && lineNr <= document.LineCount)
				this.line = document.GetLineByNumber(lineNr);
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassBookmarkContextMenu";
		
		public virtual IImage Image {
			get {
				return ClassBrowserIconService.GetIcon(@class);
			}
		}
		
		public int LineNumber {
			get {
				if (line != null && !line.IsDeleted)
					return line.LineNumber;
				else
					return @class.Region.BeginLine;
			}
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
