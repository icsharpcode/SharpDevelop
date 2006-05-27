// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
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
			baseClass = baseClass.GetCompoundClass();
			string baseClassName = baseClass.Name;
			string baseClassFullName = baseClass.FullyQualifiedName;
			List<IClass> list = new List<IClass>();
			foreach (IProjectContent pc in projectContents) {
				if (pc != baseClass.ProjectContent && !pc.ReferencedContents.Contains(baseClass.ProjectContent)) {
					// only project contents referencing the content of the base class
					// can derive from the class
					continue;
				}
				AddDerivedClasses(pc, baseClass, baseClassName, baseClassFullName, pc.Classes, list);
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
		
		static void AddDerivedClasses(IProjectContent pc, IClass baseClass, string baseClassName, string baseClassFullName,
		                              IEnumerable<IClass> classList, IList<IClass> resultList)
		{
			foreach (IClass c in classList) {
				AddDerivedClasses(pc, baseClass, baseClassName, baseClassFullName, c.InnerClasses, resultList);
				int count = c.BaseTypes.Count;
				for (int i = 0; i < count; i++) {
					string baseTypeName = c.BaseTypes[i].Name;
					if (pc.Language.NameComparer.Equals(baseTypeName, baseClassName) ||
					    pc.Language.NameComparer.Equals(baseTypeName, baseClassFullName)) {
						IReturnType possibleBaseClass = c.GetBaseType(i);
						if (possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName) {
							resultList.Add(c);
						}
					}
				}
			}
		}
		#endregion
		
		#region FindReferences
		/// <summary>
		/// Find all references to the specified member.
		/// </summary>
		public static List<Reference> FindReferences(IMember member, IProgressMonitor progressMonitor)
		{
			return RunFindReferences(member.DeclaringType, member, false, progressMonitor);
		}
		
		/// <summary>
		/// Find all references to the specified class.
		/// </summary>
		public static List<Reference> FindReferences(IClass @class, IProgressMonitor progressMonitor)
		{
			if (@class == null)
				throw new ArgumentNullException("class");
			return RunFindReferences(@class, null, false, progressMonitor);
		}
		
		/// <summary>
		/// Find all references to the resolved entity.
		/// </summary>
		public static List<Reference> FindReferences(ResolveResult entity, IProgressMonitor progressMonitor)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (entity is LocalResolveResult) {
				return RunFindReferences(entity.CallingClass, (entity as LocalResolveResult).Field, true, progressMonitor);
			} else if (entity is TypeResolveResult) {
				return FindReferences((entity as TypeResolveResult).ResolvedClass, progressMonitor);
			} else if (entity is MemberResolveResult) {
				return FindReferences((entity as MemberResolveResult).ResolvedMember, progressMonitor);
			} else if (entity is MethodResolveResult) {
				IMethod method = (entity as MethodResolveResult).GetMethodIfSingleOverload();
				if (method != null) {
					return FindReferences(method, progressMonitor);
				}
			} else if (entity is MixedResolveResult) {
				return FindReferences((entity as MixedResolveResult).PrimaryResult, progressMonitor);
			}
			return null;
		}
		
		/// <summary>
		/// This method can be used in three modes:
		/// 1. Find references to classes (parentClass = targetClass, member = null, isLocal = false)
		/// 2. Find references to members (parentClass = parent, member = member, isLocal = false)
		/// 3. Find references to local variables (parentClass = parent, member = local var as field, isLocal = true)
		/// </summary>
		static List<Reference> RunFindReferences(IClass ownerClass, IMember member,
		                                         bool isLocal,
		                                         IProgressMonitor progressMonitor)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				return null;
			}
			List<ProjectItem> files;
			if (isLocal) {
				files = new List<ProjectItem>();
				files.Add(FindItem(ownerClass.CompilationUnit.FileName));
			} else {
				ownerClass = ownerClass.GetCompoundClass();
				files = GetPossibleFiles(ownerClass, member);
			}
			ParseableFileContentEnumerator enumerator = new ParseableFileContentEnumerator(files.ToArray());
			List<Reference> references = new List<Reference>();
			try {
				if (progressMonitor != null) {
					progressMonitor.BeginTask("${res:SharpDevelop.Refactoring.FindingReferences}", files.Count);
				}
				#if DEBUG
				if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control) {
					System.Diagnostics.Debugger.Break();
				}
				#endif
				while (enumerator.MoveNext()) {
					if (progressMonitor != null) {
						progressMonitor.WorkDone = enumerator.Index;
					}
					
					AddReferences(references, ownerClass, member, isLocal, enumerator.CurrentFileName, enumerator.CurrentFileContent);
				}
			} finally {
				if (progressMonitor != null) {
					progressMonitor.Done();
				}
				enumerator.Dispose();
			}
			return references;
		}
		
		/// <summary>
		/// This method can be used in three modes (like RunFindReferences)
		/// </summary>
		static void AddReferences(List<Reference> list,
		                          IClass parentClass, IMember member,
		                          bool isLocal,
		                          string fileName, string fileContent)
		{
			string lowerFileContent = fileContent.ToLowerInvariant();
			string searchedText; // the text that is searched for
			bool searchingIndexer = false;
			
			if (member == null) {
				searchedText = parentClass.Name.ToLowerInvariant();
			} else {
				// When looking for a member, the name of the parent class does not always exist
				// in the file where the member is accessed.
				// (examples: derived classes, partial classes)
				if (member is IMethod && ((IMethod)member).IsConstructor)
					searchedText = parentClass.Name.ToLowerInvariant();
				else {
					if (member is IProperty && ((IProperty)member).IsIndexer) {
						searchingIndexer = true;
						searchedText = GetIndexerExpressionStartToken(fileName);
					} else {
						searchedText = member.Name.ToLowerInvariant();
					}
				}
			}
			
			int pos = -1;
			int exprPos;
			IExpressionFinder expressionFinder = null;
			while ((pos = lowerFileContent.IndexOf(searchedText, pos + 1)) >= 0) {
				if (!searchingIndexer) {
					if (pos > 0 && char.IsLetterOrDigit(fileContent, pos - 1)) {
						continue; // memberName is not a whole word (a.SomeName cannot reference Name)
					}
					if (pos < fileContent.Length - searchedText.Length - 1
					    && char.IsLetterOrDigit(fileContent, pos + searchedText.Length))
					{
						continue; // memberName is not a whole word (a.Name2 cannot reference Name)
					}
					exprPos = pos;
				} else {
					exprPos = pos-1;	// indexer expressions are found by resolving the part before the indexer
				}
				
				if (expressionFinder == null) {
					expressionFinder = ParserService.GetExpressionFinder(fileName);
				}
				ExpressionResult expr = expressionFinder.FindFullExpression(fileContent, exprPos);
				if (expr.Expression != null) {
					Point position = GetPosition(fileContent, exprPos);
				repeatResolve:
					// TODO: Optimize by re-using the same resolver if multiple expressions were
					// found in this file (the resolver should parse all methods at once)
					ResolveResult rr = ParserService.Resolve(expr, position.Y, position.X, fileName, fileContent);
					MemberResolveResult mrr = rr as MemberResolveResult;
					if (isLocal) {
						// find reference to local variable
						if (IsReferenceToLocalVariable(rr, member)) {
							list.Add(new Reference(fileName, pos, searchedText.Length, expr.Expression, rr));
						} else if (FixIndexerExpression(expressionFinder, ref expr, mrr)) {
							goto repeatResolve;
						}
					} else if (member != null) {
						// find reference to member
						if (IsReferenceToMember(member, rr)) {
							list.Add(new Reference(fileName, pos, searchedText.Length, expr.Expression, rr));
						} else if (FixIndexerExpression(expressionFinder, ref expr, mrr)) {
							goto repeatResolve;
						}
					} else {
						// find reference to class
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
		
		/// <summary>
		/// Makes the given ExpressionResult point to the underlying expression if
		/// the expression is an indexer expression.
		/// </summary>
		/// <returns><c>true</c>, if the expression was an indexer expression and has been changed, <c>false</c> otherwise.</returns>
		public static bool FixIndexerExpression(IExpressionFinder expressionFinder, ref ExpressionResult expr, MemberResolveResult mrr)
		{
			if (mrr != null && mrr.ResolvedMember is IProperty && ((IProperty)mrr.ResolvedMember).IsIndexer) {
				// we got an indexer call as expression ("objectList[0].ToString()[2]")
				// strip the index from the expression to resolve the underlying expression
				string newExpr = expressionFinder.RemoveLastPart(expr.Expression);
				if (newExpr.Length >= expr.Expression.Length) {
					throw new ApplicationException("new expression must be shorter than old expression");
				}
				expr.Expression = newExpr;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Determines the token that denotes a possible beginning of an indexer
		/// expression in the specified file.
		/// </summary>
		static string GetIndexerExpressionStartToken(string fileName)
		{
			if (fileName != null) {
				ParseInformation pi = ParserService.GetParseInformation(fileName);
				if (pi != null &&
				    pi.MostRecentCompilationUnit != null &&
				    pi.MostRecentCompilationUnit.ProjectContent != null &&
				    pi.MostRecentCompilationUnit.ProjectContent.Language != null) {
					return pi.MostRecentCompilationUnit.ProjectContent.Language.IndexerExpressionStartToken;
				}
			}
			LoggingService.Warn("RefactoringService: unable to determine the correct indexer expression start token for file '"+fileName+"'");
			return LanguageProperties.CSharp.IndexerExpressionStartToken;
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
		
		static List<string> GetFileNames(IClass c)
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
		/// Gets the files of files that could have a reference to the <paramref name="member"/>
		/// int the <paramref name="ownerClass"/>.
		/// </summary>
		static List<ProjectItem> GetPossibleFiles(IClass ownerClass, IDecoration member)
		{
			List<ProjectItem> resultList = new List<ProjectItem>();
			if (ProjectService.OpenSolution == null) {
				FileProjectItem tempItem = new FileProjectItem(null, ItemType.Compile);
				tempItem.Include = ownerClass.CompilationUnit.FileName;
				resultList.Add(tempItem);
				return resultList;
			}
			
			if (member == null) {
				// get files possibly referencing ownerClass
				while (ownerClass.DeclaringType != null) {
					// for nested classes, treat class as member
					member = ownerClass;
					ownerClass = ownerClass.DeclaringType;
				}
				if (member == null) {
					GetPossibleFilesInternal(resultList, ownerClass.ProjectContent, ownerClass.IsInternal);
					return resultList;
				}
			}
			if (member.IsPrivate) {
				List<string> fileNames = GetFileNames(ownerClass);
				foreach (string fileName in fileNames) {
					ProjectItem item = FindItem(fileName);
					if (item != null) resultList.Add(item);
				}
				return resultList;
			}
			
			if (member.IsProtected) {
				// TODO: Optimize when member is protected
			}
			
			GetPossibleFilesInternal(resultList, ownerClass.ProjectContent, ownerClass.IsInternal || member.IsInternal && !member.IsProtected);
			return resultList;
		}
		
		static ProjectItem FindItem(string fileName)
		{
			if (ProjectService.OpenSolution != null) {
				foreach (IProject p in ProjectService.OpenSolution.Projects) {
					foreach (ProjectItem item in p.Items) {
						if (FileUtility.IsEqualFileName(fileName, item.FileName)) {
							return item;
						}
					}
				}
			}
			FileProjectItem tempItem = new FileProjectItem(null, ItemType.Compile);
			tempItem.Include = fileName;
			return tempItem;
		}
		
		static void GetPossibleFilesInternal(List<ProjectItem> resultList, IProjectContent ownerProjectContent, bool internalOnly)
		{
			if (ProjectService.OpenSolution == null) {
				return;
			}
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				IProjectContent pc = ParserService.GetProjectContent(p);
				if (pc == null) continue;
				if (pc != ownerProjectContent) {
					if (internalOnly) {
						// internal = can be only referenced from same project content
						continue;
					}
					if (!pc.ReferencedContents.Contains(ownerProjectContent)) {
						// project contents that do not reference the owner's content cannot reference the member
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
		
		#region IsReferenceTo...
		public static bool IsReferenceToLocalVariable(ResolveResult rr, IMember variable)
		{
			LocalResolveResult local = rr as LocalResolveResult;
			if (local == null) {
				return false;
			} else {
				return local.Field.Region.BeginLine == variable.Region.BeginLine
					&& local.Field.Region.BeginColumn == variable.Region.BeginColumn;
			}
		}
		
		/// <summary>
		/// Gets if <paramref name="rr"/> is a reference to <paramref name="member"/>.
		/// </summary>
		public static bool IsReferenceToMember(IMember member, ResolveResult rr)
		{
			MemberResolveResult mrr = rr as MemberResolveResult;
			if (mrr != null) {
				return IsSimilarMember(mrr.ResolvedMember, member);
			} else if (rr is MethodResolveResult) {
				return IsSimilarMember((rr as MethodResolveResult).GetMethodIfSingleOverload(), member);
			} else {
				return false;
			}
		}
		#endregion
		
		#region IsSimilarMember / FindBaseMember
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
			if (member == null) return null;
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
		#endregion
	}
}
