// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Interface for reference to types (classes).
	/// Such a reference can be direct (DefaultReturnType), lazy (SearchClassReturnType) or
	/// returns types that stand for special references (e.g. ArrayReturnType)
	/// </summary>
	public interface IReturnType
	{
		/// <summary>
		/// Gets the fully qualified name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System.Int32" for int[]<br/>
		/// "System.Collections.Generic.List" for List&lt;string&gt;
		/// </returns>
		string FullyQualifiedName {
			get;
		}
		
		/// <summary>
		/// Gets the short name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "Int32" or "int" (depending how the return type was created) for int[]<br/>
		/// "List" for List&lt;string&gt;
		/// </returns>
		string Name {
			get;
		}
		
		/// <summary>
		/// Gets the namespace of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System" for int[]<br/>
		/// "System.Collections.Generic" for List&lt;string&gt;
		/// </returns>
		string Namespace {
			get;
		}
		
		/// <summary>
		/// Gets the full dotnet name of the return type. The DotnetName is used for the
		/// documentation tags.
		/// </summary>
		/// <returns>
		/// "System.Int[]" for int[]<br/>
		/// "System.Collections.Generic.List{System.String}" for List&lt;string&gt;
		/// </returns>
		string DotNetName {
			get;
		}
		
		/// <summary>
		/// Gets the array ranks of the return type.
		/// When the return type is not an array, this property returns 0.
		/// </summary>
		int ArrayDimensions {
			get;
		}
		
		/// <summary>
		/// Gets if the return type is a default type, i.e. no array, generic etc.
		/// </summary>
		/// <returns>
		/// True for SearchClassReturnType, GetClassReturnType and DefaultReturnType.<br/>
		/// False for ArrayReturnType, SpecificReturnType etc.
		/// </returns>
		bool IsDefaultReturnType {
			get;
		}
		
		/// <summary>
		/// Gets the underlying class of this return type.
		/// </summary>
		IClass GetUnderlyingClass();
		
		/// <summary>
		/// Gets all methods that can be called on this return type.
		/// </summary>
		List<IMethod>   GetMethods();
		
		/// <summary>
		/// Gets all properties that can be called on this return type.
		/// </summary>
		List<IProperty> GetProperties();
		
		/// <summary>
		/// Gets all fields that can be called on this return type.
		/// </summary>
		List<IField>    GetFields();
		
		/// <summary>
		/// Gets all events that can be called on this return type.
		/// </summary>
		List<IEvent>    GetEvents();
	}
}
