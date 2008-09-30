// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;

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
		int typeParameterCount;
		
		public SearchClassReturnType(IProjectContent projectContent, IClass declaringClass, int caretLine, int caretColumn, string name, int typeParameterCount)
		{
			if (declaringClass == null)
				throw new ArgumentNullException("declaringClass");
			this.declaringClass = declaringClass;
			this.pc = projectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.typeParameterCount = typeParameterCount;
			this.name = name;
			int pos = name.LastIndexOf('.');
			if (pos < 0)
				shortName = name;
			else
				shortName = name.Substring(pos + 1);
		}
		
		volatile IReturnType cachedBaseType;
		int isSearching; // 0=false, 1=true
		
		void ClearCachedBaseType()
		{
			cachedBaseType = null;
		}
		
		public override IReturnType BaseType {
			get {
				IReturnType type = cachedBaseType;
				if (type != null)
					return type;
				if (Interlocked.CompareExchange(ref isSearching, 1, 0) != 0)
					return null;
				try {
					type = pc.SearchType(new SearchTypeRequest(name, typeParameterCount, declaringClass, caretLine, caretColumn)).Result;
					cachedBaseType = type;
					if (type != null)
						DomCache.RegisterForClear(ClearCachedBaseType);
					return type;
				} finally {
					isSearching = 0;
				}
			}
		}
		
		public override string FullyQualifiedName {
			get {
				string tmp = base.FullyQualifiedName;
				if (tmp == "?") {
					return name;
				}
				return tmp;
			}
		}
		
		public override string Name {
			get {
				return shortName;
			}
		}
		
		public override string DotNetName {
			get {
				string tmp = base.DotNetName;
				if (tmp == "?") {
					return name;
				}
				return tmp;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[SearchClassReturnType: {0}]", name);
		}
	}
}
