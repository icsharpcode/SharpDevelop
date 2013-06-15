// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement, global::EnvDTE.CodeVariable
	{
		readonly IFieldModel field;
		readonly IDocumentLoader documentLoader;
		
		public CodeVariable()
		{
		}
		
		public CodeVariable(IFieldModel field, IDocumentLoader documentLoader)
		{
			this.field = field;
			this.documentLoader = documentLoader;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementVariable; }
		}
		
		public global::EnvDTE.vsCMAccess Access {
			get { return field.Accessibility.ToAccess(); }
			set { field.Accessibility = value.ToAccessibility(); }
		}
		
		public override global::EnvDTE.TextPoint GetStartPoint()
		{
			return TextPoint.CreateStartPoint(field.Region, documentLoader);
		}
		
		public override global::EnvDTE.TextPoint GetEndPoint()
		{
			return TextPoint.CreateEndPoint(field.Region, documentLoader);
		}
	}
}
