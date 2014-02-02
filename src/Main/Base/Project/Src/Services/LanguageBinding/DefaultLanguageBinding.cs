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
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultLanguageBinding : ILanguageBinding
	{
		public static readonly DefaultLanguageBinding DefaultInstance = new DefaultLanguageBinding(true);
		
		protected readonly ServiceContainer container;
		
		DefaultLanguageBinding(bool isDefault)
		{
			if (isDefault) {
				this.container = new ServiceContainer();
				this.container.AddService(typeof(IFormattingStrategy), DefaultFormattingStrategy.DefaultInstance);
				this.container.AddService(typeof(IBracketSearcher), DefaultBracketSearcher.DefaultInstance);
				this.container.AddService(typeof(CodeGenerator), CodeGenerator.DummyCodeGenerator);
			} else {
				this.container = new ServiceContainer(DefaultInstance);
			}
		}
		
		public DefaultLanguageBinding()
			: this(false)
		{
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				return this.GetService<IFormattingStrategy>();
			}
		}
		
		public IBracketSearcher BracketSearcher {
			get {
				return this.GetService<IBracketSearcher>();
			}
		}
		
		public CodeGenerator CodeGenerator {
			get {
				return this.GetService<CodeGenerator>();
			}
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				return this.GetService<System.CodeDom.Compiler.CodeDomProvider>();
			}
		}
		
		public virtual ICodeCompletionBinding CreateCompletionBinding(FileName fileName, TextLocation currentLocation, ITextSource fileContent)
		{
			throw new NotSupportedException();
		}
		
		public object GetService(Type serviceType)
		{
			return container.GetService(serviceType);
		}
	}
}
