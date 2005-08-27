// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public static class ReflectionReturnType
	{
		#region Primitive Types
		static IReturnType @object, @int, @string, @bool, type, @void, array, disposable, exception, @delegate;
		
		/// <summary>Gets a ReturnType describing System.Object.</summary>
		public static IReturnType Object {
			get {
				if (@object == null) {
					@object = CreatePrimitive(typeof(object));
				}
				return @object;
			}
		}
		/// <summary>Gets a ReturnType describing System.Int32.</summary>
		public static IReturnType Int {
			get {
				if (@int == null) {
					@int = CreatePrimitive(typeof(int));
				}
				return @int;
			}
		}
		/// <summary>Gets a ReturnType describing System.String.</summary>
		public static IReturnType String {
			get {
				if (@string == null) {
					@string = CreatePrimitive(typeof(string));
				}
				return @string;
			}
		}
		/// <summary>Gets a ReturnType describing System.Boolean.</summary>
		public static IReturnType Bool {
			get {
				if (@bool == null) {
					@bool = CreatePrimitive(typeof(bool));
				}
				return @bool;
			}
		}
		/// <summary>Gets a ReturnType describing System.Type.</summary>
		public static IReturnType Type {
			get {
				if (type == null) {
					type = CreatePrimitive(typeof(Type));
				}
				return type;
			}
		}
		/// <summary>Gets a ReturnType describing System.Void.</summary>
		public static IReturnType Void {
			get {
				if (@void == null) {
					@void = CreatePrimitive(typeof(void));
				}
				return @void;
			}
		}
		/// <summary>Gets a ReturnType describing System.Array.</summary>
		public static IReturnType Array {
			get {
				if (array == null) {
					array = CreatePrimitive(typeof(Array));
				}
				return array;
			}
		}
		/// <summary>Gets a ReturnType describing System.IDisposable.</summary>
		public static IReturnType Disposable {
			get {
				if (disposable == null) {
					disposable = CreatePrimitive(typeof(IDisposable));
				}
				return disposable;
			}
		}
		/// <summary>Gets a ReturnType describing System.IDisposable.</summary>
		public static IReturnType Exception {
			get {
				if (exception == null) {
					exception = CreatePrimitive(typeof(Exception));
				}
				return exception;
			}
		}
		
		/// <summary>Gets a ReturnType describing System.Delegate.</summary>
		public static IReturnType Delegate {
			get {
				if (@delegate == null) {
					@delegate = CreatePrimitive(typeof(Delegate));
				}
				return @delegate;
			}
		}
		
		/// <summary>
		/// Create a primitive return type.
		/// Allowed are ONLY simple classes from MsCorlib (no arrays/generics etc.)
		/// </summary>
		public static IReturnType CreatePrimitive(Type type)
		{
			return ProjectContentRegistry.Mscorlib.GetClass(type.FullName).DefaultReturnType;
		}
		#endregion
		
		public static IReturnType Create(IProjectContent content, Type type, bool createLazyReturnType)
		{
			if (type.IsArray) {
				return MakeArray(type, Create(content, type.GetElementType(), createLazyReturnType));
			} else {
				string name = type.FullName;
				if (name == null)
					return null;
				if (name.Length > 1) {
					if (name[name.Length - 1] == '&') {
						name = name.Substring(0, name.Length - 1);
					}
				}
				if (name.Length > 2) {
					if (name[name.Length - 2] == '`') {
						name = name.Substring(0, name.Length - 2);
					}
				}
				if (name.IndexOf('+') > 0) {
					name = name.Replace('+', '.');
				}
				if (!createLazyReturnType) {
					IClass c = content.GetClass(name);
					if (c != null)
						return c.DefaultReturnType;
					// example where name is not found: pointers like System.Char*
					// or when the class is in a assembly that is not referenced
				}
				return new GetClassReturnType(content, name);
			}
		}
		
		public static bool IsDefaultType(Type type)
		{
			return !type.IsArray && !type.IsGenericType && !type.IsGenericParameter;
		}
		
		public static IReturnType Create(IMember member, Type type, bool createLazyReturnType)
		{
			if (type.IsArray) {
				return MakeArray(type, Create(member, type.GetElementType(), createLazyReturnType));
			} else if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				Type[] args = type.GetGenericArguments();
				List<IReturnType> para = new List<IReturnType>(args.Length);
				for (int i = 0; i < args.Length; ++i) {
					para.Add(Create(member, args[i], createLazyReturnType));
				}
				return new ConstructedReturnType(Create(member, type.GetGenericTypeDefinition(), createLazyReturnType), para);
			} else if (type.IsGenericParameter) {
				IClass c = member.DeclaringType;
				if (type.GenericParameterPosition < c.TypeParameters.Count) {
					if (c.TypeParameters[type.GenericParameterPosition].Name == type.Name) {
						return new GenericReturnType(c.TypeParameters[type.GenericParameterPosition]);
					}
				}
				if (type.DeclaringMethod != null) {
					IMethod method = member as IMethod;
					if (method != null) {
						return new GenericReturnType(new DefaultTypeParameter(method, type));
					}
				}
				return new GenericReturnType(new DefaultTypeParameter(c, type));
			}
			return Create(member.DeclaringType.ProjectContent, type, createLazyReturnType);
		}
		
		static IReturnType MakeArray(Type type, IReturnType baseType)
		{
			return new ArrayReturnType(baseType, type.GetArrayRank());
		}
	}
}
