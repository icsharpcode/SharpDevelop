// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Context used to resolve lazy return types.
	/// </summary>
	public interface IResolveContext
	{
		IReturnType Resolve(object data);
	}
	
	/// <summary>
	/// The LazyReturnType is the most used return type:
	/// It is not bound to a class, but only resolved on demand.
	/// The LazyReturnType is nearly always used to point at a <see cref="DefaultReturnType"/>.
	/// </summary>
	public sealed class LazyReturnType : ProxyReturnType
	{
		IResolveContext context;
		object data;
		
		public LazyReturnType(IResolveContext context, object data)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (data == null)
				throw new ArgumentNullException("data");
			this.context = context;
			this.data = data;
		}
		
		public override bool Equals(object o)
		{
			LazyReturnType rt = o as LazyReturnType;
			if (rt == null) return false;
			if (!context.Equals(rt.context)) return false;
			return data.Equals(rt.data);
		}
		
		public override int GetHashCode()
		{
			return context.GetHashCode() ^ data.GetHashCode();
		}
		
		public override IReturnType BaseType {
			get {
				return context.Resolve(data);
			}
		}
		
		public override string ToString()
		{
			return String.Format("[LazyReturnType: context = {0}, data = {1}]",
			                     context,
			                     data);
		}
	}
	
	public class GetClassResolveContext : IResolveContext
	{
		IProjectContent content;
		
		public GetClassResolveContext(IProjectContent content)
		{
			this.content = content;
		}
		
		public IReturnType Resolve(object data)
		{
			IClass c = content.GetClass((string)data);
			return (c != null) ? c.DefaultReturnType : null;
		}
		
		public override bool Equals(object obj)
		{
			GetClassResolveContext b = obj as GetClassResolveContext;
			if (b == null) return false;
			return content == b.content;
		}
		
		public override int GetHashCode()
		{
			return content.GetHashCode();
		}
	}
	
	public class SearchClassResolveContext : IResolveContext
	{
		IClass declaringClass;
		int caretLine;
		int caretColumn;
		
		public SearchClassResolveContext(IClass declaringClass, int caretLine, int caretColumn)
		{
			this.declaringClass = declaringClass;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
		}
		
		public IReturnType Resolve(object data)
		{
			IClass c = declaringClass.ProjectContent.SearchType((string)data, declaringClass, caretLine, caretColumn);
			return (c != null) ? c.DefaultReturnType : null;
		}
		
		public override bool Equals(object obj)
		{
			SearchClassResolveContext b = obj as SearchClassResolveContext;
			if (b == null) return false;
			if (declaringClass != b.declaringClass) return false;
			if (caretLine != b.caretLine) return false;
			if (caretColumn != b.caretColumn) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			return declaringClass.GetHashCode() ^ caretLine ^ caretColumn;
		}
	}
}
