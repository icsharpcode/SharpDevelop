// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlResolver
	{
		ITypeResolveContext resolveContext;
		
		public XamlResolver(ICompilation compilation)
		{
			this.resolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);
		}
		
		public IType ResolveType(string @namespace, string type)
		{
			return XamlUnresolvedFile.CreateTypeReference(@namespace, type).Resolve(resolveContext);
		}
	}
}
