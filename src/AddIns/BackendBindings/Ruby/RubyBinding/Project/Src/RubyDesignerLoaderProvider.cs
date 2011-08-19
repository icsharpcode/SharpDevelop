// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.FormsDesigner;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class RubyDesignerLoaderProvider : IDesignerLoaderProviderWithViewContent
	{
		public RubyDesignerLoaderProvider()
		{
		}
		
		public IDesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new RubyDesignerLoader(generator as IScriptingDesignerGenerator);
		}	
		
		public FormsDesignerViewContent ViewContent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
