// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
//	[Flags]
//	public enum ShowMembers {
//		Public     = 1,
//		Protected  = 2,
//		Private    = 4,
//		Static     = 8
//	}
	
	public class ResolveResult
	{
		IClass type;
		ArrayList members;
		List<string> namespaces;
		
		public IClass Type {
			get {
				return type;
			}
		}
		
		public ArrayList Members {
			get {
				return members;
			}
		}
		
		public List<string> Namespaces {
			get {
				return namespaces;
			}
		}
		
		public ResolveResult(string[] namespaces) {
			this.namespaces = new List<string>();
			this.namespaces.AddRange(namespaces);
			members = new ArrayList();
		}
		
		public ResolveResult(string[] namespaces, ArrayList classes) {
			this.namespaces = new List<string>();
			this.namespaces.AddRange(namespaces);
			members = classes;
		}
		
		public ResolveResult(List<string> namespaces) {
			this.namespaces = namespaces;
			members = new ArrayList();
		}
		
		public ResolveResult(IClass type, ArrayList members) {
			this.type = type;
			this.members = members;
			namespaces = new List<string>();
		}
//		object[]    resolveContents;
//		ShowMembers showMembers;
//		
//		public bool ShowPublic {
//			get {
//				return (showMembers & ShowMembers.Public) == ShowMembers.Public;
//			}
//		}
//
//		public bool ShowProtected {
//			get {
//				return (showMembers & ShowMembers.Protected) == ShowMembers.Protected;
//			}
//		}
//		
//		public bool ShowPrivate {
//			get {
//				return (showMembers & ShowMembers.Private) == ShowMembers.Private;
//			}
//		}
//
//		public bool ShowStatic {
//			get {
//				return (showMembers & ShowMembers.Static) == ShowMembers.Static;
//			}
//		}
//		
//		public object[] ResolveContents {
//			get {
//				return resolveContents;
//			}
//		}
//		
//		public ShowMembers ShowMembers {
//			get {
//				return showMembers;
//			}
//		}
//		
//		public ResolveResult(object[] resolveContents, ShowMembers showMembers)
//		{
//			this.resolveContents = resolveContents;
//			this.showMembers     = showMembers;
//		}
	}
	
	public interface IParser {
		
		string[] LexerTags {
//// Alex - need to have get accessor too
			get;
			set;
		}
		
		IExpressionFinder ExpressionFinder {
			get;
		}
		
		/// <summary>
		/// Gets if the parser can parse the specified file.
		/// This method is used to get the correct parser for a specific file and normally decides based on the file
		/// extension.
		/// </summary>
		bool CanParse(string fileName);
		/// <summary>
		/// Gets if the parser can parse the specified project.
		/// Only when no parser for a project is found, the assembly is loaded.
		/// </summary>
		bool CanParse(IProject project);
		
		ICompilationUnitBase Parse(string fileName);
		ICompilationUnitBase Parse(string fileName, string fileContent);
		
		IResolver CreateResolver();
	}
}
