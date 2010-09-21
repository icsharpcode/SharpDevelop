// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Utils
{
	public class RubyCodeDomSerializerHelper
	{
		public static RubyCodeDomSerializer CreateSerializer()
		{
			return new RubyCodeDomSerializer();
		}
	}
}
