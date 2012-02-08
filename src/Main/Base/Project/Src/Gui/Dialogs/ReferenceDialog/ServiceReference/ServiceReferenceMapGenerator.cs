// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml.Serialization;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceMapGenerator : IServiceReferenceMapGenerator
	{
		public void GenerateServiceReferenceMapFile(ServiceReferenceMapFile mapFile)
		{
			var writer = new StreamWriter(mapFile.FileName);
			GenerateServiceReferenceMapFile(writer, mapFile);
		}
		
		public void GenerateServiceReferenceMapFile(TextWriter textWriter, ServiceReferenceMapFile mapFile)
		{
			var serializer = new XmlSerializer(typeof(ServiceReferenceMapFile));
			serializer.Serialize(textWriter, mapFile);
		}
	}
}
