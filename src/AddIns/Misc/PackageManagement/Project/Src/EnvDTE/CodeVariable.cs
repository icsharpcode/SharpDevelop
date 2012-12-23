// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement, global::EnvDTE.CodeVariable
	{
		IField field;
		IDocumentLoader documentLoader;
		
		public CodeVariable()
		{
		}
		
		public CodeVariable(IField field)
			: this(field, new DocumentLoader())
		{
		}
		
		public CodeVariable(IField field, IDocumentLoader documentLoader)
			: base(field)
		{
			this.field = field;
			this.documentLoader = documentLoader;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementVariable; }
		}
		
		public global::EnvDTE.vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public override global::EnvDTE.TextPoint GetStartPoint()
		{
			return new TextPoint(field.GetStartPosition(), documentLoader);
		}
		
		public override global::EnvDTE.TextPoint GetEndPoint()
		{
			return new TextPoint(field.GetEndPosition(), documentLoader);
		}
	}
}
