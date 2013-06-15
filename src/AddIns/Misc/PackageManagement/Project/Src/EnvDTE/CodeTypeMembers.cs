// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeMembers : CodeElementsList
	{
		public CodeTypeMembers(IModelCollection<IMemberModel> members)
		{
			foreach (var m in members) {
				var e = CreateMember(m);
				if (e != null)
					base.AddCodeElement(e);
			}
			// TODO track collection changes
			//members.CollectionChanged += members_CollectionChanged;
		}

		CodeElement CreateMember(IMemberModel m)
		{
			switch (m.SymbolKind) {
				case SymbolKind.Field:
//					return new CodeVariable(m);
					throw new NotImplementedException();
				case SymbolKind.Property:
				case SymbolKind.Indexer:
//					return new CodeProperty2(m);
					throw new NotImplementedException();
				case SymbolKind.Event:
					return null; // events are not supported in EnvDTE?
				case SymbolKind.Method:
				case SymbolKind.Operator:
				case SymbolKind.Constructor:
				case SymbolKind.Destructor:
//					return new CodeFunction2(m);
					throw new NotImplementedException();
				default:
					throw new NotSupportedException("Invalid value for SymbolKind");
			}
		}
	}
}
