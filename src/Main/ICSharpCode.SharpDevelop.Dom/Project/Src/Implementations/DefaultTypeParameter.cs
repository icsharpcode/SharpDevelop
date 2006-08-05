// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	public class DefaultTypeParameter : ITypeParameter
	{
		public static readonly IList<ITypeParameter> EmptyTypeParameterList = new List<ITypeParameter>().AsReadOnly();
		
		string name;
		IMethod method;
		IClass targetClass;
		int index;
		List<IReturnType> constraints = new List<IReturnType>();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public int Index {
			get {
				return index;
			}
		}
		
		public IMethod Method {
			get {
				return method;
			}
		}
		
		public IClass Class {
			get {
				return targetClass;
			}
		}
		
		public IList<IReturnType> Constraints {
			get {
				return constraints;
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				return DefaultAttribute.EmptyAttributeList;
			}
		}
		
		bool hasConstructableContraint = false;
		
		/// <summary>
		/// Gets if the type parameter has the 'new' constraint.
		/// </summary>
		public bool HasConstructableContraint {
			get {
				return hasConstructableContraint;
			}
			set {
				hasConstructableContraint = value;
			}
		}
		
		
		public DefaultTypeParameter(IMethod method, string name, int index)
		{
			this.method = method;
			this.targetClass = method.DeclaringType;
			this.name = name;
			this.index = index;
		}
		
		public DefaultTypeParameter(IMethod method, Type type)
		{
			this.method = method;
			this.targetClass = method.DeclaringType;
			this.name = type.Name;
			this.index = type.GenericParameterPosition;
		}
		
		public DefaultTypeParameter(IClass targetClass, string name, int index)
		{
			this.targetClass = targetClass;
			this.name = name;
			this.index = index;
		}
		
		public DefaultTypeParameter(IClass targetClass, Type type)
		{
			this.targetClass = targetClass;
			this.name = type.Name;
			this.index = type.GenericParameterPosition;
		}
		
		public override bool Equals(object obj)
		{
			DefaultTypeParameter tp = obj as DefaultTypeParameter;
			if (tp == null) return false;
			if (tp.index != index) return false;
			if (tp.name != name) return false;
			if (tp.hasConstructableContraint != hasConstructableContraint) return false;
			if (tp.method != method) {
				if (tp.method == null || method == null) return false;
				if (tp.method.FullyQualifiedName == method.FullyQualifiedName) return false;
			} else {
				if (tp.targetClass.FullyQualifiedName == targetClass.FullyQualifiedName) return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: {1}]", GetType().Name, name);
		}
	}
}
