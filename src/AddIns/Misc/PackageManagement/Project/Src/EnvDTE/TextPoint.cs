// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextPoint : MarshalByRefObject, global::EnvDTE.TextPoint
	{
		protected readonly string fileName;
		protected readonly TextLocation location;
		protected readonly IDocumentLoader documentLoader;
		
		internal TextPoint(string fileName, TextLocation location, IDocumentLoader documentLoader)
		{
			this.fileName = fileName;
			this.location = location;
			this.documentLoader = documentLoader;
		}
		
		public int LineCharOffset {
			get { return location.Column; }
		}
		
		public int Line {
			get { return location.Line; }
		}
		
		public global::EnvDTE.EditPoint CreateEditPoint()
		{
			return new EditPoint(fileName, location, documentLoader);
		}
		
		internal static TextPoint CreateStartPoint(CodeModelContext context, DomRegion region)
		{
			return new TextPoint(region.FileName, region.Begin, context.DocumentLoader);
		}
		
		internal static TextPoint CreateEndPoint(CodeModelContext context, DomRegion region)
		{
			return new TextPoint(region.FileName, region.End, context.DocumentLoader);
		}
	}
}
