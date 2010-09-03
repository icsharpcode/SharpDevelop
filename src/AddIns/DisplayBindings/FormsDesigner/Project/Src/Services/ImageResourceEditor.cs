// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Edit image and icon properties with the option to use a project resource
	/// in an <see cref="ImageResourceEditorDialog"/>.
	/// </summary>
	public sealed class ImageResourceEditor : UITypeEditor
	{
		public ImageResourceEditor()
		{
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context == null || context.PropertyDescriptor == null)
				return UITypeEditorEditStyle.None;
			
			if (typeof(Image).IsAssignableFrom(context.PropertyDescriptor.PropertyType) ||
			    typeof(Icon).IsAssignableFrom(context.PropertyDescriptor.PropertyType)) {
				
				if (context.GetService(typeof(ProjectResourceService)) is ProjectResourceService) {
					return UITypeEditorEditStyle.Modal;
				}
				
			}
			
			return UITypeEditorEditStyle.Modal;
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context == null || context.PropertyDescriptor == null || context.Instance == null || provider == null) {
				return value;
			}
			
			IComponent component = context.Instance as IComponent;
			if (component == null || component.Site == null) {
				LoggingService.Info("Editing of image properties on objects not implementing IComponent and components without Site is not supported by the ImageResourceEditor.");
				if (typeof(Icon).IsAssignableFrom(context.PropertyDescriptor.PropertyType)) {
					return new IconEditor().EditValue(context, provider, value);
				} else {
					return new ImageEditor().EditValue(context, provider, value);
				}
			}
			
			var prs = provider.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			if (prs == null) {
				return value;
			}
			
			var edsvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edsvc == null) {
				throw new InvalidOperationException("The required IWindowsFormsEditorService is not available.");
			}
			
			var dictService = component.Site.GetService(typeof(IDictionaryService)) as IDictionaryService;
			if (dictService == null) {
				throw new InvalidOperationException("The required IDictionaryService is not available.");
			}
			
			var projectResource = dictService.GetValue(ProjectResourceService.ProjectResourceKey + context.PropertyDescriptor.Name) as ProjectResourceInfo;
			
			IProject project = prs.ProjectContent.Project as IProject;
			ImageResourceEditorDialog dialog;
			
			if (projectResource != null && Object.ReferenceEquals(projectResource.OriginalValue, value) && prs.DesignerSupportsProjectResources) {
				dialog = new ImageResourceEditorDialog(project, context.PropertyDescriptor.PropertyType, projectResource);
			} else {
				if (context.PropertyDescriptor.PropertyType == typeof(Image)) {
					dialog = new ImageResourceEditorDialog(project, value as Image, prs.DesignerSupportsProjectResources);
				} else if (context.PropertyDescriptor.PropertyType == typeof(Icon)) {
					dialog = new ImageResourceEditorDialog(project, value as Icon, prs.DesignerSupportsProjectResources);
				} else {
					throw new InvalidOperationException("ImageResourceEditor called on unsupported property type: " + context.PropertyDescriptor.PropertyType.ToString());
				}
			}
			
			using(dialog) {
				if (edsvc.ShowDialog(dialog) == DialogResult.OK) {
					projectResource = dialog.SelectedProjectResource;
					if (projectResource != null) {
						dictService.SetValue(ProjectResourceService.ProjectResourceKey + context.PropertyDescriptor.Name, projectResource);
						
						// Ensure the resource generator is turned on for the selected resource file.
						if (project != null) {
							FileProjectItem fpi = project.FindFile(projectResource.ResourceFile);
							if (fpi == null) {
								throw new InvalidOperationException("The selected resource file '" + projectResource.ResourceFile + "' was not found in the project.");
							}
							const string resourceGeneratorToolName = "ResXFileCodeGenerator";
							const string publicResourceGeneratorToolName = "PublicResXFileCodeGenerator";
							if (!String.Equals(resourceGeneratorToolName, fpi.CustomTool, StringComparison.Ordinal) &&
							    !String.Equals(publicResourceGeneratorToolName, fpi.CustomTool, StringComparison.Ordinal)) {
								fpi.CustomTool = resourceGeneratorToolName;
							}
							CustomToolsService.RunCustomTool(fpi, true);
						}
						
						return projectResource.OriginalValue;
					} else {
						dictService.SetValue(ProjectResourceService.ProjectResourceKey + context.PropertyDescriptor.Name, null);
						return dialog.SelectedResourceValue;
					}
				}
			}
			
			return value;
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			if (context != null && context.PropertyDescriptor != null &&
			    (context.PropertyDescriptor.PropertyType == typeof(Image) ||
			     context.PropertyDescriptor.PropertyType == typeof(Icon))) {
				return true;
			}
			return base.GetPaintValueSupported(context);
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override void PaintValue(PaintValueEventArgs e)
		{
			Image img = e.Value as Image;
			if (img != null) {
				e.Graphics.DrawImage(img, e.Bounds);
			} else {
				Icon icon = e.Value as Icon;
				if (icon != null) {
					e.Graphics.DrawIcon(icon, e.Bounds);
				} else {
					base.PaintValue(e);
				}
			}
		}
	}
}
