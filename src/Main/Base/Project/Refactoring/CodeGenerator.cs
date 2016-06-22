// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public virtual void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo, InsertEventHandlerBodyKind bodyKind = InsertEventHandlerBodyKind.ThrowNotImplementedException)
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
		
		public virtual string EscapeIdentifier(string identifier)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddField(ITypeDefinition declaringType, Accessibility accessibility, IType fieldType, string name)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddFieldAtStart(ITypeDefinition typeDefinition, Accessibility accessibility, IType fieldType, string name)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddMethodAtStart(ITypeDefinition declaringType, Accessibility accessibility, IType returnType, string name)
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
	
	public enum InsertEventHandlerBodyKind
	{
		Nothing,
		TodoComment,
		ThrowNotImplementedException
	}
}
