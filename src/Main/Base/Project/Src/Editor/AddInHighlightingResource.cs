// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor
{
	public class AddInHighlightingResource
	{
		Runtime[] runtimes;
		
		public AddInHighlightingResource(Runtime[] runtimes)
		{
			this.runtimes = runtimes;
		}
		
		public Stream OpenStream(string name)
		{
			foreach (Runtime runtime in runtimes) {
				Assembly assembly = runtime.LoadedAssembly;
				if (assembly != null) {
					Stream stream = assembly.GetManifestResourceStream(name);
					if (stream != null) {
						return stream;
					}
				}
			}
			ThrowFileNotFoundException(name);
			return null;
		}
		
		void ThrowFileNotFoundException(string name)
		{
			string message = String.Format("The resource file '{0}' was not found.", name);
			throw new FileNotFoundException(message);
		}
		
		public IHighlightingDefinition LoadHighlighting(string name, IHighlightingDefinitionReferenceResolver resolver)
		{
			if (resolver == null) {
				throw new ArgumentNullException("resolver");
			}
			
			using (Stream stream = OpenStream(name)) {
				using (XmlTextReader reader = new XmlTextReader(stream)) {
					return HighlightingLoader.Load(reader, resolver);
				}
			}
		}
	}
}
