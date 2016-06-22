// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Resources.Tools;

using ICSharpCode.Core;
using ICSharpCode.EasyCodeDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.CSharp;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Can serialize assignments to component properties using project resource references.
	/// </summary>
	internal sealed class ProjectResourcesMemberCodeDomSerializer : MemberCodeDomSerializer
	{
		readonly MemberCodeDomSerializer baseSerializer;
		
		public ProjectResourcesMemberCodeDomSerializer(MemberCodeDomSerializer baseSerializer)
		{
			if (baseSerializer == null)
				throw new ArgumentNullException("baseSerializer");
			this.baseSerializer = baseSerializer;
		}
		
		public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
		{
			if (!this.SerializeProjectResource(manager, value, descriptor, statements)) {
				this.baseSerializer.Serialize(manager, value, descriptor, statements);
			}
		}
		
		bool SerializeProjectResource(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
		{
			var propDesc = descriptor as PropertyDescriptor;
			if (propDesc == null) return false;
			
			var component = value as IComponent;
			if (component == null || component.Site == null) return false;
			
			if (!propDesc.ShouldSerializeValue(component)) {
				return false;
			}
			
			var dictService = component.Site.GetService(typeof(IDictionaryService)) as IDictionaryService;
			if (dictService == null) return false;
			
			var resourceInfo = dictService.GetValue(ProjectResourceService.ProjectResourceKey + propDesc.Name) as ProjectResourceInfo;
			if (resourceInfo == null) return false;
			
			if (!Object.ReferenceEquals(resourceInfo.OriginalValue, propDesc.GetValue(value))) {
				LoggingService.Info("Value of property '" + propDesc.Name + "' on component '" + value.ToString() + "' is not equal to stored project resource value. Ignoring this resource.");
				return false;
			}
			
			
			// Find the generated file with the resource accessing class.
			
			var prs = manager.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			if (prs == null) {
				LoggingService.Warn("ProjectResourceService not found");
				return false;
			}
			
			IProject project = prs.ProjectContent;
			if (project == null) {
				LoggingService.Warn("Serializer cannot proceed because project is not an IProject");
				return false;
			}
			
			string resourceFileDirectory = Path.GetDirectoryName(resourceInfo.ResourceFile);
			string resourceFileName = Path.GetFileName(resourceInfo.ResourceFile);
			var items = project.Items
				.OfType<FileProjectItem>()
				.Where(
					fpi =>
					FileUtility.IsEqualFileName(Path.GetDirectoryName(fpi.FileName), resourceFileDirectory) &&
					FileUtility.IsEqualFileName(fpi.DependentUpon, resourceFileName) &&
					fpi.ItemType == ItemType.Compile &&
					fpi.VirtualName.ToUpperInvariant().Contains("DESIGNER")
				).ToList();
			
			if (items.Count != 1) {
				LoggingService.Info("Did not find exactly one possible file that contains the generated class for the resource file '" + resourceInfo.ResourceFile + "'. Ignoring this resource.");
				return false;
			}
			
			FileName resourceCodeFile = items.Single().FileName;
			
			// We expect a single class to be in this file.
			var resourceClass = SD.ParserService.GetExistingUnresolvedFile(resourceCodeFile).TopLevelTypeDefinitions.Single();
			// Here we assume that VerifyResourceName is the same name transform that
			// was used when generating the resource code file.
			// This should be true as long as the code is generated using the
			// custom tool in SharpDevelop or Visual Studio.
			string resourcePropertyName = StronglyTypedResourceBuilder.VerifyResourceName(resourceInfo.ResourceKey, prs.ProjectContent.CreateCodeDomProvider() ?? new CSharpCodeProvider());
			if (resourcePropertyName == null) {
				throw new InvalidOperationException("The resource name '" + resourceInfo.ResourceKey + "' could not be transformed to a name that is valid in the current programming language.");
			}
			
			
			// Now do the actual serialization.
			
			LoggingService.Debug("Serializing project resource: Component '" + component.ToString() + "', Property: '" + propDesc.Name + "', Resource class: '" + resourceClass.FullName + "', Resource property: '" + resourcePropertyName + "'");
			
			var targetObjectExpr = base.SerializeToExpression(manager, value);
			if (targetObjectExpr == null) {
				LoggingService.Info("Target object could not be serialized: " + value.ToString());
				return false;
			}
			
			if (propDesc.SerializationVisibility == DesignerSerializationVisibility.Content) {
				LoggingService.Debug("-> is a content property, ignoring this.");
				return false;
			}
			
			var propRefSource =
				Easy.Type(
					new CodeTypeReference(resourceClass.FullName, CodeTypeReferenceOptions.GlobalReference)
				).Property(resourcePropertyName);
			
			var extAttr = propDesc.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
			if (extAttr != null && extAttr.Provider != null) {
				
				// This is an extender property.
				var extProvider = base.SerializeToExpression(manager, extAttr.Provider);
				if (extProvider == null) {
					throw new InvalidOperationException("Could not serialize the extender provider '" + extAttr.Provider.ToString() + "'.");
				}
				
				statements.Add(
					extProvider.InvokeMethod(
						"Set" + propDesc.Name,
						targetObjectExpr,
						propRefSource
					)
				);
				
			} else {
				
				// This is a standard property.
				statements.Add(
					new CodeAssignStatement(
						new CodePropertyReferenceExpression(targetObjectExpr, propDesc.Name),
						propRefSource)
				);
				
			}
			
			return true;
		}
		
		public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
		{
			return this.baseSerializer.ShouldSerialize(manager, value, descriptor);
		}
	}
}
