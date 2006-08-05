// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
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

		/// <summary>
		/// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
		/// members can be overridden, too; if they are already overriding a method.
		/// </summary>
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
		/// <summary>
		/// Gets if the member can be overridden. Returns true when the member is "virtual" or "override" but not "sealed".
		/// </summary>
		bool IsOverridable {
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
