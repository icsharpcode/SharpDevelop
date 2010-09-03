// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
