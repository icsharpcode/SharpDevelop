// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Resources.Tools;

using ICSharpCode.EasyCodeDom;

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
			
			var prs = manager.GetService(typeof(IProjectResourceService)) as IProjectResourceService;
			if (prs == null) {
				FormsDesignerLoggingService.Warn("ProjectResourceService not found");
				return false;
			}
			
			var resourceInfo = dictService.GetValue(prs.ProjectResourceKey + propDesc.Name) as IProjectResourceInfo;
			if (resourceInfo == null) return false;
			
			if (!Object.ReferenceEquals(resourceInfo.OriginalValue, propDesc.GetValue(value))) {
				FormsDesignerLoggingService.Info("Value of property '" + propDesc.Name + "' on component '" + value.ToString() + "' is not equal to stored project resource value. Ignoring this resource.");
				return false;
			}
			
			// Find the generated file with the resource accessing class.
			string resourceClassFullyQualifiedName;
			string resourcePropertyName;
			if (!prs.FindResourceClassNames(resourceInfo, out resourceClassFullyQualifiedName, out resourcePropertyName))
				return false;
			
			// Now do the actual serialization.
			
			FormsDesignerLoggingService.Debug("Serializing project resource: Component '" + component.ToString() + "', Property: '" + propDesc.Name + "', Resource class: '" + resourceClassFullyQualifiedName + "', Resource property: '" + resourcePropertyName + "'");
			
			var targetObjectExpr = base.SerializeToExpression(manager, value);
			if (targetObjectExpr == null) {
				FormsDesignerLoggingService.Info("Target object could not be serialized: " + value.ToString());
				return false;
			}
			
			if (propDesc.SerializationVisibility == DesignerSerializationVisibility.Content) {
				FormsDesignerLoggingService.Debug("-> is a content property, ignoring this.");
				return false;
			}
			
			var propRefSource =
				Easy.Type(
					new CodeTypeReference(resourceClassFullyQualifiedName, CodeTypeReferenceOptions.GlobalReference)
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
