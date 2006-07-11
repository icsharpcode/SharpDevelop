/*
 * Created by SharpDevelop.
 * User: David
 * Date: 23/04/2006
 * Time: 11:47 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Description of TextEditorNavigationPoint.
	/// </summary>
	public class TextEditorNavigationPoint : DefaultNavigationPoint
	{
		public TextEditorNavigationPoint(string fileName, int line, int col) :
			base(fileName, new Point(col, line)) {}
		
		public int Line {
			get {
				return ((Point)base.NavigationData).Y;
			}
				
		}

		public override string Description {
			get {
				return String.Format("{0}: {1}", this.FileName, this.Line);
			}
		}
		
		public override int Index {
			get {
				return this.Line;
			}
		}
		
		public override bool Equals(object obj)
		{
			TextEditorNavigationPoint b = obj as TextEditorNavigationPoint;
			if (b == null) return false;
			return b.FileName == this.FileName && b.Line == this.Line;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Line;
		}
		
		public override void JumpTo()
		{
			Point p = (Point)this.NavigationData;
			FileService.JumpToFilePosition(this.FileName, p.Y, p.X);
		}
	}
}
