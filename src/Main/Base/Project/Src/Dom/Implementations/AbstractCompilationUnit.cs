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
		
		public override string ToString() {
			return String.Format("[AbstractCompilationUnit: classes = {0}, fileName = {1}]",
			                     classes.Count,
			                     fileName);
		}
		
	}
}
