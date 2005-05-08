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
		
		public override bool Equals(object o)
		{
			GetClassReturnType rt = o as GetClassReturnType;
			if (rt == null) return false;
			return fullName == rt.fullName;
		}
		
		public override int GetHashCode()
		{
			return content.GetHashCode() ^ fullName.GetHashCode();
		}
		
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
	
	/// <summary>
	/// The SearchClassReturnType is used when only a part of the class name is known and the
	/// type can only be resolved on demand (the ConvertVisitor uses SearchClassReturnType's).
	/// </summary>
	public sealed class SearchClassReturnType : ProxyReturnType
	{
		IClass declaringClass;
		int caretLine;
		int caretColumn;
		string name;
		string shortName;
		
		public SearchClassReturnType(IClass declaringClass, int caretLine, int caretColumn, string name)
		{
			this.declaringClass = declaringClass;
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
			if (rt == null) return false;
			if (declaringClass != rt.declaringClass) return false;
			return name == rt.name;
		}
		
		public override int GetHashCode()
		{
			return declaringClass.GetHashCode() ^ name.GetHashCode();
		}
		
		public override IReturnType BaseType {
			get {
				IClass c = declaringClass.ProjectContent.SearchType(name, declaringClass, caretLine, caretColumn);
				return (c != null) ? c.DefaultReturnType : null;
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
		
		public override string ToString()
		{
			return String.Format("[SearchClassReturnType: {0}]", name);
		}
	}
}
