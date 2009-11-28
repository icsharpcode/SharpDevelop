// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
