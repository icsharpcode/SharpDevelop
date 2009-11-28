// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
