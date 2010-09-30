// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class PythonCodeDomSerializerHelper
	{
		public static PythonCodeDomSerializer CreateSerializer()
		{
			return new PythonCodeDomSerializer();
		}
	}
}
