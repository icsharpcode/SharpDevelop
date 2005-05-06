// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Abstract return type for return types that are not a <see cref="ProxyReturnType"/>.
	/// </summary>
	[Serializable]
	public abstract class AbstractReturnType : IReturnType
	{
		public abstract List<IMethod>   GetMethods();
		public abstract List<IProperty> GetProperties();
		public abstract List<IField>    GetFields();
		public abstract List<IEvent>    GetEvents();
		public abstract List<IIndexer>  GetIndexers();
		
		public override bool Equals(object o)
		{
			AbstractReturnType rt = o as AbstractReturnType;
			if (rt == null) return false;
			return this.fullyQualifiedName == rt.fullyQualifiedName;
		}
		
		public override int GetHashCode()
		{
			return fullyQualifiedName.GetHashCode();
		}
		
		string fullyQualifiedName = null;
		
		public virtual string FullyQualifiedName {
			get {
				if (fullyQualifiedName == null) {
					return String.Empty;
				}
				return fullyQualifiedName;
			}
			set {
				fullyQualifiedName = value;
			}
		}
		
		public virtual string Name {
			get {
				if (FullyQualifiedName == null) {
					return null;
				}
				int index = FullyQualifiedName.LastIndexOf('.');
				return index < 0 ? FullyQualifiedName : FullyQualifiedName.Substring(index + 1);
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
		
		public virtual string DotNetName {
			get {
				return FullyQualifiedName;
			}
		}
		
		public int ArrayDimensions {
			get {
				return 0;
			}
		}
	}
}
