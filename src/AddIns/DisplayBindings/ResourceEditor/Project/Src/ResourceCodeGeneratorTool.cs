// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Resources;
using System.Resources.Tools;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ResourceEditor
{
	public class ResourceCodeGeneratorTool : ICustomTool
	{
		protected bool createInternalClass = true;
		
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			/*context.GenerateCodeDomAsync(item, context.GetOutputFileName(item, ".Designer"),
			                             delegate {
			                             	return GenerateCodeDom();
			                             });*/
			string inputFilePath = item.FileName;
			
			// Ensure that the generated code will not conflict with an
			// existing class.
			if (context.Project != null) {
				ICompilation pc = SD.ParserService.GetCompilation(context.Project);
				if (pc != null) {
					var codeDomProvider = context.Project.LanguageBinding.CodeDomProvider;
					string resourceName = Path.GetFileNameWithoutExtension(inputFilePath);
					if (codeDomProvider != null) {
						resourceName = StronglyTypedResourceBuilder.VerifyResourceName(resourceName, codeDomProvider);
					}
					var existingClass = pc.FindType(new FullTypeName(context.OutputNamespace + "." + resourceName)).GetDefinition();
					if (existingClass != null) {
						if (!IsGeneratedResourceClass(existingClass)) {
							context.MessageView.AppendLine(String.Format(System.Globalization.CultureInfo.CurrentCulture, ResourceService.GetString("ResourceEditor.ResourceCodeGeneratorTool.ClassConflict"), inputFilePath, existingClass.FullName));
							return;
						}
					}
				}
			}
			
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
			
			string generatedCodeNamespace = context.OutputNamespace;
			
			context.WriteCodeDomToFile(
				item,
				context.GetOutputFileName(item, ".Designer"),
				StronglyTypedResourceBuilder.Create(
					resources,        // resourceList
					Path.GetFileNameWithoutExtension(inputFilePath), // baseName
					generatedCodeNamespace, // generatedCodeNamespace
					context.OutputNamespace, // resourcesNamespace
					context.Project.LanguageBinding.CodeDomProvider, // codeProvider
					createInternalClass,             // internal class
					out unmatchable
				));
			
			foreach (string s in unmatchable) {
				context.MessageView.AppendLine(String.Format(System.Globalization.CultureInfo.CurrentCulture, ResourceService.GetString("ResourceEditor.ResourceCodeGeneratorTool.CouldNotGenerateResourceProperty"), s));
			}
		}
		
		/// <summary>
		/// Determines whether the specified class is a generated resource
		/// class, based on the attached attributes.
		/// </summary>
		static bool IsGeneratedResourceClass(ITypeDefinition type)
		{
			var generatedCodeAttributeType = type.Compilation.FindType(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute));
			if (generatedCodeAttributeType.Kind == TypeKind.Unknown) {
				LoggingService.Info("Could not find the class for 'System.CodeDom.Compiler.GeneratedCodeAttribute'.");
				return false;
			}
			
			foreach (IAttribute att in type.Attributes) {
				if (att.AttributeType.Equals(generatedCodeAttributeType) && att.PositionalArguments.Count == 2) {
					var firstArg = att.PositionalArguments[0].ConstantValue as string;
					if (string.Equals(typeof(StronglyTypedResourceBuilder).FullName, firstArg, StringComparison.Ordinal))
						return true;
				}
			}
			return false;
		}
	}
	
	public class PublicResourceCodeGeneratorTool : ResourceCodeGeneratorTool
	{
		public PublicResourceCodeGeneratorTool()
		{
			base.createInternalClass = false;
		}
	}
}
