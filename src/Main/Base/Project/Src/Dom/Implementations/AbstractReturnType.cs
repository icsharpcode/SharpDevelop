// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractReturnType : System.MarshalByRefObject, IReturnType
	{
		protected int    pointerNestingLevel;
		protected int[]  arrayDimensions;
		protected object declaredin = null;
		string fullyQualifiedName = null;
//		int nameHashCode = -1;
		
		public virtual string FullyQualifiedName {
			get {
				if (fullyQualifiedName == null) {
					return String.Empty;
				}
				return fullyQualifiedName;
//				return (string)AbstractNamedEntity.fullyQualifiedNames[nameHashCode];
			}
			set {
				fullyQualifiedName = value;
//				nameHashCode = value.GetHashCode();
//				if (AbstractNamedEntity.fullyQualifiedNames[nameHashCode] == null) {
//					AbstractNamedEntity.fullyQualifiedNames[nameHashCode] = value;
//				}
			}
		}

		public virtual string Name {
			get {
				if (FullyQualifiedName == null) {
					return null;
				}
 				int index = FullyQualifiedName.LastIndexOf('.');
				return index < 0 ? FullyQualifiedName : FullyQualifiedName.Substring(index + 1);
//				string[] name = FullyQualifiedName.Split(new char[] {'.'});
//				return name[name.Length - 1];
			}
		}

		public virtual string Namespace {
			get {
				if (FullyQualifiedName == null) {
					return null;
				}
				int index = FullyQualifiedName.LastIndexOf('.');
				return index < 0 ? String.Empty : FullyQualifiedName.Substring(0, index);
			}
		}

		public virtual int PointerNestingLevel {
			get {
				return pointerNestingLevel;
			}
		}

		public int ArrayCount {
			get {
				return ArrayDimensions.Length;
			}
		}

		public virtual int[] ArrayDimensions {
			get {
				if (arrayDimensions == null) return new int[0];
				return arrayDimensions;
			}
		}

		public virtual int CompareTo(IReturnType value) {
			int cmp;
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			cmp = (PointerNestingLevel - value.PointerNestingLevel);
			if (cmp != 0) {
				return cmp;
			}
			
			return DiffUtility.Compare(ArrayDimensions, value.ArrayDimensions);
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IReturnType)value);
		}
		
		public virtual object DeclaredIn {
			get {
				return declaredin;
			}
		}
	}
	
}
