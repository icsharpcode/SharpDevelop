// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class ProxyReturnType : IReturnType
	{
		public abstract IReturnType BaseType {
			get;
		}
		
		public virtual string FullyQualifiedName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.FullyQualifiedName : null;
			}
		}
		
		public virtual string Name {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.Name : null;
			}
		}
		
		public virtual string Namespace {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.Namespace : null;
			}
		}
		
		public virtual string DotNetName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.DotNetName : null;
			}
		}
		
		/// <summary>
		/// Gets the array ranks of the return type.
		/// When the return type is not an array, this property returns null.
		/// </summary>
		public virtual int[] ArrayDimensions {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.ArrayDimensions : null;
			}
		}
		
		public List<IMethod> GetMethods()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetMethods() : new List<IMethod>();
		}
		
		public List<IProperty> GetProperties()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetProperties() : new List<IProperty>();
		}
		
		public List<IField> GetFields()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetFields() : new List<IField>();
		}
		
		public List<IEvent> GetEvents()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetEvents() : new List<IEvent>();
		}
		
		public List<IIndexer> GetIndexers()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetIndexers() : new List<IIndexer>();
		}
	}
}
