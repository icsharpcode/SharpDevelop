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
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	/// <summary>
	/// File code model namespaces take the full name of the namespace that a class
	/// is inside. So for the FileCodeModelNamespace class the CodeNamespace.Name
	/// would be ICSharpCode.PackageManagement.EnvDTE.
	/// This differs from the CodeModel CodeNamespace which breaks up the namespaces into
	/// parts.
	/// </summary>
	public class FileCodeModelCodeNamespace : CodeNamespace
	{
		public FileCodeModelCodeNamespace(CodeModelContext context, string namespaceName)
			: base(context, namespaceName)
		{
		}
		
		public override string Name {
			get { return base.FullName; }
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject; }
		}
		
		CodeElementsList<CodeElement> members = new CodeElementsList<CodeElement>();
		
		public override global::EnvDTE.CodeElements Members {
			get { return members; }
		}
		
		internal void AddMember(CodeElement member)
		{
			members.Add(member);
		}
	}
}
