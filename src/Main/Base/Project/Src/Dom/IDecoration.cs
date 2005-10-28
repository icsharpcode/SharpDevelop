// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IDecoration: IComparable
	{
		IClass DeclaringType {
			get;
		}
		
		ModifierEnum Modifiers {
			get;
			set;
		}
		
		IList<IAttribute> Attributes {
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
		
		bool IsConst {
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

		bool IsPartial {
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

		bool IsOverride {
			get;
		}
		
		bool IsNew {
			get;
		}
		bool IsSynthetic {
			get;
		}
		
		object UserData {
			get;
			set;
		}
		
		bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree);
		bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree);
	}
}
