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
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// An abstract base class for CodeDOM designer loaders.
	/// </summary>
	public abstract class AbstractCodeDomDesignerLoader : CodeDomDesignerLoader
	{
		bool loading;
		IDesignerLoaderHost designerLoaderHost = null;
		ITypeResolutionService typeResolutionService = null;
		
		public override bool Loading {
			get { return base.Loading || loading; }
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get { return this.typeResolutionService; }
		}
		
		protected IDesignerLoaderHost DesignerLoaderHost {
			get { return this.designerLoaderHost; }
		}
		
		public override void Dispose()
		{
			try {
				IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null) {
					LoggingService.Debug("Forms designer: Removing ComponentAdded handler for nested container setup");
					componentChangeService.ComponentAdded -= ComponentContainerSetUp;
				} else {
					LoggingService.Info("Forms designer: Could not remove ComponentAdding handler because IComponentChangeService is no longer available");
				}
			} finally {
				base.Dispose();
			}
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			this.loading = true;
			this.typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			this.designerLoaderHost = host;
			
			base.BeginLoad(host);
		}
		
		static void ComponentContainerSetUp(object sender, ComponentEventArgs e)
		{
			// HACK: This reflection mess fixes SD2-1374 and SD2-1375. However I am not sure why it is needed in the first place.
			// There seems to be a problem with the nested container class used
			// by the designer. It only establishes a connection to the service
			// provider of the DesignerHost after it has been queried for
			// an IServiceContainer service. This does not always happen
			// automatically, so we enforce that here. We have to use
			// reflection because the request for IServiceContainer is
			// not forwarded by higher-level GetService methods.
			// Also, be very careful when trying to troubleshoot this using
			// the debugger because it automatically gets all properties and
			// this can cause side effects here, such as initializing that service
			// so that the problem no longer appears.
			INestedContainer nestedContainer = e.Component.Site.GetService(typeof(INestedContainer)) as INestedContainer;
			if (nestedContainer != null) {
				MethodInfo getServiceMethod = nestedContainer.GetType().GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic, null, new [] {typeof(Type)}, null);
				if (getServiceMethod != null) {
					LoggingService.Debug("Forms designer: Initializing nested service container of " + e.Component.ToString() + " using Reflection");
					getServiceMethod.Invoke(nestedContainer, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, new [] {typeof(IServiceContainer)}, null);
				}
			}
		}
		
		protected override void Initialize()
		{
			CodeDomLocalizationModel model = FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.DefaultLocalizationModel;
			
			if (FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.KeepLocalizationModel) {
				// Try to find out the current localization model of the designed form
				CodeDomLocalizationModel existingModel = this.GetCurrentLocalizationModelFromDesignedFile();
				if (existingModel != CodeDomLocalizationModel.None) {
					LoggingService.Debug("Determined existing localization model, using that: " + existingModel.ToString());
					model = existingModel;
				} else {
					LoggingService.Debug("Could not determine existing localization model, using default: " + model.ToString());
				}
			} else {
				LoggingService.Debug("Using default localization model: " + model.ToString());
			}
			
			CodeDomLocalizationProvider localizationProvider = new CodeDomLocalizationProvider(designerLoaderHost, model);
			IDesignerSerializationManager manager = (IDesignerSerializationManager)designerLoaderHost.GetService(typeof(IDesignerSerializationManager));
			manager.AddSerializationProvider(new SharpDevelopSerializationProvider());
			manager.AddSerializationProvider(localizationProvider);
			base.Initialize();
			
			IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				LoggingService.Debug("Forms designer: Adding ComponentAdded handler for nested container setup");
				componentChangeService.ComponentAdded += ComponentContainerSetUp;
			} else {
				LoggingService.Warn("Forms designer: Cannot add ComponentAdded handler for nested container setup because IComponentChangeService is unavailable");
			}
		}
		
		/// <summary>
		/// When overridden in derived classes, this method should return the current
		/// localization model of the designed file or None, if it cannot be determined.
		/// </summary>
		/// <returns>The default implementation always returns None.</returns>
		protected virtual CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			return CodeDomLocalizationModel.None;
		}
		
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			this.loading = false;
			//when control's Dispose() has a exception and on loading also raised exception
			//then this is only place where this error can be logged, because after errors is
			//catched internally in .net
			try {
				base.OnEndLoad(successful, errors);
			} catch(ExceptionCollection e) {
				LoggingService.Error("DesignerLoader.OnEndLoad error " + e.Message, e);
				foreach(Exception ine in e.Exceptions) {
					LoggingService.Error("DesignerLoader.OnEndLoad error " + ine.Message, ine);
				}
				throw;
			} catch(Exception e) {
				LoggingService.Error("DesignerLoader.OnEndLoad error " + e.Message, e);
				throw;
			}
		}
		
		protected override void ReportFlushErrors(ICollection errors)
		{
			StringBuilder sb = new StringBuilder(StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.ReportFlushErrors}") + Environment.NewLine + Environment.NewLine);
			foreach (var error in errors) {
				sb.AppendLine(error.ToString());
				sb.AppendLine();
			}
			MessageService.ShowError(sb.ToString());
		}
	}
}
