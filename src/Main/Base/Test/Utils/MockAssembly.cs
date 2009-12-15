// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockAssembly : Assembly
	{
		Dictionary<string, Stream> manifestResourceStreams = new Dictionary<string, Stream>();
		
		public MockAssembly()
		{
		}
		
		public void AddManifestResourceStream(string name, Stream stream)
		{
			manifestResourceStreams.Add(name, stream);
		}
		
		public override Stream GetManifestResourceStream(string name)
		{
			Stream stream;
			if (manifestResourceStreams.TryGetValue(name, out stream)) {
				return stream;
			}
			return null;
		}
		
		public override Type[] GetExportedTypes()
		{
			return new Type[0];
		}
	}
}
