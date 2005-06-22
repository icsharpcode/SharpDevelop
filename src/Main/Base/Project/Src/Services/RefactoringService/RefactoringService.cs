// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
					int count = c.BaseTypes.Count;
					for (int i = 0; i < count; i++) {
						string baseType = c.BaseTypes[i];
						if (pc.Language.NameComparer.Equals(baseType, baseClassName) ||
						    pc.Language.NameComparer.Equals(baseType, baseClassFullName)) {
							IClass possibleBaseClass = c.GetBaseClass(i);
							if (possibleBaseClass != null &&
							    possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName) {
								list.Add(c);
							}
						}
					}
				}
			}
			return list;
		}
		#endregion
		
		#region FindReferences
		/// <summary>
		/// Find all references to the specified member.
		/// </summary>
		public static List<Reference> FindReferences(IMember member, IProgressMonitor progressMonitor)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				MessageService.ShowMessage("Find references cannot be executed until all files have been parsed.");
				return null;
			}
			IClass ownerClass = member.DeclaringType;
			List<ProjectItem> files = GetPossibleFiles(ownerClass, member);
			ParseableFileContentEnumerator enumerator = new ParseableFileContentEnumerator(files.ToArray());
			List<Reference> references = new List<Reference>();
			try {
				if (progressMonitor != null) {
					progressMonitor.BeginTask("Finding references...", files.Count);
				}
				while (enumerator.MoveNext()) {
					if (progressMonitor != null) {
						progressMonitor.WorkDone = enumerator.Index;
					}
					
					AddReferences(references, ownerClass, member, enumerator.CurrentFileName, enumerator.CurrentFileContent);
				}
			} finally {
				if (progressMonitor != null) {
					progressMonitor.Done();
				}
				enumerator.Dispose();
			}
			return references;
		}
		
		static void AddReferences(List<Reference> list, IClass parentClass, IMember member, string fileName, string fileContent)
		{
			string lowerFileContent = fileContent.ToLower();
			if (lowerFileContent.IndexOf(parentClass.Name.ToLower()) < 0) return;
			string lowerMemberName;
			if (member is IMethod && ((IMethod)member).IsConstructor)
				lowerMemberName = parentClass.Name.ToLower();
			else
				lowerMemberName = member.Name.ToLower();
			//Console.WriteLine(fileName + "    /    " + lowerMemberName);
			int pos = -1;
			IExpressionFinder expressionFinder = null;
			while ((pos = lowerFileContent.IndexOf(lowerMemberName, pos + 1)) >= 0) {
				if (expressionFinder == null) {
					expressionFinder = ParserService.GetExpressionFinder(fileName);
				}
				string expr = expressionFinder.FindFullExpression(fileContent, pos + 1);
				if (expr != null) {
					Point position = GetPosition(fileContent, pos);
					ResolveResult rr = ParserService.Resolve(expr, position.Y, position.X, fileName, fileContent);
					if (IsReferenceToMember(member, rr)) {
						list.Add(new Reference(fileName, pos, lowerMemberName.Length, expr, rr));
					}
				}
			}
		}
		
		static Point GetPosition(string fileContent, int pos)
		{
			int line = 1;
			int column = 0;
			for (int i = 0; i < pos; ++i) {
				if (fileContent[i] == '\n') {
					++line;
					column = 0;
				} else {
					++column;
				}
			}
			return new Point(column, line);
		}
		
		static List<ProjectItem> GetPossibleFiles(IClass ownerClass, IMember member)
		{
			// TODO: Optimize when member is not public
			List<ProjectItem> resultList = new List<ProjectItem>();
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				IProjectContent pc = ParserService.GetProjectContent(p);
				if (pc == null) continue;
				if (pc != ownerClass.ProjectContent && !pc.HasReferenceTo(ownerClass.ProjectContent)) {
					// unreferences project contents cannot reference the class
					continue;
				}
				foreach (ProjectItem item in p.Items) {
					if (item.ItemType == ItemType.Compile) {
						resultList.Add(item);
					}
				}
			}
			return resultList;
		}
		#endregion
		
		public static bool IsReferenceToMember(IMember member, ResolveResult rr)
		{
			MemberResolveResult mrr = rr as MemberResolveResult;
			if (mrr != null)
				return IsSimilarMember(mrr.ResolvedMember, member);
			else
				return false;
		}
		
		/// <summary>
		/// Gets if member1 is the same as member2 or if member1 overrides member2.
		/// </summary>
		public static bool IsSimilarMember(IMember member1, IMember member2)
		{
			do {
				if (IsSimilarMemberInternal(member1, member2))
					return true;
			} while ((member1 = FindBaseMember(member1)) != null);
			return false;
		}
		
		static bool IsSimilarMemberInternal(IMember member1, IMember member2)
		{
			if (member1 == member2)
				return true;
			if (member1 == null || member2 == null)
				return false;
			if (member1.FullyQualifiedName != member2.FullyQualifiedName)
				return false;
			if (member1.IsStatic != member2.IsStatic)
				return false;
			if (member1 is IMethod) {
				if (member2 is IMethod) {
					if (DiffUtility.Compare(((IMethod)member1).Parameters, ((IMethod)member2).Parameters) != 0)
						return false;
				} else {
					return false;
				}
			}
			if (member1 is IProperty) {
				if (member2 is IProperty) {
					if (DiffUtility.Compare(((IProperty)member1).Parameters, ((IProperty)member2).Parameters) != 0)
						return false;
				} else {
					return false;
				}
			}
			return true;
		}
		
		public static IMember FindSimilarMember(IClass type, IMember member)
		{
			if (member is IMethod) {
				IMethod parentMethod = (IMethod)member;
				foreach (IMethod m in type.Methods) {
					if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
						if (m.IsStatic == parentMethod.IsStatic) {
							if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
								return m;
							}
						}
					}
				}
			} else if (member is IProperty) {
				IProperty parentMethod = (IProperty)member;
				foreach (IProperty m in type.Properties) {
					if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
						if (m.IsStatic == parentMethod.IsStatic) {
							if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
								return m;
							}
						}
					}
				}
			}
			return null;
		}
		
		public static IMember FindBaseMember(IMember member)
		{
			IClass parentClass = member.DeclaringType;
			IClass baseClass = parentClass.BaseClass;
			if (baseClass == null) return null;
			
			foreach (IClass childClass in baseClass.ClassInheritanceTree) {
				IMember m = FindSimilarMember(childClass, member);
				if (m != null)
					return m;
			}
			return null;
		}
	}
}
