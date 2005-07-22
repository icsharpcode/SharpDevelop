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
	/// The GetClassReturnType is used when the class should be resolved on demand, but the
	/// full name is already known. Example: ReflectionReturnType
	/// </summary>
	public sealed class GetClassReturnType : ProxyReturnType
	{
		IProjectContent content;
		string fullName;
		string shortName;
		
		public GetClassReturnType(IProjectContent content, string fullName)
		{
			this.content = content;
			this.fullName = fullName;
			int pos = fullName.LastIndexOf('.');
			if (pos < 0)
				shortName = fullName;
			else
				shortName = fullName.Substring(pos + 1);
		}
		
		public override bool IsDefaultReturnType {
			get {
				return true;
			}
		}
		
		public override bool Equals(object o)
		{
			GetClassReturnType rt = o as GetClassReturnType;
			if (rt == null) {
				IReturnType rt2 = o as IReturnType;
				if (rt2 != null && rt2.IsDefaultReturnType)
					return rt2.FullyQualifiedName == fullName;
				else
					return false;
			}
			return fullName == rt.fullName;
		}
		
		public override int GetHashCode()
		{
			return content.GetHashCode() ^ fullName.GetHashCode();
		}
		
		// TODO: Cache BaseType until a new CompilationUnit is generated (static counter in ParserService)
		public override IReturnType BaseType {
			get {
				IClass c = content.GetClass(fullName);
				return (c != null) ? c.DefaultReturnType : null;
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return fullName;
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
				return (baseType != null) ? baseType.Namespace : fullName.Substring(0, fullName.LastIndexOf('.'));
			}
		}
		
		public override string DotNetName {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.DotNetName : fullName;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[GetClassReturnType: {0}]", fullName);
		}
	}
}
