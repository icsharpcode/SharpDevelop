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
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty2 : CodeProperty, global::EnvDTE.CodeProperty2
	{
		public CodeProperty2()
		{
		}
		
		public CodeProperty2(CodeModelContext context, IProperty property)
			: base(context, property)
		{
		}
		
		public global::EnvDTE.vsCMPropertyKind ReadWrite { 
			get { return GetPropertyKind(); }
		}
		
		global::EnvDTE.vsCMPropertyKind GetPropertyKind()
		{
			if (property.CanSet && property.CanGet) {
				return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadWrite;
			} else if (property.CanSet) {
				return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindWriteOnly;
			}
			return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadOnly;
		}
		
		public global::EnvDTE.CodeElements Parameters {
			get {
				var parameters = new CodeElementsList<CodeElement>();
				parameters.AddRange(property.Parameters.Select(parameter => new CodeParameter2(context, parameter)));
				return parameters;
			}
		}
	}
}
