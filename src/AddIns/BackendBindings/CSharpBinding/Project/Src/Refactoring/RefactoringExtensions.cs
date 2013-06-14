// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Some extensions related to refactoring features.
	/// </summary>
	public static class RefactoringExtensions
	{
		/// <summary>
		/// Checks whether a property is auto-implemented (has only "get; set;" definitions).
		/// </summary>
		/// <param name="property">Property to check</param>
		/// <returns>True if auto-implemented, false otherwise.</returns>
		public static bool IsAutoImplemented(this IProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			if (property.IsAbstract)
				return false;
			
			return property.CanGet && !property.Getter.HasBody && property.CanSet && !property.Setter.HasBody;
		}
	}
}
