// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Default implementation for classes that wrap Navigational
	/// information for the <see cref="NavigationService"/>.
	/// </summary>
	public class DefaultNavigationPoint : INavigationPoint
	{
		string fileName;
		object data;
		
		#region constructor
		public DefaultNavigationPoint() : this(String.Empty, null) {}
		public DefaultNavigationPoint(string fileName) : this(fileName, null) {}
		public DefaultNavigationPoint(string fileName, object data)
		{
			this.fileName = fileName;
			this.data = data;
		}
		#endregion
		
		#region overrides
		public override string ToString()
		{
			return String.Format("[{0}: {1}]",
			                     this.GetType().Name,
			                     this.Description);
		}
		#endregion
		
		#region INavigationPoint implementation
		public virtual string FileName {
			get {
				return fileName;
			}
		}
		
		public virtual string Description {
			get {
				return String.Format("{0}: {1}", fileName, data);
			}
		}
		
		public virtual string FullDescription {
			get {
				return Description;
			}
		}
		
		public virtual string ToolTip {
			get {
				return Description;
			}
		}
//		public string TabName {
//			get {
//				return tabName;
//			}
//		}
		
		public virtual int Index {
			get {
				return 0;
			}
		}
		
		public object NavigationData {
			get {
				return data;
			}
			set {
				data = value;
			}
		}
				
		public virtual void JumpTo()
		{
			FileService.JumpToFilePosition(this.FileName, 0, 0);
		}
		
		public void FileNameChanged(string newName)
		{
			fileName = newName;
		}
		
		public virtual void ContentChanging(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}
		#endregion

		#region IComparable
		public virtual int CompareTo(object obj)
		{
			if (this.GetType() != obj.GetType()) {
				return this.GetType().Name.CompareTo(obj.GetType().Name);
			}
			DefaultNavigationPoint b = obj as DefaultNavigationPoint;
			return this.FileName.CompareTo(b.FileName);
		}
		
		#endregion
		
		#region Equality		

		public override bool Equals(object obj)
		{
			DefaultNavigationPoint b = obj as DefaultNavigationPoint;
			if (b == null) return false;
			return this.FileName == b.FileName;
		}
		
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode();
		}
		#endregion
	}
}

