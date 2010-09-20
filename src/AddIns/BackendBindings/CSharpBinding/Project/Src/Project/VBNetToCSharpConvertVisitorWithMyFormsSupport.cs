// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
