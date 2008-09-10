// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Project;
using System.Resources;
using System.Resources.Tools;
using System.IO;

namespace ResourceEditor
{
	public class ResourceCodeGeneratorTool : ICustomTool
	{
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			/*context.GenerateCodeDomAsync(item, context.GetOutputFileName(item, ".Designer"),
			                             delegate {
			                             	return GenerateCodeDom();
			                             });*/
			string inputFilePath = item.FileName;
			
			IResourceReader reader;
			if (string.Equals(Path.GetExtension(inputFilePath), ".resx", StringComparison.OrdinalIgnoreCase)) {
				reader = new ResXResourceReader(inputFilePath);
				((ResXResourceReader)reader).BasePath = Path.GetDirectoryName(inputFilePath);
			} else {
				reader = new ResourceReader(inputFilePath);
			}
			
			Hashtable resources = new Hashtable();
			foreach (DictionaryEntry de in reader) {
				resources.Add(de.Key, de.Value);
			}
			
			string[] unmatchable = null;
			
			context.WriteCodeDomToFile(
				item,
				context.GetOutputFileName(item, ".Designer"),
				StronglyTypedResourceBuilder.Create(
					resources,        // resourceList
					Path.GetFileNameWithoutExtension(inputFilePath), // baseName
					context.OutputNamespace, // generatedCodeNamespace
					context.OutputNamespace, // resourcesNamespace
					context.Project.LanguageProperties.CodeDomProvider, // codeProvider
					true,             // internal class
					out unmatchable
				));
		}
		
		
	}
}
