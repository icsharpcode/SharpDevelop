// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractCompilationUnit : ICompilationUnit
	{
		protected List<IUsing> usings = new List<IUsing>();
		protected List<IClass> classes = new List<IClass>();
		protected List<IAttributeSection> attributes = new List<IAttributeSection>();
		protected bool errorsDuringCompile = false;
		protected object tag               = null;
		protected List<FoldingRegion> foldingRegions = new List<FoldingRegion>();
		protected string fileName          = "";
		protected List<Tag> tagComments = new List<Tag>();
		IProjectContent projectContent;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				Debug.Assert(value != null);
				fileName = value;
			}
		}
		
		public IProjectContent ProjectContent {
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

		public virtual List<IAttributeSection> Attributes {
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

		public abstract List<IComment> MiscComments {
			get;
		}

		public abstract List<IComment> DokuComments {
			get;
		}

		public virtual List<Tag> TagComments {
			get {
				return tagComments;
			}
		}
		
		protected AbstractCompilationUnit(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			foreach (IClass c in Classes) {
				if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
					return c.GetInnermostClass(caretLine, caretColumn);
				}
			}
			return null;
		}
		
		
		
		/// <remarks>
		/// Returns all (nestet) classes in which the carret currently is exept
		/// the innermost class, returns an empty collection if the carret is in 
		/// no class or only in the innermost class.
		/// the most outer class is the last in the collection.
		/// </remarks>
		public List<IClass> GetOuterClasses(int caretLine, int caretColumn)
		{
			List<IClass> classes = new List<IClass>();
			IClass innerMostClass = GetInnermostClass(caretLine, caretColumn);
			foreach (IClass c in Classes) {
				if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
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
					if (c != null && c.Region != null && c.Region.IsInside(caretLine, caretColumn)) {
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
			return String.Format("[AbstractCompilationUnit: classes = {0}, fileName = {1}]",
			                     classes.Count,
			                     fileName);
		}
		
	}
}
