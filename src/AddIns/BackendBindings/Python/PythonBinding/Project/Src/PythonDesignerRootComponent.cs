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
			: base(component)
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
			foreach (PythonDesignerComponent component in GetChildComponents()) {
				component.AppendComponent(codeBuilder);
			}
			
			// Add root component
			AppendComponentProperties(codeBuilder, false, false, true);
		}
		
		/// <summary>
		/// Gets the child components in reverse order since the forms designer has them reversed.
		/// </summary>
		public override PythonDesignerComponent[] GetChildComponents()
		{
			PythonDesignerComponent[] components = base.GetChildComponents();
			Array.Reverse(components);
			return components;
		}
		
		/// <summary>
		/// Returns true if non-visual components (e.g. Timer) are associated with this root component.
		/// </summary>	
		public bool HasNonVisualChildComponents()
		{
			foreach (IComponent containerComponent in Component.Site.Container.Components) {
				if (IsNonVisualComponent(containerComponent)) {
					return true;
				}
			}
			return false;
		}
		
		public PythonDesignerComponent[] GetNonVisualChildComponents()
		{
			List<PythonDesignerComponent> components = new List<PythonDesignerComponent>();
			foreach (IComponent containerComponent in Component.Site.Container.Components) {
				PythonDesignerComponent designerComponent = PythonDesignerComponentFactory.CreateDesignerComponent(containerComponent);
				if (designerComponent.IsNonVisual) {
					components.Add(designerComponent);
				}
			}
			return components.ToArray();
		}
		
		/// <summary>
		/// Appends an expression that creates an instance of the Container to hold non-visual components
		/// </summary>
		public void AppendCreateComponentsContainer(PythonCodeBuilder codeBuilder)
		{
			codeBuilder.AppendIndentedLine("self._components = " + typeof(Container).FullName + "()");
		}
		
		/// <summary>
		/// Appends code to create all the non-visual component.
		/// </summary>
		public void AppendCreateNonVisualComponents(PythonCodeBuilder codeBuilder)
		{
			foreach (PythonDesignerComponent component in GetNonVisualChildComponents()) {
				if (component.HasIContainerConstructor()) {
					component.AppendCreateInstance(codeBuilder, "self._components");
				} else {
					component.AppendCreateInstance(codeBuilder);
				}
			}
		}
		
		/// <summary>
		/// Appends code to set all the non-visual component properties.
		/// </summary>
		public void AppendNonVisualComponents(PythonCodeBuilder codeBuilder)
		{
			foreach (PythonDesignerComponent component in GetNonVisualChildComponents()) {
				component.AppendComponent(codeBuilder);
			}
		}
	}
}
