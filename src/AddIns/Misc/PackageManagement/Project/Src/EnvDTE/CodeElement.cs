// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElement : MarshalByRefObject
	{
		public virtual string Name { get; set; }
		public virtual string Language { get; private set; }
		
		// default is vsCMPart.vsCMPartWholeWithAttributes
		public virtual TextPoint GetStartPoint()
		{
			throw new NotImplementedException();
		}
		
		public virtual TextPoint GetEndPoint()
		{
			throw new NotImplementedException();
		}
		
		public virtual vsCMInfoLocation InfoLocation { get; private set; }
		public virtual DTE DTE { get; private set; }
	}
}
