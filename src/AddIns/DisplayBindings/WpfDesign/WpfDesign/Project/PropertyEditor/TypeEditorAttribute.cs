// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Attribute to specify that the decorated class is a editor for properties with the specified
	/// return type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public sealed class TypeEditorAttribute : Attribute
	{
		readonly Type supportedPropertyType;
		
		/// <summary>
		/// Creates a new TypeEditorAttribute that specifies that the decorated class is a editor
		/// for properties with the return type "<paramref name="supportedPropertyType"/>".
		/// </summary>
		public TypeEditorAttribute(Type supportedPropertyType)
		{
			if (supportedPropertyType == null)
				throw new ArgumentNullException("supportedPropertyType");
			this.supportedPropertyType = supportedPropertyType;
		}
		
		/// <summary>
		/// Gets the supported property type.
		/// </summary>
		public Type SupportedPropertyType {
			get { return supportedPropertyType; }
		}
	}
}
