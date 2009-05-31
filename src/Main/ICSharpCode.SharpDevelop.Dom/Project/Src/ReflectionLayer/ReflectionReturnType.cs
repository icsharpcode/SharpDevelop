// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Dom.ReflectionLayer
{
	public static class ReflectionReturnType
	{
		public static bool IsDefaultType(Type type)
		{
			return !type.IsArray && !type.IsGenericType && !type.IsGenericParameter;
		}
		
		#region Parse Reflection Type Name
		public static IReturnType Parse(IProjectContent pc, string reflectionTypeName)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			using (var tokenizer = Tokenize(reflectionTypeName)) {
				tokenizer.MoveNext();
				IReturnType result = Parse(pc, tokenizer);
				if (tokenizer.Current != null)
					throw new ReflectionTypeNameSyntaxError("Expected end of type name, but found " + tokenizer.Current);
				return result;
			}
		}
		
		static IReturnType Parse(IProjectContent pc, IEnumerator<string> tokenizer)
		{
			string typeName = tokenizer.Current;
			if (typeName == null)
				throw new ReflectionTypeNameSyntaxError("Unexpected end of type name");
			tokenizer.MoveNext();
			int typeParameterCount;
			typeName = ReflectionClass.SplitTypeParameterCountFromReflectionName(typeName, out typeParameterCount);
			IReturnType rt = new GetClassReturnType(pc, typeName, typeParameterCount);
			if (tokenizer.Current == "[") {
				// this is a constructed type
				List<IReturnType> typeArguments = new List<IReturnType>();
				do {
					tokenizer.MoveNext();
					if (tokenizer.Current != "[")
						throw new ReflectionTypeNameSyntaxError("Expected '['");
					tokenizer.MoveNext();
					typeArguments.Add(Parse(pc, tokenizer));
					if (tokenizer.Current != "]")
						throw new ReflectionTypeNameSyntaxError("Expected ']' after generic argument");
					tokenizer.MoveNext();
				} while (tokenizer.Current == ",");
				if (tokenizer.Current != "]")
					throw new ReflectionTypeNameSyntaxError("Expected ']' after generic argument list");
				tokenizer.MoveNext();
				
				rt = new ConstructedReturnType(rt, typeArguments);
			}
			while (tokenizer.Current == ",") {
				tokenizer.MoveNext();
				string token = tokenizer.Current;
				if (token != null && token != "," && token != "[" && token != "]")
					tokenizer.MoveNext();
			}
			return rt;
		}
		
		static IEnumerator<string> Tokenize(string reflectionTypeName)
		{
			StringBuilder currentText = new StringBuilder();
			for (int i = 0; i < reflectionTypeName.Length; i++) {
				char c = reflectionTypeName[i];
				if (c == ',' || c == '[' || c == ']') {
					if (currentText.Length > 0) {
						yield return currentText.ToString();
						currentText.Length = 0;
					}
					yield return c.ToString();
				} else {
					currentText.Append(c);
				}
			}
			if (currentText.Length > 0)
				yield return currentText.ToString();
			yield return null;
		}
		#endregion
		
		public static IReturnType Create(IClass @class, Type type, bool createLazyReturnType)
		{
			return Create(@class.ProjectContent, @class, type, createLazyReturnType);
		}
		
		public static IReturnType Create(IMember member, Type type, bool createLazyReturnType)
		{
			return Create(member.DeclaringType.ProjectContent, member, type, createLazyReturnType);
		}
		
		public static IReturnType Create(IProjectContent pc, IEntity member, Type type, bool createLazyReturnType)
		{
			return Create(pc, member, type, createLazyReturnType, true);
		}
		
		/// <summary>
		/// Creates a IReturnType from the reflection type.
		/// </summary>
		/// <param name="pc">The project content used as context.</param>
		/// <param name="member">The member used as context (e.g. as GenericReturnType)</param>
		/// <param name="type">The reflection return type that should be converted</param>
		/// <param name="createLazyReturnType">Set this parameter to false to create a direct return type
		/// (without GetClassReturnType indirection) where possible</param>
		/// <param name="forceGenericType">Set this parameter to false to allow unbound generic types</param>
		/// <returns>The IReturnType</returns>
		public static IReturnType Create(IProjectContent pc, IEntity member, Type type, bool createLazyReturnType, bool forceGenericType)
		{
			if (type.IsByRef) {
				// TODO: Use ByRefRefReturnType
				return Create(pc, member, type.GetElementType(), createLazyReturnType);
			} else if (type.IsPointer) {
				return new PointerReturnType(Create(pc, member, type.GetElementType(), createLazyReturnType));
			} else if (type.IsArray) {
				return new ArrayReturnType(pc, Create(pc, member, type.GetElementType(), createLazyReturnType), type.GetArrayRank());
			} else if (type.IsGenericType && (forceGenericType || !type.IsGenericTypeDefinition)) {
				Type[] args = type.GetGenericArguments();
				List<IReturnType> para = new List<IReturnType>(args.Length);
				for (int i = 0; i < args.Length; ++i) {
					para.Add(Create(pc, member, args[i], createLazyReturnType));
				}
				return new ConstructedReturnType(Create(pc, member, type.GetGenericTypeDefinition(), createLazyReturnType, false), para);
			} else if (type.IsGenericParameter) {
				IClass c = (member is IClass) ? (IClass)member : (member is IMember) ? ((IMember)member).DeclaringType : null;
				if (c != null && type.GenericParameterPosition < c.TypeParameters.Count) {
					if (c.TypeParameters[type.GenericParameterPosition].Name == type.Name) {
						return new GenericReturnType(c.TypeParameters[type.GenericParameterPosition]);
					}
				}
				if (type.DeclaringMethod != null) {
					IMethod method = member as IMethod;
					if (method != null) {
						if (type.GenericParameterPosition < method.TypeParameters.Count) {
							return new GenericReturnType(method.TypeParameters[type.GenericParameterPosition]);
						}
						return new GenericReturnType(new DefaultTypeParameter(method, type));
					}
				}
				return new GenericReturnType(new DefaultTypeParameter(c, type));
			} else {
				string name = type.FullName;
				if (name == null)
					throw new ApplicationException("type.FullName returned null. Type: " + type.ToString());
				int typeParameterCount;
				name = ReflectionClass.ConvertReflectionNameToFullName(name, out typeParameterCount);
				if (!createLazyReturnType) {
					IClass c = pc.GetClass(name, typeParameterCount);
					if (c != null)
						return c.DefaultReturnType;
					// example where name is not found: pointers like System.Char*
					// or when the class is in a assembly that is not referenced
				}
				return new GetClassReturnType(pc, name, typeParameterCount);
			}
		}
	}
}
