// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IClass : IDecoration
	{
		string FullyQualifiedName {
			get;
			set;
		}
		
		/// <summary>
		/// The default return type to use for this class.
		/// </summary>
		IReturnType DefaultReturnType { get; }
		
		/// <summary>
		/// The fully qualified name in the internal .NET notation (with `1 for generic types)
		/// </summary>
		string DotNetName {
			get;
		}
		
		string Name {
			get;
		}

		string Namespace {
			get;
		}
		
		ClassType ClassType {
			get;
		}
		
		/// <summary>
		/// The project content in which this class is defined.
		/// </summary>
		IProjectContent ProjectContent {
			get;
		}
		
		ICompilationUnit CompilationUnit {
			get;
		}
		
		DomRegion Region {
			get;
		}
		
		List<IReturnType> BaseTypes {
			get;
		}
		
		/// <summary>Gets the class associated with the base type with the same index.</summary>
		IReturnType GetBaseType(int index);
		
		List<IClass> InnerClasses {
			get;
		}

		List<IField> Fields {
			get;
		}

		List<IProperty> Properties {
			get;
		}

		List<IMethod> Methods {
			get;
		}

		List<IEvent> Events {
			get;
		}
		
		IList<ITypeParameter> TypeParameters {
			get;
		}

		IEnumerable<IClass> ClassInheritanceTree {
			get;
		}
		
		IClass BaseClass {
			get;
		}
		
		IReturnType BaseType {
			get;
		}
		
		/// <summary>
		/// If this is a partial class, gets the compound class containing information from all parts.
		/// If this is not a partial class, a reference to this class is returned.
		/// </summary>
		IClass GetCompoundClass();
		
		IClass GetInnermostClass(int caretLine, int caretColumn);
		
		List<IClass> GetAccessibleTypes(IClass callingClass);
		
		/// <summary>
		/// Searches the member with the specified name. Returns the first member/overload found.
		/// </summary>
		IMember SearchMember(string memberName, LanguageProperties language);
		
		/// <summary>Return true if the specified class is a base class of this class; otherwise return false.</summary>
		/// <remarks>Returns false when possibleBaseClass is null.</remarks>
		bool IsTypeInInheritanceTree(IClass possibleBaseClass);
		
		bool HasPublicOrInternalStaticMembers {
			get;
		}
		bool HasExtensionMethods {
			get;
		}
	}
}
