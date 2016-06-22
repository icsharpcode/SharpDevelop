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
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Parser
{
	class UnknownCodeContext : ICodeContext
	{
		ITypeResolveContext context;
		
		public UnknownCodeContext(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			this.context = new SimpleTypeResolveContext(compilation.MainAssembly);
		}
		
		public UnknownCodeContext(ICompilation compilation, IUnresolvedFile file, TextLocation location)
			: this(compilation)
		{
			var curDef = file.GetInnermostTypeDefinition (location);
			if (curDef != null) {
				var resolvedDef = curDef.Resolve (context).GetDefinition ();
				if (resolvedDef != null) {
					context = context.WithCurrentTypeDefinition (resolvedDef);
					
					var curMember = resolvedDef.Members.FirstOrDefault (m => m.Region.FileName == file.FileName && m.Region.Begin <= location && location < m.BodyRegion.End);
					if (curMember != null)
						context = context.WithCurrentMember (curMember);
				}
			}
		}

		public ICompilation Compilation { 
			get { return context.Compilation; }
		}

		public IAssembly CurrentAssembly {
			get { return context.CurrentAssembly; }
		}

		public ITypeDefinition CurrentTypeDefinition {
			get { return context.CurrentTypeDefinition; }
		}

		public IMember CurrentMember {
			get { return context.CurrentMember; }
		}

		public IEnumerable<IVariable> LocalVariables {
			get { return EmptyList<IVariable>.Instance; }
		}

		public bool IsWithinLambdaExpression {
			get { return false; }
		}

		public ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return context.WithCurrentTypeDefinition(typeDefinition);
		}

		public ITypeResolveContext WithCurrentMember(IMember member)
		{
			return context.WithCurrentMember(member);
		}
	}
}


