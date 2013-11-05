// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)using System;
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

