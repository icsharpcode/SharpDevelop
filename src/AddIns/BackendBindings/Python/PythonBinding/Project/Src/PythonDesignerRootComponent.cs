// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a root component in the designer.
	/// </summary>
	public class PythonDesignerRootComponent : PythonDesignerComponent
	{
		public PythonDesignerRootComponent(IComponent component)
			: base(null, component)
		{
		}
		
		public override string GetPropertyOwnerName()
		{
			return "self";
		}
		
		public override void AppendSuspendLayout(PythonCodeBuilder codeBuilder)
		{
			AppendMethodCalls(codeBuilder, suspendLayoutMethods);
		}
		
		public override void AppendResumeLayout(PythonCodeBuilder codeBuilder)
		{
			AppendMethodCalls(codeBuilder, resumeLayoutMethods);
		}		
		
		public override void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			// Add the child components first.
			foreach (PythonDesignerComponent component in GetContainerComponents()) {
				component.AppendComponent(codeBuilder);
			}
			
			// Add root component
			AppendComponentProperties(codeBuilder, false, true);
		}
				
		public PythonDesignerComponent[] GetNonVisualChildComponents()
		{
			List<PythonDesignerComponent> components = new List<PythonDesignerComponent>();
			foreach (IComponent containerComponent in Component.Site.Container.Components) {
				PythonDesignerComponent designerComponent = PythonDesignerComponentFactory.CreateDesignerComponent(this, containerComponent);
				if (designerComponent.IsNonVisual) {
					components.Add(designerComponent);
				}
			}
			return components.ToArray();
		}
				
		/// <summary>
		/// Adds BeginInit method call for any non-visual components that implement the 
		/// System.ComponentModel.ISupportInitialize interface.
		/// </summary>
		public void AppendNonVisualComponentsBeginInit(PythonCodeBuilder codeBuilder)
		{
			AppendNonVisualComponentsMethodCalls(codeBuilder, new string[] {"BeginInit()"});
		}
		
		/// <summary>
		/// Adds EndInit method call for any non-visual components that implement the 
		/// System.ComponentModel.ISupportInitialize interface.
		/// </summary>
		public void AppendNonVisualComponentsEndInit(PythonCodeBuilder codeBuilder)
		{
			AppendNonVisualComponentsMethodCalls(codeBuilder, new string[] {"EndInit()"});
		}		
		
		public void AppendNonVisualComponentsMethodCalls(PythonCodeBuilder codeBuilder, string[] methods)
		{
			foreach (PythonDesignerComponent component in GetNonVisualChildComponents()) {
				if (typeof(ISupportInitialize).IsAssignableFrom(component.GetComponentType())) {
					component.AppendMethodCalls(codeBuilder, methods);
				}
			}			
		}
		
		/// <summary>
		/// Reverses the ordering when adding items to the Controls collection.
		/// </summary>
		public override void AppendMethodCallWithArrayParameter(PythonCodeBuilder codeBuilder, string propertyOwnerName, object propertyOwner, PropertyDescriptor propertyDescriptor)
		{
			bool reverse = propertyDescriptor.Name == "Controls";
			AppendMethodCallWithArrayParameter(codeBuilder, propertyOwnerName, propertyOwner, propertyDescriptor, reverse);
		}		
	}
}
