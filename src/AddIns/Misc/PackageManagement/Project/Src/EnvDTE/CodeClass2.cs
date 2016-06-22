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
				partialClasses.Add(this);
				return partialClasses;
			}
		}
		
		public bool IsGeneric {
			get { return typeDefinition.FullTypeName.TypeParameterCount > 0; }
		}
		
		public global::EnvDTE.vsCMClassKind ClassKind {
			get {
				if (typeDefinition.Parts.First().IsPartial) {
					return global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
				}
				return global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass;
			}
			set {
				if (value == ClassKind) {
					return;
				}
				
				if (value == global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass) {
					context.CodeGenerator.MakePartial(typeDefinition);
				} else {
					throw new NotImplementedException();
				}
			}
		}
		
		public bool IsAbstract {
			get { return typeDefinition.IsAbstract; }
		}
	}
}
