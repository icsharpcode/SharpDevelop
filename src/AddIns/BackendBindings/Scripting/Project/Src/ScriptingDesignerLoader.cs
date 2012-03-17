// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Resources;
using System.Security.Permissions;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public class ScriptingDesignerLoader : BasicDesignerLoader, IComponentCreator
	{
		IScriptingDesignerGenerator generator;
		IDesignerSerializationManager serializationManager;
		IResourceService resourceService;
		Dictionary<string, IComponent> addedObjects = new Dictionary<string, IComponent>();
	
		public ScriptingDesignerLoader(IScriptingDesignerGenerator generator)
		{
			if (generator == null) {
				throw new ArgumentException("Generator cannot be null.", "generator");
			}
			this.generator = generator;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			AddServices(host);
			SetDesignerSupportsProjectResourcesToFalse(host);
			base.BeginLoad(host);
		}
		
		void AddServices(IDesignerLoaderHost host)
		{
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(INameCreationService), new ScriptingNameCreationService(host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(host));
		}
		
		void SetDesignerSupportsProjectResourcesToFalse(IDesignerLoaderHost host)
		{
			ProjectResourceService projectResourceService = host.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			if (projectResourceService != null) {
				projectResourceService.DesignerSupportsProjectResources = false;
			}
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			return base.LoaderHost.CreateComponent(componentClass, name);
		}
		
		public void Add(IComponent component, string name)
		{
			base.LoaderHost.Container.Add(component, name);
		}
		
		/// <summary>
		/// Gets a component that has been added to the loader.
		/// </summary>
		/// <returns>Null if the component cannot be found.</returns>
		public IComponent GetComponent(string name)
		{
			return LoaderHost.Container.Components[name];
		}
		
		public IComponent RootComponent {
			get { return base.LoaderHost.RootComponent; }
		}
		
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			return serializationManager.CreateInstance(type, arguments, name, addToContainer);
		}
		
		public object GetInstance(string name)
		{
			return serializationManager.GetInstance(name);
		}

		public Type GetType(string typeName)
		{
			return serializationManager.GetType(typeName);
		}
		
		/// <summary>
		/// Gets the property descriptor associated with the event.
		/// </summary>
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			IEventBindingService eventBindingService = GetService(typeof(IEventBindingService)) as IEventBindingService;
			return eventBindingService.GetEventProperty(e);
		}

		public IResourceReader GetResourceReader(CultureInfo info)
		{
			if (GetResourceService()) {
				return resourceService.GetResourceReader(info);
			}
			return null;
		}
		
		bool GetResourceService()
		{
			resourceService = (IResourceService)LoaderHost.GetService(typeof(IResourceService));
			return resourceService != null;
		}

		public IResourceWriter GetResourceWriter(CultureInfo info)
		{
			if (GetResourceService()) {
				return resourceService.GetResourceWriter(info);
			}
			return null;
		}
		
		/// <summary>
		/// Passes the designer host's root component to the generator so it can update the
		/// source code with changes made at design time.
		/// </summary>
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			generator.MergeRootComponentChanges(LoaderHost, serializationManager);
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			this.serializationManager = serializationManager;
			CreateComponents();
		}
		
		void CreateComponents()
		{
			IComponentWalker walker = CreateComponentWalker(this);
			walker.CreateComponent(generator.ViewContent.DesignerCodeFileContent);
		}
		
		protected virtual IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return null;
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
