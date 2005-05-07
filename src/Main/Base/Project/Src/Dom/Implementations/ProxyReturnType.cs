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
	/// <summary>
	/// Base class for return types that wrap around other return types.
	/// </summary>
	[Serializable]
	public abstract class ProxyReturnType : IReturnType
	{
		public abstract IReturnType BaseType {
			get;
		}
		
		public virtual string FullyQualifiedName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.FullyQualifiedName : "?";
			}
		}
		
		public virtual string Name {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.Name : "?";
			}
		}
		
		public virtual string Namespace {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.Namespace : "?";
			}
		}
		
		public virtual string DotNetName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.DotNetName : "?";
			}
		}
		
		/// <summary>
		/// Gets the array ranks of the return type.
		/// When the return type is not an array, this property returns null.
		/// </summary>
		public virtual int ArrayDimensions {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.ArrayDimensions : 0;
			}
		}
		
		public virtual List<IMethod> GetMethods()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetMethods() : new List<IMethod>();
		}
		
		public virtual List<IProperty> GetProperties()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetProperties() : new List<IProperty>();
		}
		
		public virtual List<IField> GetFields()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetFields() : new List<IField>();
		}
		
		public virtual List<IEvent> GetEvents()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetEvents() : new List<IEvent>();
		}
		
		public virtual List<IIndexer> GetIndexers()
		{
			IReturnType baseType = BaseType;
			return (baseType != null) ? baseType.GetIndexers() : new List<IIndexer>();
		}
	}
}
