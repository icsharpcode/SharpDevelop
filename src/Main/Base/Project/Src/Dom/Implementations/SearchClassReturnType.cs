// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		
		public override int TypeParameterCount {
			get {
				return typeParameterCount;
			}
		}
		
		public override bool Equals(object o)
		{
			SearchClassReturnType rt = o as SearchClassReturnType;
			if (rt != null) {
				if (name != rt.name)
					return false;
				if (declaringClass.FullyQualifiedName == rt.declaringClass.FullyQualifiedName
				    && typeParameterCount == rt.typeParameterCount
				    && caretLine == rt.caretLine
				    && caretColumn == rt.caretColumn)
				{
					return true;
				}
			}
			IReturnType rt2 = o as IReturnType;
			if (rt2 != null && rt2.IsDefaultReturnType)
				return rt2.FullyQualifiedName == this.FullyQualifiedName && rt2.TypeParameterCount == this.TypeParameterCount;
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return declaringClass.GetHashCode() ^ name.GetHashCode()
					^ (typeParameterCount << 16 + caretLine << 8 + caretColumn);
			}
		}
		
		// we need to use a static Dictionary as cache to provide a easy was to clear all cached
		// BaseTypes.
		// When the cached BaseTypes could not be cleared as soon as the parse information is updated
		// (in contrast to a check if the parse information was updated when the base type is needed
		// the next time), we can get a memory leak:
		// The cached type of a property in Class1 is Class2. Then Class2 is updated, but the property
		// in Class1 is not needed again -> the reference causes the GC to keep the old version
		// of Class2 in memory.
		// The solution is this static cache which is cleared when some parse information updates.
		// That way, there can never be any reference to an out-of-date class.
		static Dictionary<SearchClassReturnType, IReturnType> cache;
		
		static SearchClassReturnType()
		{
			cache = new Dictionary<SearchClassReturnType, IReturnType>(new ReferenceComparer());
			ParserService.ParserUpdateStepFinished += OnParserUpdateStepFinished;
		}
		
		class ReferenceComparer : IEqualityComparer<SearchClassReturnType>
		{
			public bool Equals(SearchClassReturnType x, SearchClassReturnType y)
			{
				return x == y; // don't use x.Equals(y) - Equals might cause a FullyQualifiedName lookup on its own
			}
			
			public int GetHashCode(SearchClassReturnType obj)
			{
				return obj.GetHashCode();
			}
		}
		
		static void OnParserUpdateStepFinished(object sender, ParserUpdateStepEventArgs e)
		{
			if (e.Updated) {
				// clear the cache completely when the information was updated
				lock (cache) {
					cache.Clear();
				}
			}
		}
		
		bool isSearching;
		
		public override IReturnType BaseType {
			get {
				if (isSearching)
					return null;
				IReturnType type;
				lock (cache) {
					if (cache.TryGetValue(this, out type))
						return type;
					try {
						isSearching = true;
						type = pc.SearchType(new SearchTypeRequest(name, typeParameterCount, declaringClass, caretLine, caretColumn)).Result;
						cache[this] = type;
						return type;
					} finally {
						isSearching = false;
					}
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
