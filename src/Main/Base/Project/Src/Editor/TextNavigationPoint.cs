// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Description of TextNavigationPoint.
	/// </summary>
	public class TextNavigationPoint : DefaultNavigationPoint
	{
		const int THREASHOLD = 5;
		
		#region constructor
		public TextNavigationPoint() : this(String.Empty, 0, 0) {}
		public TextNavigationPoint(string fileName) : this(fileName, 0, 0) {}
		public TextNavigationPoint(string fileName, int lineNumber, int column) : this(fileName, lineNumber, column, String.Empty) {}
		public TextNavigationPoint(string fileName, int lineNumber, int column, string content) : base(fileName, new Point(column, lineNumber))
		{
			if (String.IsNullOrEmpty(content)) {
				this.content = String.Empty;
				return;
			}
			this.content = content.Trim();
		}
		#endregion
		
		// TODO: Navigation - eventually, we'll store a reference to the document
		//       itself so we can track filename changes, inserts (that affect
		//       line numbers), and dynamically retrieve the text at this.lineNumber
		//
		//       what happens to the doc reference when the document is closed?
		//
		string content;
		
		public int LineNumber {
			get {return ((Point)this.NavigationData).Y;}
		}
		
		public int Column {
			get {return ((Point)this.NavigationData).X;}
		}
		
		public override void JumpTo()
		{
			FileService.JumpToFilePosition(this.FileName,
			                               this.LineNumber,
			                               this.Column);
		}
		
		public override void ContentChanging(object sender, EventArgs e)
		{
			// TODO: Navigation - finish ContentChanging
//			if (e is DocumentEventArgs) {
//				DocumentEventArgs de = (DocumentEventArgs)e;
//				if (this.LineNumber >= 
//			}
		}
		
		#region IComparable
		public override int CompareTo(object obj)
		{
			int result = base.CompareTo(obj);
			if (0!=result) {
				return result;
			}
			TextNavigationPoint b = obj as TextNavigationPoint;
			if (this.LineNumber==b.LineNumber) {
				return 0;
			} else if (this.LineNumber>b.LineNumber) {
				return 1;
			} else {
				return -1;
			}
		}
		#endregion
		
		#region Equality
		public override bool Equals(object obj)
		{
			TextNavigationPoint b = obj as TextNavigationPoint;
			if (b==null) return false;
			return this.FileName.Equals(b.FileName)
				&& (Math.Abs(this.LineNumber - b.LineNumber)<=THREASHOLD);
		}
		
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode() ^ this.LineNumber.GetHashCode();
		}
		#endregion
		
		public override string Description {
			get {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                     "{0}: {1}",
				                     this.LineNumber,
				                     this.content);
			}
		}
		
		public override string FullDescription {
			get {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                     "{0} - {1}",
				                     Path.GetFileName(this.FileName),
				                     this.Description);
			}
		}
	}
}
