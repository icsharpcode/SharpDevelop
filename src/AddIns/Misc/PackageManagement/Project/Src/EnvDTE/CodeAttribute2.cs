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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute2 : CodeAttribute, global::EnvDTE.CodeAttribute2
	{
		public CodeAttribute2()
		{
		}
		
		public CodeAttribute2(CodeModelContext context, IAttribute attribute)
			: base(context, attribute)
		{
		}
		
		public virtual global::EnvDTE.CodeElements Arguments {
			get { 
				var list = new CodeElementsList<CodeAttributeArgument>();
				foreach (var arg in attribute.PositionalArguments) {
					list.Add(new CodeAttributeArgument(string.Empty, arg));
				}
				foreach (var arg in attribute.NamedArguments) {
					list.Add(new CodeAttributeArgument(arg.Key.Name, arg.Value));
				}
				return list;
			}
		}
	}
}
