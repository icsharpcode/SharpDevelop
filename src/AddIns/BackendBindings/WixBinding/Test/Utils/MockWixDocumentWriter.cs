// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockWixDocumentWriter : IWixDocumentWriter
	{
		public MockWixDocumentWriter()
		{
		}
		
		public void Write(WixDocument document)
		{
			throw new NotImplementedException();
		}
	}
}
