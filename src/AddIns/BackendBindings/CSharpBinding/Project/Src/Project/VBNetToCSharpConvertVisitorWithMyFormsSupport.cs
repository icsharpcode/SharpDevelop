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

/*
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace CSharpBinding
{
	sealed class VBNetToCSharpConvertVisitorWithMyFormsSupport : VBNetToCSharpConvertVisitor
	{
		IClass vbMyFormsClass;
		
		public VBNetToCSharpConvertVisitorWithMyFormsSupport(IProjectContent pc, ParseInformation parseInfo, string rootNamespace)
			: base(pc, parseInfo)
		{
			this.NamespacePrefixToAdd = rootNamespace;
			if (string.IsNullOrEmpty(rootNamespace))
				vbMyFormsClass = CSharpMyNamespaceBuilder.FindMyFormsClass(pc, "My");
			else
				vbMyFormsClass = CSharpMyNamespaceBuilder.FindMyFormsClass(pc, rootNamespace + ".My");
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			base.VisitAssignmentExpression(assignmentExpression, data);
			
			if (vbMyFormsClass != null) {
				TypeResolveResult trr = Resolve(assignmentExpression.Right) as TypeResolveResult;
				if (trr != null && trr.ResolvedClass != null) {
					foreach (IProperty p in vbMyFormsClass.Properties) {
						if (p.ReturnType.FullyQualifiedName == trr.ResolvedClass.FullyQualifiedName) {
							assignmentExpression.Right = MakeFieldReferenceExpression("My.MyProject.Forms." + p.Name);
							break;
						}
					}
				}
			}
			
			return null;
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression fieldReferenceExpression, object data)
		{
			ResolveResult fieldRR = base.VisitMemberReferenceExpression(fieldReferenceExpression, data) as ResolveResult;
			
			if (vbMyFormsClass != null && IsReferenceToInstanceMember(fieldRR)) {
				TypeResolveResult trr = Resolve(fieldReferenceExpression.TargetObject) as TypeResolveResult;
				if (trr != null && trr.ResolvedClass != null) {
					foreach (IProperty p in vbMyFormsClass.Properties) {
						if (p.ReturnType.FullyQualifiedName == trr.ResolvedClass.FullyQualifiedName) {
							fieldReferenceExpression.TargetObject = MakeFieldReferenceExpression("My.MyProject.Forms." + p.Name);
						}
					}
				}
			}
			
			return null;
		}
	}
}
*/
