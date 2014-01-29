// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class CodeGenerator
	{
		public static readonly CodeGenerator DummyCodeGenerator = new CodeGenerator();
		
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
		
		public virtual void AddField(ITypeDefinition declaringType, Accessibility accessibility, IType fieldType, string name)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void ChangeAccessibility(IEntity entity, Accessibility newAccessiblity)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void MakePartial(ITypeDefinition td)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void MakeVirtual(IMember member)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddImport(FileName fileName, string namespaceName)
		{
			throw new NotSupportedException("Feature not supported!");
		}
	}
}
