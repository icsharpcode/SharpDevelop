// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass, global::EnvDTE.CodeClass2
	{
		public CodeClass2(CodeModelContext context, ITypeDefinition typeDefinition)
			: base(context, typeDefinition)
		{
		}
		
		public global::EnvDTE.CodeElements PartialClasses {
			get {
				var partialClasses = new CodeElementsList<CodeType>();
//				ITypeDefinition typeDefinition = typeModel.Resolve();
//				if (typeDefinition != null) {
//					foreach (string fileName in typeDefinition.Parts.Select(p => p.UnresolvedFile.FileName).Distinct()) {
//						CodeModelContext newContext = context.WithFilteredFileName(fileName);
//						list.Add(CodeType.Create(newContext, typeDefinition));
//					}
//				} else {
					partialClasses.Add(this);
//				}
				return partialClasses;
			}
		}
		
		public bool IsGeneric {
			get { return typeDefinition.FullTypeName.TypeParameterCount > 0; }
		}
		
		public global::EnvDTE.vsCMClassKind ClassKind {
			get {
				//if (typeDefinition.IsPartial) {
				//	return global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
				//}
				return global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass;
			}
			set {
//				if (value == this.ClassKind) {
//					return;
//				}
//				
//				if (value == global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass) {
//					ITypeDefinition typeDefinition = typeModel.Resolve();
//					if (typeDefinition == null) {
//						throw new NotSupportedException();
//					}
//					context.CodeGenerator.MakePartial(typeDefinition);
//				} else {
					throw new NotImplementedException();
//				}
			}
		}
		
		public bool IsAbstract {
			get { return typeDefinition.IsAbstract; }
		}
	}
}
