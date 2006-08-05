// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultCompilationUnit : ICompilationUnit
	{
		public static readonly ICompilationUnit DummyCompilationUnit = new DefaultCompilationUnit(DefaultProjectContent.DummyProjectContent);
		
		List<IUsing> usings  = new List<IUsing>();
		List<IClass> classes = new List<IClass>();
		List<IAttribute> attributes = new List<IAttribute>();
		List<FoldingRegion> foldingRegions = new List<FoldingRegion>();
		List<TagComment> tagComments = new List<TagComment>();
		
		bool errorsDuringCompile = false;
		object tag               = null;
		string fileName          = null;
		IProjectContent projectContent;
		
		/// <summary>
		/// Source code file this compilation unit was created from. For compiled are compiler-generated
		/// code, this property returns null.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public IProjectContent ProjectContent {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return projectContent;
			}
		}
		
		public bool ErrorsDuringCompile {
			get {
				return errorsDuringCompile;
			}
			set {
				errorsDuringCompile = value;
			}
		}
		
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public virtual List<IUsing> Usings {
			get {
				return usings;
			}
		}

		public virtual List<IAttribute> Attributes {
			get {
				return attributes;
			}
		}

		public virtual List<IClass> Classes {
			get {
				return classes;
			}
		}
		
		public List<FoldingRegion> FoldingRegions {
			get {
				return foldingRegions;
			}
		}

		public virtual List<IComment> MiscComments {
			get {
				return null;
			}
		}

		public virtual List<IComment> DokuComments {
			get {
				return null;
			}
		}

		public virtual List<TagComment> TagComments {
			get {
				return tagComments;
			}
		}
		
		public DefaultCompilationUnit(IProjectContent projectContent)
		{
			Debug.Assert(projectContent != null);
			this.projectContent = projectContent;
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			foreach (IClass c in Classes) {
				if (c != null && c.Region.IsInside(caretLine, caretColumn)) {
					return c.GetInnermostClass(caretLine, caretColumn);
				}
			}
			return null;
		}
		
		
		
		/// <summary>
		/// Returns all (nested) classes in which the caret currently is exept
		/// the innermost class, returns an empty collection if the caret is in 
		/// no class or only in the innermost class.
		/// Zhe most outer class is the last in the collection.
		/// </summary>
		public List<IClass> GetOuterClasses(int caretLine, int caretColumn)
		{
			List<IClass> classes = new List<IClass>();
			IClass innerMostClass = GetInnermostClass(caretLine, caretColumn);
			foreach (IClass c in Classes) {
				if (c != null && c.Region.IsInside(caretLine, caretColumn)) {
					if (c != innerMostClass) {
						GetOuterClasses(classes, c, caretLine, caretColumn);
						if (!classes.Contains(c)) {
							classes.Add(c);
						}
					}
					break;
				}
			}
			return classes;
		}
		
		void GetOuterClasses(List<IClass> classes, IClass curClass, int caretLine, int caretColumn)
		{
			if (curClass != null && curClass.InnerClasses.Count > 0) {
				IClass innerMostClass = GetInnermostClass(caretLine, caretColumn);
				foreach (IClass c in curClass.InnerClasses) {
					if (c != null && c.Region.IsInside(caretLine, caretColumn)) {
						if (c != innerMostClass) {
							GetOuterClasses(classes, c, caretLine, caretColumn);
							if (!classes.Contains(c)) {
								classes.Add(c);
							}
						}
						break;
					}
				}
			}
		}
		
		public override string ToString() {
			return String.Format("[CompilationUnit: classes = {0}, fileName = {1}]",
			                     classes.Count,
			                     fileName);
		}
		
	}
}
