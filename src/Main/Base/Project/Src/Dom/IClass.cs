// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IClass : IDecoration
	{
		string FullyQualifiedName {
			get;
		}
		
		string Name {
			get;
		}

		string Namespace {
			get;
		}
		
		ClassType ClassType {
			get;
		}		
		
		/// <summary>
		/// The project content in which this class is defined.
		/// </summary>
		IProjectContent ProjectContent {
			get;
		}
		
		ICompilationUnit CompilationUnit {
			get;
		}
		
		IRegion Region {
			get;
		}
		
		IRegion BodyRegion {
			get;
		}
		
		List<string> BaseTypes {
			get;
		}
		
		List<IClass> InnerClasses {
			get;
		}

		List<IField> Fields {
			get;
		}

		List<IProperty> Properties {
			get;
		}

		List<IIndexer> Indexer {
			get;
		}

		List<IMethod> Methods {
			get;
		}

		List<IEvent> Events {
			get;
		}

		IEnumerable ClassInheritanceTree {
			get;
		}
		
		object DeclaredIn {
			get;
		}
		
		IClass BaseClass {
			get;
		}
		
	}
}
