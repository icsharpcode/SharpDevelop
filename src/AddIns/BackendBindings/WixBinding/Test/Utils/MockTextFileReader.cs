// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockTextFileReader : ITextFileReader
	{
		public TextReader Create(string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
