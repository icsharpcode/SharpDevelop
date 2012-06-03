// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction : CodeElement
	{
		IMethodOrProperty method;
		
		public CodeFunction(IMethod method)
			: base(method)
		{
			this.method = method;
		}
		
		public CodeFunction()
		{
		}
		
		public CodeFunction(IProperty property)
			: base(property)
		{
		}
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementFunction; }
		}
		
		public virtual vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public override TextPoint GetStartPoint()
		{
			return TextPoint.CreateStartPoint(method.Region);
		}
		
		public override TextPoint GetEndPoint()
		{
			return TextPoint.CreateEndPoint(method.BodyRegion);
		}
	}
}
