// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.Core
{
	public class BreakpointBookmark : SDBookmark
	{
		Breakpoint breakpoint;
		
		public Breakpoint Breakpoint {
			get {
				return breakpoint;
			}
		}
		
		public BreakpointBookmark(Breakpoint breakpoint, string fileName, ICSharpCode.TextEditor.Document.IDocument document, int lineNumber) : base(fileName, document, lineNumber)
		{
			this.breakpoint = breakpoint;
		}
		
		public override void Draw(ICSharpCode.TextEditor.IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBreakpoint(g, p.Y, IsEnabled);
		}
	}
}

