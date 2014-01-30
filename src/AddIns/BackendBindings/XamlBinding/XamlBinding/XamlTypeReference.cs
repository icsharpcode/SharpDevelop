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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
namespace ICSharpCode.XamlBinding
{
	public class XamlTypeReference : ITypeReference
	{
		string localName;
		string xmlNamespace;
		const string XmlnsDefinitionAttribute = "System.Windows.Markup.XmlnsDefinitionAttribute";
		
		public XamlTypeReference(string xmlNamespace, string localName)
		{
			this.localName = localName;
			this.xmlNamespace = xmlNamespace;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			foreach (var asm in context.Compilation.Assemblies) {
				foreach (var attr in asm.AssemblyAttributes.Where(a => a.AttributeType.FullName == XmlnsDefinitionAttribute && a.PositionalArguments.Count == 2)) {
					ConstantResolveResult crr = attr.PositionalArguments[0] as ConstantResolveResult;
					ConstantResolveResult crr2 = attr.PositionalArguments[1] as ConstantResolveResult;
					if (crr == null || crr2 == null)
						continue;
					string namespaceName = crr2.ConstantValue as string;
					if (xmlNamespace.Equals(crr.ConstantValue as string, StringComparison.OrdinalIgnoreCase) && namespaceName != null) {
						ITypeDefinition td = asm.GetTypeDefinition(namespaceName, localName, 0);
						if (td != null)
							return td;
					}
				}
			}
			return new UnknownType(null, localName, 0);
		}
	}
}
