// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Bookmark used to give additional operations for class members.
	/// Does not derive from SDBookmark because it is not stored in the central BookmarkManager,
	/// but only in the document's BookmarkManager.
	/// </summary>
	public abstract class ClassMemberBookmark : Bookmark
	{
		IMember member;
		
		public IMember Member {
			get {
				return member;
			}
		}
		
		public ClassMemberBookmark(IDocument document, IMember member)
			: base(document, member.Region.BeginLine - 1)
		{
			this.member = member;
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassMemberContextMenu";
		
		public override void Click(Control parent, MouseEventArgs e)
		{
			MenuService.ShowContextMenu(this, ContextMenuPath, parent, e.X, e.Y);
		}
		
		public abstract int IconIndex {
			get;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			g.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[IconIndex], p);
		}
	}
	
	public class ClassBookmark : Bookmark
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
		
		public ClassBookmark(IDocument document, IClass @class)
			: base(document, Math.Max(@class.Region.BeginLine - 1, 0))
		{
			this.@class = @class;
		}
		
		public const string ContextMenuPath = "/SharpDevelop/ViewContent/DefaultTextEditor/ClassBookmarkContextMenu";
		
		public override void Click(Control parent, MouseEventArgs e)
		{
			MenuService.ShowContextMenu(this, ContextMenuPath, parent, e.X, e.Y);
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			g.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[ClassBrowserIconService.GetIcon(@class)], p);
		}
	}
	
	public class PropertyBookmark : ClassMemberBookmark
	{
		IProperty property;
		
		public PropertyBookmark(IDocument document, IProperty property) : base(document, property)
		{
			this.property = property;
		}
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(property); }
		}
	}
	
	public class MethodBookmark : ClassMemberBookmark
	{
		IMethod method;
		
		public MethodBookmark(IDocument document, IMethod method) : base(document, method)
		{
			this.method = method;
		}
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(method); }
		}
	}
	
	public class FieldBookmark : ClassMemberBookmark
	{
		IField field;
		
		public FieldBookmark(IDocument document, IField field) : base(document, field)
		{
			this.field = field;
		}
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(field); }
		}
	}
	
	public class EventBookmark : ClassMemberBookmark
	{
		IEvent @event;
		
		public EventBookmark(IDocument document, IEvent @event) : base(document, @event)
		{
			this.@event = @event;
		}
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(@event); }
		}
	}
}
