// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IDecoration: IComparable
	{
		ModifierEnum Modifiers {
			get;
		}

		List<IAttributeSection> Attributes {
			get;
		}
		
		string Documentation {
			get;
		}

		bool IsAbstract {
			get;
		}

		bool IsSealed {
			get;
		}

		bool IsStatic {
			get;
		}

		bool IsVirtual {
			get;
		}

		bool IsPublic {
			get;
		}

		bool IsProtected {
			get;
		}

		bool IsPrivate {
			get;
		}


		bool IsInternal {
			get;
		}

		bool IsFinal {
			get;
		}

		bool IsSpecialName {
			get;
		}

		bool IsReadonly {
			get;
		}

		bool IsProtectedAndInternal {
			get;
		}

		bool IsProtectedOrInternal {
			get;
		}

		bool IsLiteral {
			get;
		}

		bool IsOverride {
			get;
		}
		
		bool IsNew {
			get;
		}
		
		object UserData {
			get;
			set;
		}
	}
}
