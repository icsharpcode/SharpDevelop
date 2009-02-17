// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
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
		public static IEnumerable<IClass> FindDerivedClasses(IClass baseClass, IEnumerable<IProjectContent> projectContents, bool directDerivationOnly)
		{
			HashSet<IClass> resultList = new HashSet<IClass>();
			FindDerivedClasses(resultList, baseClass, projectContents, directDerivationOnly);
			return resultList.OrderBy(c => c.FullyQualifiedName);
		}
		
		static void FindDerivedClasses(HashSet<IClass> resultList, IClass baseClass, IEnumerable<IProjectContent> projectContents, bool directDerivationOnly)
		{
			baseClass = baseClass.GetCompoundClass();
			string baseClassName = baseClass.Name;
			string baseClassFullName = baseClass.FullyQualifiedName;
			LoggingService.Debug("FindDerivedClasses for " + baseClassFullName);
			List<IClass> list = new List<IClass>();
			foreach (IProjectContent pc in projectContents) {
				if (pc != baseClass.ProjectContent && !pc.ReferencedContents.Contains(baseClass.ProjectContent)) {
					// only project contents referencing the content of the base class
					// can derive from the class
					continue;
				}
				AddDerivedClasses(pc, baseClass, baseClassName, baseClassFullName, pc.Classes, list);
			}
			if (directDerivationOnly) {
				resultList.AddRange(list);
			} else {
				foreach (IClass c in list) {
					if (resultList.Add(c)) {
						FindDerivedClasses(resultList, c, projectContents, directDerivationOnly);
					}
				}
			}
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
						if (possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName
						    && possibleBaseClass.TypeArgumentCount == baseClass.TypeParameters.Count)
						{
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
			if (member == null)
				throw new ArgumentNullException("member");
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
			} else if (entity is MethodGroupResolveResult) {
				IMethod method = (entity as MethodGroupResolveResult).GetMethodIfSingleOverload();
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
				if (progressMonitor != null) progressMonitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				if (progressMonitor != null) progressMonitor.ShowingDialog = false;
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
					progressMonitor.BeginTask("${res:SharpDevelop.Refactoring.FindingReferences}", files.Count, true);
				}
				#if DEBUG
				if (System.Windows.Forms.Control.ModifierKeys == DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl.DebugBreakModifiers) {
					System.Diagnostics.Debugger.Break();
				}
				#endif
				while (enumerator.MoveNext()) {
					if (progressMonitor != null) {
						progressMonitor.WorkDone = enumerator.Index;
						if (progressMonitor.IsCancelled) {
							return null;
						}
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
			TextFinder textFinder; // the class used to find the position to resolve
			if (member == null) {
				textFinder = parentClass.ProjectContent.Language.GetFindClassReferencesTextFinder(parentClass);
			} else {
				Debug.Assert(member.DeclaringType.GetCompoundClass() == parentClass.GetCompoundClass());
				textFinder = parentClass.ProjectContent.Language.GetFindMemberReferencesTextFinder(member);
			}
			
			// It is possible that a class or member does not have a name (when parsing incomplete class definitions)
			// - in that case, we cannot find references.
			if (textFinder == null) {
				return;
			}
			
			string fileContentForFinder = textFinder.PrepareInputText(fileContent);
			
			IExpressionFinder expressionFinder = null;
			TextFinderMatch match = new TextFinderMatch(-1, 0);
			
			while (true) {
				match = textFinder.Find(fileContentForFinder, match.Position + 1);
				if (match.Position < 0)
					break;
				
				if (expressionFinder == null) {
					expressionFinder = ParserService.GetExpressionFinder(fileName);
					if (expressionFinder == null) {
						// ignore file if we cannot get an expression finder
						return;
					}
				}
				ExpressionResult expr = expressionFinder.FindFullExpression(fileContent, match.ResolvePosition);
				if (expr.Expression != null) {
					Point position = GetPosition(fileContent, match.ResolvePosition);
				repeatResolve:
					// TODO: Optimize by re-using the same resolver if multiple expressions were
					// found in this file (the resolver should parse all methods at once)
					ResolveResult rr = ParserService.Resolve(expr, position.Y, position.X, fileName, fileContent);
					MemberResolveResult mrr = rr as MemberResolveResult;
					if (member != null) {
						// find reference to member
						if (rr != null && rr.IsReferenceTo(member)) {
							list.Add(new Reference(fileName, match.Position, match.Length, expr.Expression, rr));
						} else if (FixIndexerExpression(expressionFinder, ref expr, mrr)) {
							goto repeatResolve;
						}
					} else {
						// find reference to class
						if (rr != null && rr.IsReferenceTo(parentClass)) {
							list.Add(new Reference(fileName, match.Position, match.Length, expr.Expression, rr));
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
		static List<ProjectItem> GetPossibleFiles(IClass ownerClass, IEntity member)
		{
			List<ProjectItem> resultList = new List<ProjectItem>();
			if (ProjectService.OpenSolution == null) {
				foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string name = vc.PrimaryFileName;
					if (!string.IsNullOrEmpty(name) && ParserService.GetParser(name) != null) {
						FileProjectItem tempItem = new FileProjectItem(null, ItemType.Compile);
						tempItem.Include = name;
						resultList.Add(tempItem);
					}
				}
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
					if (item is FileProjectItem) {
						resultList.Add(item);
					}
				}
			}
		}
		#endregion
	}
}
