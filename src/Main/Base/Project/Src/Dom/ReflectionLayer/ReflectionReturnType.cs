// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionReturnType : AbstractReturnType
	{
		public ReflectionReturnType(Type type)
		{
			string fullyQualifiedName = type.FullName == null ? type.Name : type.FullName.Replace("+", ".").Trim('&');
			
			// base.FullyQualifiedName = fullyQualifiedName.TrimEnd('[', ']', ',', '*');
			for (int i = fullyQualifiedName.Length; i > 0; i--) {
				char c = fullyQualifiedName[i - 1];
				if (c != '[' && c != ']' && c != ',' && c != '*') {
					if (i < fullyQualifiedName.Length)
						fullyQualifiedName = fullyQualifiedName.Substring(0, i);
					break;
				}
			}
			base.FullyQualifiedName = fullyQualifiedName;

			SetPointerNestingLevel(type);
			SetArrayDimensions(type);
			if (arrays == null)
				arrayDimensions = new int[0];
			else
				arrayDimensions = (int[])arrays.ToArray(typeof(int));
		}
		
		ArrayList arrays = null;
		void SetArrayDimensions(Type type)
		{
			if (type.IsArray && type != typeof(Array)) {
				if (arrays == null)
					arrays = new ArrayList();
				arrays.Add(type.GetArrayRank());
				SetArrayDimensions(type.GetElementType());
			}
		}
		
		void SetPointerNestingLevel(Type type)
		{
			if (type.IsPointer) {
				SetPointerNestingLevel(type.GetElementType());
				++pointerNestingLevel;
			}
		}
	}
}
