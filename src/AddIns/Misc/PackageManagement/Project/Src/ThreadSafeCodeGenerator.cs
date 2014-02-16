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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public class ThreadSafeCodeGenerator : ICodeGenerator
	{
		readonly CodeGenerator codeGenerator;
		readonly IMessageLoop mainThread;
		
		public ThreadSafeCodeGenerator(CodeGenerator codeGenerator)
		{
			this.codeGenerator = codeGenerator;
			this.mainThread = SD.MainThread;
		}
		
		public void AddImport(FileName fileName, string name)
		{
			InvokeIfRequired(() => codeGenerator.AddImport(fileName, name));
		}
		
		public void MakePartial(ITypeDefinition typeDefinition)
		{
			InvokeIfRequired(() => codeGenerator.MakePartial(typeDefinition));
		}
		
		void InvokeIfRequired(Action action)
		{
			mainThread.InvokeIfRequired(action);
		}
		
		public void AddFieldAtStart(ITypeDefinition typeDefinition, Accessibility accessibility, IType fieldType, string name)
		{
			InvokeIfRequired(() => codeGenerator.AddFieldAtStart(typeDefinition, accessibility, fieldType, name));
		}
		
		public void AddMethodAtStart(ITypeDefinition typeDefinition, Accessibility accessibility, IType returnType, string name)
		{
			InvokeIfRequired(() => codeGenerator.AddMethodAtStart(typeDefinition, accessibility, returnType, name));
		}
		
		public void MakeVirtual(IMember member)
		{
			InvokeIfRequired(() => codeGenerator.MakeVirtual(member));
		}
	}
}
