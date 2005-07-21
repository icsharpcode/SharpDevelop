// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	public class DefaultTypeParameter : ITypeParameter
	{
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
			foreach (Type constraint in type.GetGenericParameterConstraints()) {
				constraints.Add(ReflectionReturnType.Create(targetClass.ProjectContent, constraint, false));
			}
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
			foreach (Type constraint in type.GetGenericParameterConstraints()) {
				constraints.Add(ReflectionReturnType.Create(targetClass.ProjectContent, constraint, false));
			}
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: {1}]", GetType().Name, name);
		}
	}
}
