// 
// TypeScriptResolvecontenxt.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptResolveContext : ITypeResolveContext
	{
		DefaultResolvedTypeDefinition currentTypeDefinition;
		TypeScriptUnresolvedTypeDefinition unresolvedtypeDefinition;
		
		public TypeScriptResolveContext(TypeScriptUnresolvedTypeDefinition unresolvedtypeDefinition)
		{
			this.unresolvedtypeDefinition = unresolvedtypeDefinition;
		}
		
		public IAssembly CurrentAssembly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITypeDefinition CurrentTypeDefinition {
			get {
				if (currentTypeDefinition == null) {
					currentTypeDefinition = GetCurrentTypeDefinition();
				}
				return currentTypeDefinition;
			}
		}
		
		DefaultResolvedTypeDefinition GetCurrentTypeDefinition()
		{
			return new DefaultResolvedTypeDefinition(this, unresolvedtypeDefinition);
		}
		
		public IMember CurrentMember {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICompilation Compilation {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			throw new NotImplementedException();
		}
		
		public ITypeResolveContext WithCurrentMember(IMember member)
		{
			throw new NotImplementedException();
		}
	}
}
