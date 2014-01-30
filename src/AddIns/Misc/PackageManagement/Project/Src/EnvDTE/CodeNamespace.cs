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
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
	{
		readonly string fullName;
		INamespaceModel model;
		
		public CodeNamespace(CodeModelContext context, INamespaceModel model)
			: base(context, model)
		{
			this.model = model;
		}
		
		public CodeNamespace(CodeModelContext context, string fullName)
			: base(context)
		{
			this.fullName = fullName;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal; }
		}
		
		public string FullName {
			get { return fullName; }
		}
		
		CodeElementsList<CodeElement> members;
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				if (members == null) {
					if (model == null)
						throw new NotSupportedException();
					IModelCollection<CodeElement> namespaceMembers = model.ChildNamespaces.Select(ns => new CodeNamespace(context, ns));
					IModelCollection<CodeElement> typeMembers = model.Types.Select(td => CodeType.Create(context, td));
					members = namespaceMembers.Concat(typeMembers).AsCodeElements();
				}
				return members;
			}
		}
	}
}
