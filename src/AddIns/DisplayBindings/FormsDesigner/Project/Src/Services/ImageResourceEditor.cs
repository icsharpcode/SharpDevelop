// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.FormsDesigner.Gui;

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
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);

			if (context == null || context.PropertyDescriptor == null)
				return UITypeEditorEditStyle.None;
			
			if (typeof(Image).IsAssignableFrom(context.PropertyDescriptor.PropertyType) ||
			    typeof(Icon).IsAssignableFrom(context.PropertyDescriptor.PropertyType)) {
				
				if (context.GetService(typeof(IProjectResourceService)) is IProjectResourceService) {
					return UITypeEditorEditStyle.Modal;
				}
				
			}
			
			return UITypeEditorEditStyle.Modal;
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);
			
			if (context == null || context.PropertyDescriptor == null || context.Instance == null || provider == null) {
				return value;
			}
			
			IFormsDesignerLoggingService logger = provider.GetService(typeof(IFormsDesignerLoggingService)) as IFormsDesignerLoggingService;
			
			IComponent component = context.Instance as IComponent;
			if (component == null || component.Site == null) {
				logger.Info("Editing of image properties on objects not implementing IComponent and components without Site is not supported by the ImageResourceEditor.");
				if (typeof(Icon).IsAssignableFrom(context.PropertyDescriptor.PropertyType)) {
					return new IconEditor().EditValue(context, provider, value);
				} else {
					return new ImageEditor().EditValue(context, provider, value);
				}
			}
			
			var prs = provider.GetService(typeof(IProjectResourceService)) as IProjectResourceService;
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
			
			var projectResource = dictService.GetValue(prs.ProjectResourceKey + context.PropertyDescriptor.Name) as IProjectResourceInfo;
			
			var imageDialogWrapper = provider.GetService(typeof(IImageResourceEditorDialogWrapper)) as IImageResourceEditorDialogWrapper;
			
			ImageResourceEditorDialog dialog;
			
			if (projectResource != null && object.ReferenceEquals(projectResource.OriginalValue, value) && prs.DesignerSupportsProjectResources) {
				dialog = new ImageResourceEditorDialog(provider, imageDialogWrapper, context.PropertyDescriptor.PropertyType, projectResource);
			} else {
				if (context.PropertyDescriptor.PropertyType == typeof(Image)) {
					dialog = new ImageResourceEditorDialog(provider, imageDialogWrapper, value, false, prs.DesignerSupportsProjectResources);
				} else if (context.PropertyDescriptor.PropertyType == typeof(Icon)) {
					dialog = new ImageResourceEditorDialog(provider, imageDialogWrapper, value, true, prs.DesignerSupportsProjectResources);
				} else {
					throw new InvalidOperationException("ImageResourceEditor called on unsupported property type: " + context.PropertyDescriptor.PropertyType.ToString());
				}
			}
			
			object imageData = null;
			
			using(dialog) {
				if (edsvc.ShowDialog(dialog) == DialogResult.OK) {
					projectResource = dialog.SelectedProjectResource;
					if (projectResource != null) {
						dictService.SetValue(prs.ProjectResourceKey + context.PropertyDescriptor.Name, projectResource);
						
						// Ensure the resource generator is turned on for the selected resource file.
						imageDialogWrapper.UpdateProjectResource(projectResource);
						
						imageData = projectResource.OriginalValue;
					} else {
						dictService.SetValue(prs.ProjectResourceKey + context.PropertyDescriptor.Name, null);
						imageData = dialog.SelectedResourceValue;
					}
				}
			}
			
			return imageData ?? value;
		}
		
		[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);
			
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
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);
			
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
	
	class DictServiceProxy : MarshalByRefObject, IDictionaryService
	{
		IDictionaryService svc;
		
		public DictServiceProxy(IDictionaryService svc)
		{
			this.svc = svc;
		}
		
		public object GetKey(object value)
		{
			return svc.GetKey(value);
		}
		
		public object GetValue(object key)
		{
			return svc.GetValue(key);
		}
		
		public void SetValue(object key, object value)
		{
			svc.SetValue(key, value);
		}
	}
}
