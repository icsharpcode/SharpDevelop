// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ParseInformation 
	{
		ICompilationUnitBase validCompilationUnit;
		ICompilationUnitBase dirtyCompilationUnit;
		
		public ICompilationUnitBase ValidCompilationUnit {
			get {
				return validCompilationUnit;
			}
			set {
				validCompilationUnit = value;
			}
		}
		
		public ICompilationUnitBase DirtyCompilationUnit {
			get {
				return dirtyCompilationUnit;
			}
			set {
				dirtyCompilationUnit = value;
			}
		}
		
		public ICompilationUnitBase BestCompilationUnit {
			get {
				return validCompilationUnit == null ? dirtyCompilationUnit : validCompilationUnit;
			}
		}
		
		public ICompilationUnitBase MostRecentCompilationUnit {
			get {
				return dirtyCompilationUnit == null ? validCompilationUnit : dirtyCompilationUnit;
			}
		}
	}
}
