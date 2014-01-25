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
