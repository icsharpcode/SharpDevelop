// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The SearchClassReturnType is used when only a part of the class name is known and the
	/// type can only be resolved on demand (the ConvertVisitor uses SearchClassReturnType's).
	/// </summary>
	public sealed class SearchClassReturnType : ProxyReturnType
	{
		IClass declaringClass;
		IProjectContent pc;
		int caretLine;
		int caretColumn;
		string name;
		string shortName;
		
		public SearchClassReturnType(IProjectContent projectContent, IClass declaringClass, int caretLine, int caretColumn, string name)
		{
			if (declaringClass == null)
				throw new ArgumentNullException("declaringClass");
			this.declaringClass = declaringClass;
			this.pc = projectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.name = name;
			int pos = name.LastIndexOf('.');
			if (pos < 0)
				shortName = name;
			else
				shortName = name.Substring(pos + 1);
		}
		
		public override bool Equals(object o)
		{
			SearchClassReturnType rt = o as SearchClassReturnType;
			if (rt == null) {
				IReturnType rt2 = o as IReturnType;
				if (rt2 != null && rt2.IsDefaultReturnType)
					return rt2.FullyQualifiedName == this.FullyQualifiedName;
				else
					return false;
			}
			if (declaringClass.FullyQualifiedName != rt.declaringClass.FullyQualifiedName) return false;
			return name == rt.name;
		}
		
		public override int GetHashCode()
		{
			return declaringClass.GetHashCode() ^ name.GetHashCode();
		}
		
		// TODO: Cache BaseType until a new CompilationUnit is generated (static counter in ParserService)
		bool isSearching;
		
		public override IReturnType BaseType {
			get {
				if (isSearching)
					return null;
				try {
					isSearching = true;
					return pc.SearchType(name, declaringClass, caretLine, caretColumn);
				} finally {
					isSearching = false;
				}
			}
		}
		
		public override string FullyQualifiedName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.FullyQualifiedName : name;
			}
		}
		
		public override string Name {
			get {
				return shortName;
			}
		}
		
		public override string Namespace {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.Namespace : "?";
			}
		}
		
		public override string DotNetName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.DotNetName : name;
			}
		}
		
		public override bool IsDefaultReturnType {
			get {
				return true;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[SearchClassReturnType: {0}]", name);
		}
	}
}
