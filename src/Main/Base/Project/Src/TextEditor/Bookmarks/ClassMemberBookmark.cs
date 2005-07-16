// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
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
	public class ClassMemberBookmark : Bookmark
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
		
		public override void Click(Control parent, MouseEventArgs e)
		{
			MenuService.ShowContextMenu(this, "/SharpDevelop/ViewContent/DefaultTextEditor/ClassMemberContextMenu", parent, e.X, e.Y);
		}
		
		protected void DrawIcon(int iconIndex, Graphics g, Point p)
		{
			g.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[iconIndex], p);
		}
	}
	
	public class PropertyBookmark : ClassMemberBookmark
	{
		IProperty property;
		
		public PropertyBookmark(IDocument document, IProperty property) : base(document, property)
		{
			this.property = property;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			DrawIcon(ClassBrowserIconService.GetIcon(property), g, p);
		}
	}
	
	public class MethodBookmark : ClassMemberBookmark
	{
		IMethod method;
		
		public MethodBookmark(IDocument document, IMethod method) : base(document, method)
		{
			this.method = method;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			DrawIcon(ClassBrowserIconService.GetIcon(method), g, p);
		}
	}
	
	public class FieldBookmark : ClassMemberBookmark
	{
		IField field;
		
		public FieldBookmark(IDocument document, IField field) : base(document, field)
		{
			this.field = field;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			DrawIcon(ClassBrowserIconService.GetIcon(field), g, p);
		}
	}
	
	public class EventBookmark : ClassMemberBookmark
	{
		IEvent @event;
		
		public EventBookmark(IDocument document, IEvent @event) : base(document, @event)
		{
			this.@event = @event;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			DrawIcon(ClassBrowserIconService.GetIcon(@event), g, p);
		}
	}
}
