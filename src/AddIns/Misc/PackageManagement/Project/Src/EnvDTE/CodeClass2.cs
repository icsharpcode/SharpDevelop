// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass, global::EnvDTE.CodeClass2
	{
		public CodeClass2(CodeModelContext context, ITypeDefinitionModel typeModel)
			: base(context, typeModel)
		{
		}
		
		public global::EnvDTE.CodeElements PartialClasses {
			get {
				var list = new CodeElementsList<CodeType>();
				var td = typeModel.Resolve();
				if (td != null) {
					foreach (var fileName in td.Parts.Select(p => p.UnresolvedFile.FileName).Distinct()) {
						var newContext = context.WithFilteredFileName(fileName);
						list.Add(CodeType.Create(newContext, typeModel));
					}
				} else {
					list.Add(this);
				}
				return list;
			}
		}
		
		public bool IsGeneric {
			get { return typeModel.FullTypeName.TypeParameterCount > 0; }
		}
		
		public global::EnvDTE.vsCMClassKind ClassKind {
			get {
				if (typeModel.IsPartial) {
					return global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
				}
				return global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass;
			}
			set {
				if (value == this.ClassKind)
					return;
				if (value == global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass) {
					var td = typeModel.Resolve();
					if (td == null)
						throw new NotSupportedException();
					context.CodeGenerator.MakePartial(td);
				} else {
					throw new NotSupportedException();
				}
			}
		}
		
		public bool IsAbstract {
			get { return typeModel.IsAbstract; }
		}
	}
}
