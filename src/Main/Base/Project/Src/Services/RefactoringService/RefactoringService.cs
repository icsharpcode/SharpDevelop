// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public static class RefactoringService
	{
		#region FindDerivedClasses
		/// <summary>
		/// Finds all classes deriving directly from baseClass.
		/// </summary>
		public static List<IClass> FindDerivedClasses(IClass baseClass, IEnumerable<IProjectContent> projectContents)
		{
			string baseClassName = baseClass.Name;
			string baseClassFullName = baseClass.FullyQualifiedName;
			List<IClass> list = new List<IClass>();
			foreach (IProjectContent pc in projectContents) {
				if (pc != baseClass.ProjectContent && !pc.HasReferenceTo(baseClass.ProjectContent)) {
					// only project contents referencing the content of the base class
					// can derive from the class
					continue;
				}
				foreach (IClass c in pc.Classes) {
					if (c.BaseTypes.Count == 0) continue;
					string baseType = c.BaseTypes[0];
					if (pc.Language.NameComparer.Equals(baseType, baseClassName) ||
					    pc.Language.NameComparer.Equals(baseType, baseClassFullName)) {
						if (c.BaseClass.FullyQualifiedName == baseClass.FullyQualifiedName) {
							list.Add(c);
						}
					}
				}
			}
			return list;
		}
		#endregion
	}
}
