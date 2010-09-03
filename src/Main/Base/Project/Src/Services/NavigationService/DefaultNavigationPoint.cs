// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;

namespace ICSharpCode.SharpDevelop
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
			this.fileName = fileName == null ? String.Empty : fileName;
			this.data = data;
		}
		#endregion
		
		#region overrides
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture,
			                     "[{0}: {1}]",
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
				return String.Format(CultureInfo.CurrentCulture,
				                     "{0}: {1}", fileName, data);
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
			fileName = newName == null ? String.Empty : newName;
		}
		
		public virtual void ContentChanging(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}
		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			DefaultNavigationPoint b = obj as DefaultNavigationPoint;
			if (object.ReferenceEquals(b, null)) return false;
			return this.FileName == b.FileName;
		}
		
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode();
		}
		#endregion

		#region IComparable
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return 1;
			if (this.GetType() != obj.GetType()) {
				// if of different types, sort the types by name
				return this.GetType().Name.CompareTo(obj.GetType().Name);
			}
			DefaultNavigationPoint b = obj as DefaultNavigationPoint;
			return this.FileName.CompareTo(b.FileName);
		}

		// Omitting any of the following operator overloads
		// violates rule: OverrideMethodsOnComparableTypes.
		public static bool operator == (DefaultNavigationPoint p1, DefaultNavigationPoint p2)
		{
			return object.Equals(p1, p2); // checks for null and calls p1.Equals(p2)
		}
		public static bool operator != (DefaultNavigationPoint p1, DefaultNavigationPoint p2)
		{
			return !(p1==p2);
		}
		public static bool operator < (DefaultNavigationPoint p1, DefaultNavigationPoint p2)
		{
			return p1==null ? p2!=null : (p1.CompareTo(p2) < 0);
		}
		public static bool operator > (DefaultNavigationPoint p1, DefaultNavigationPoint p2)
		{
			return p1==null ? false : (p1.CompareTo(p2) > 0);
		}
		#endregion
		
	}
}
