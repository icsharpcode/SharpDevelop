// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonClassFields : PythonWalker
	{
		FunctionDefinition functionDefinition;
		IClass declaringType;
		List<string> fieldNamesAdded;
		
		public PythonClassFields(FunctionDefinition functionDefinition)
		{
			this.functionDefinition = functionDefinition;
		}
		
		public void AddFields(IClass declaringType)
		{
			this.declaringType = declaringType;
			fieldNamesAdded = new List<string>();
			
			functionDefinition.Body.Walk(this);
		}
		
		public override bool Walk(AssignmentStatement node)
		{
			string fieldName = GetFieldName(node);
			AddFieldToDeclaringType(fieldName);
			return false;
		}
		
		string GetFieldName(AssignmentStatement node)
		{
			string[] memberNames = PythonControlFieldExpression.GetMemberNames(node.Left[0] as MemberExpression);
			return GetFieldName(memberNames);
		}
		
		string GetFieldName(string[] memberNames)
		{
			if (memberNames.Length > 1) {
				if (PythonSelfResolver.IsSelfExpression(memberNames[0])) {
					return memberNames[1];
				}
			}
			return null;
		}
		
		void AddFieldToDeclaringType(string fieldName)
		{
			if (fieldName != null) {
				if (!fieldNamesAdded.Contains(fieldName)) {
					DefaultField field = new DefaultField(declaringType, fieldName);
					declaringType.Fields.Add(field);
					fieldNamesAdded.Add(fieldName);
				}
			}
		}
	}
}
