// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Static helper methods for reflection names.
	/// </summary>
	public static class ReflectionHelper
	{
		/// <summary>
		/// Retrieves a class.
		/// </summary>
		/// <returns>Returns the class; or null if it is not found.</returns>
		public static ITypeDefinition GetClass(this ITypeResolveContext context, Type type)
		{
			if (type.DeclaringType != null) {
				ITypeDefinition declaringType = GetClass(context, type.DeclaringType);
				if (declaringType != null) {
					int typeParameterCount;
					string name = SplitTypeParameterCountFromReflectionName(type.Name, out typeParameterCount);
					foreach (ITypeDefinition innerClass in declaringType.InnerClasses) {
						if (innerClass.Name == name && innerClass.TypeParameterCount == typeParameterCount) {
							return innerClass;
						}
					}
				}
				return null;
			} else {
				int typeParameterCount;
				string name = SplitTypeParameterCountFromReflectionName(type.FullName, out typeParameterCount);
				return context.GetClass(name, typeParameterCount, StringComparer.Ordinal);
			}
		}
		
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				return reflectionName;
			} else {
				return reflectionName.Substring(0, pos);
			}
		}
		
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				typeParameterCount = 0;
				return reflectionName;
			} else {
				string typeCount = reflectionName.Substring(pos + 1);
				if (int.TryParse(typeCount, out typeParameterCount))
					return reflectionName.Substring(0, pos);
				else
					return reflectionName;
			}
		}
	}
}
