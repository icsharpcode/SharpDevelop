// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
