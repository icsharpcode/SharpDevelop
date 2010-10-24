// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// An abstract base class for CodeDOM designer loaders.
	/// </summary>
	public class SharpDevelopDesignerLoader : CodeDomDesignerLoader
	{
		bool loading;
		IDesignerLoaderHost designerLoaderHost = null;
		ITypeResolutionService typeResolutionService = null;
		readonly IDesignerLoader loader;
		readonly IDesignerGenerator generator;
		
		public override bool Loading {
			get { return base.Loading || loading; }
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get { return this.typeResolutionService; }
		}
		
		protected IDesignerLoaderHost DesignerLoaderHost {
			get { return this.designerLoaderHost; }
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get { return this.generator.CodeDomProvider; }
		}
		
		protected IDesignerGenerator Generator {
			get { return this.generator; }
		}
		
		public SharpDevelopDesignerLoader(IDesignerGenerator generator, IDesignerLoader loader)
		{
			if (generator == null) {
				throw new ArgumentNullException("generator", "Generator cannot be null");
			}
			if (loader == null)
				throw new ArgumentNullException("loader");

			this.generator = generator;
			this.loader = loader;
		}
		
		public override void Dispose()
		{
			try {
				IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null) {
					FormsDesignerLoggingService.Debug("Forms designer: Removing ComponentAdded handler for nested container setup");
					componentChangeService.ComponentAdded -= ComponentContainerSetUp;
				} else {
					FormsDesignerLoggingService.Info("Forms designer: Could not remove ComponentAdding handler because IComponentChangeService is no longer available");
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
					FormsDesignerLoggingService.Debug("Forms designer: Initializing nested service container of " + e.Component.ToString() + " using Reflection");
					getServiceMethod.Invoke(nestedContainer, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, new [] {typeof(IServiceContainer)}, null);
				}
			}
		}
		
		protected override void Initialize()
		{
			CodeDomLocalizationModel model = loader.GetLocalizationModel();
			
			CodeDomLocalizationProvider localizationProvider = new CodeDomLocalizationProvider(designerLoaderHost, model);
			IDesignerSerializationManager manager = (IDesignerSerializationManager)designerLoaderHost.GetService(typeof(IDesignerSerializationManager));
			manager.AddSerializationProvider(new SharpDevelopSerializationProvider());
			manager.AddSerializationProvider(localizationProvider);
			base.Initialize();
			
			IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				FormsDesignerLoggingService.Debug("Forms designer: Adding ComponentAdded handler for nested container setup");
				componentChangeService.ComponentAdded += ComponentContainerSetUp;
			} else {
				FormsDesignerLoggingService.Warn("Forms designer: Cannot add ComponentAdded handler for nested container setup because IComponentChangeService is unavailable");
			}
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
				FormsDesignerLoggingService.Error("DesignerLoader.OnEndLoad error " + e.Message, e);
				foreach(Exception ine in e.Exceptions) {
					FormsDesignerLoggingService.Error("DesignerLoader.OnEndLoad error " + ine.Message, ine);
				}
				throw;
			} catch(Exception e) {
				FormsDesignerLoggingService.Error("DesignerLoader.OnEndLoad error " + e.Message, e);
				throw;
			}
		}
		
		protected override void Write(System.CodeDom.CodeCompileUnit unit)
		{
			loader.Write(unit);
		}
		
		protected override System.CodeDom.CodeCompileUnit Parse()
		{
			return loader.Parse();
		}
		
		protected override bool IsReloadNeeded()
		{
			return loader.IsReloadNeeded(base.IsReloadNeeded());
		}
	}
}
