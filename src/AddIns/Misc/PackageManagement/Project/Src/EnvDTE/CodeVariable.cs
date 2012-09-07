// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement
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
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementVariable; }
		}
		
		public vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public override TextPoint GetStartPoint()
		{
			return new TextPoint(field.GetStartPosition(), documentLoader);
		}
		
		public override TextPoint GetEndPoint()
		{
			return new TextPoint(field.GetEndPosition(), documentLoader);
		}
	}
}
