// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ICodeGenerator.
	/// </summary>
	public interface ICodeGenerator
	{
		void AddAttribute(IEntity target, IAttribute attribute);
		void AddAssemblyAttribute(IProject targetProject, IAttribute attribute);
		void AddReturnTypeAttribute(IMethod target, IAttribute attribute);
		void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo);
		
		string GetPropertyName(string fieldName);
		string GetParameterName(string fieldName);
		string GetFieldName(string propertyName);
	}
	
	public class DefaultCodeGenerator : ICodeGenerator
	{
		public static readonly DefaultCodeGenerator DefaultInstance = new DefaultCodeGenerator();
		
		public virtual void AddAttribute(IEntity target, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddAssemblyAttribute(IProject targetProject, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddReturnTypeAttribute(IMethod target, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual string GetPropertyName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual string GetParameterName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToLower(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToLower(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToLower(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual string GetFieldName(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return propertyName;
			string newName = Char.ToLower(propertyName[0]) + propertyName.Substring(1);
			if (newName == propertyName)
				return "_" + newName;
			else
				return newName;
		}
	}
}
