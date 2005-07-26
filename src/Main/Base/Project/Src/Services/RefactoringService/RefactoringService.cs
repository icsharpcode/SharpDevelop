// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
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
		/// Finds all classes deriving from baseClass.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		/// <param name="projectContents">The project contents in which derived classes should be searched.</param>
		/// <param name="directDerivationOnly">If true, gets only the classes that derive directly from <paramref name="baseClass"/>.</param>
		public static List<IClass> FindDerivedClasses(IClass baseClass, IEnumerable<IProjectContent> projectContents, bool directDerivationOnly)
		{
			baseClass = FixClass(baseClass);
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
			if (!directDerivationOnly) {
				List<IClass> additional = new List<IClass>();
				foreach (IClass c in list) {
					additional.AddRange(FindDerivedClasses(c, projectContents, directDerivationOnly));
				}
				foreach (IClass c in additional) {
					if (!list.Contains(c))
						list.Add(c);
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
			return RunFindReferences(member.DeclaringType, member, progressMonitor);
		}
		
		/// <summary>
		/// Find all references to the specified class.
		/// </summary>
		public static List<Reference> FindReferences(IClass @class, IProgressMonitor progressMonitor)
		{
			return RunFindReferences(@class, null, progressMonitor);
		}
		
		static List<Reference> RunFindReferences(IClass ownerClass, IMember member, IProgressMonitor progressMonitor)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				MessageService.ShowMessage("Find references cannot be executed until all files have been parsed.");
				return null;
			}
			ownerClass = FixClass(ownerClass);
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
			string searchedText; // the text that is searched for
			
			if (member == null) {
				searchedText = parentClass.Name.ToLower();
			} else {
				// When looking for a member, the name of the parent class does not always exist
				// in the file where the member is accessed.
				// (examples: derived classes, partial classes)
				if (member is IMethod && ((IMethod)member).IsConstructor)
					searchedText = parentClass.Name.ToLower();
				else
					searchedText = member.Name.ToLower();
			}
			
			int pos = -1;
			IExpressionFinder expressionFinder = null;
			while ((pos = lowerFileContent.IndexOf(searchedText, pos + 1)) >= 0) {
				if (pos > 0 && char.IsLetterOrDigit(fileContent, pos - 1)) {
					continue; // memberName is not a whole word (a.SomeName cannot reference Name)
				}
				if (pos < fileContent.Length - searchedText.Length - 1
				    && char.IsLetterOrDigit(fileContent, pos + searchedText.Length))
				{
					continue; // memberName is not a whole word (a.Name2 cannot reference Name)
				}
				
				if (expressionFinder == null) {
					expressionFinder = ParserService.GetExpressionFinder(fileName);
				}
				ExpressionResult expr = expressionFinder.FindFullExpression(fileContent, pos + 1);
				if (expr.Expression != null) {
					Point position = GetPosition(fileContent, pos);
					// TODO: Optimize by re-using the same resolver if multiple expressions were
					// found in this file (the resolver should parse all methods at once)
					ResolveResult rr = ParserService.Resolve(expr, position.Y, position.X, fileName, fileContent);
					if (member != null) {
						if (IsReferenceToMember(member, rr)) {
							list.Add(new Reference(fileName, pos, searchedText.Length, expr.Expression, rr));
						}
					} else {
						MemberResolveResult mrr = rr as MemberResolveResult;
						if (mrr != null) {
							if (mrr.ResolvedMember is IMethod && ((IMethod)mrr.ResolvedMember).IsConstructor) {
								if (mrr.ResolvedMember.DeclaringType.FullyQualifiedName == parentClass.FullyQualifiedName) {
									list.Add(new Reference(fileName, pos, searchedText.Length, expr.Expression, rr));
								}
							}
						} else {
							if (rr is TypeResolveResult && rr.ResolvedType.FullyQualifiedName == parentClass.FullyQualifiedName) {
								list.Add(new Reference(fileName, pos, searchedText.Length, expr.Expression, rr));
							}
						}
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
		
		/// <summary>
		/// Gets the compound class if the class was partial.
		/// </summary>
		static IClass FixClass(IClass c)
		{
			return c.DefaultReturnType.GetUnderlyingClass();
		}
		
		public static List<string> GetFileNames(IClass c)
		{
			List<string> list = new List<string>();
			CompoundClass cc = c as CompoundClass;
			if (cc != null) {
				foreach (IClass part in cc.Parts) {
					string fileName = part.CompilationUnit.FileName;
					if (fileName != null)
						list.Add(fileName);
				}
			} else {
				string fileName = c.CompilationUnit.FileName;
				if (fileName != null)
					list.Add(fileName);
			}
			return list;
		}
		
		/// <summary>
		/// Gets the list of files that could have a reference to the specified class.
		/// </summary>
		static List<ProjectItem> GetPossibleFiles(IClass c)
		{
			if (c.DeclaringType != null) {
				return GetPossibleFiles(c.DeclaringType, c);
			}
			List<ProjectItem> resultList = new List<ProjectItem>();
			GetPossibleFilesInternal(resultList, c.ProjectContent, c.IsInternal);
			return resultList;
		}
		
		/// <summary>
		/// Gets the files of files that could have a reference to the <paramref name="member"/>
		/// int the <paramref name="ownerClass"/>.
		/// </summary>
		static List<ProjectItem> GetPossibleFiles(IClass ownerClass, IDecoration member)
		{
			if (member == null)
				return GetPossibleFiles(ownerClass);
			List<ProjectItem> resultList = new List<ProjectItem>();
			if (member.IsPrivate) {
				List<string> fileNames = GetFileNames(ownerClass);
				foreach (string fileName in fileNames) {
					foreach (IProject p in ProjectService.OpenSolution.Projects) {
						foreach (ProjectItem item in p.Items) {
							if (item.ItemType == ItemType.Compile) {
								if (FileUtility.IsEqualFileName(fileName, item.FileName)) {
									resultList.Add(item);
									return resultList;
								}
							}
						}
					}
				}
			}
			
			if (member.IsProtected) {
				// TODO: Optimize when member is protected
			}
			
			GetPossibleFilesInternal(resultList, ownerClass.ProjectContent, ownerClass.IsInternal || member.IsInternal && !member.IsProtected);
			return resultList;
		}
		
		static void GetPossibleFilesInternal(List<ProjectItem> resultList, IProjectContent ownerProjectContent, bool internalOnly)
		{
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				IProjectContent pc = ParserService.GetProjectContent(p);
				if (pc == null) continue;
				if (pc != ownerProjectContent) {
					if (internalOnly) {
						// internal = can be only referenced from same project content
						continue;
					}
					if (!pc.HasReferenceTo(ownerProjectContent)) {
						// unreferences project contents cannot reference the class
						continue;
					}
				}
				foreach (ProjectItem item in p.Items) {
					if (item.ItemType == ItemType.Compile) {
						resultList.Add(item);
					}
				}
			}
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
