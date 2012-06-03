// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement
	{
		IField field;
		
		public CodeVariable()
		{
		}
		
		public CodeVariable(IField field)
			: base(field)
		{
			this.field = field;
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
			return TextPoint.CreateStartPoint(field.Region);
		}
		
		public override TextPoint GetEndPoint()
		{
			return TextPoint.CreateEndPoint(field.Region);
		}
	}
}
