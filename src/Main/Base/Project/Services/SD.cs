// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using ICSharpCode.Core;
using ICSharpCode.Core.Implementation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Static entry point for retrieving SharpDevelop services.
	/// </summary>
	public static class SD
	{
		/// <summary>
		/// Gets the main service container for SharpDevelop.
		/// </summary>
		public static IServiceContainer Services {
			get { return GetRequiredService<IServiceContainer>(); }
		}
		
		/// <summary>
		/// Initializes the services for unit testing.
		/// This will replace the whole service container with a new container that
		/// contains only the following services:
		/// - ILoggingService (logging to Diagnostics.Trace)
		/// - IMessageService (writing to Console.Out)
		/// - IPropertyService (empty in-memory property container)
		/// - AddInTree (empty tree with no AddIns loaded)
		/// </summary>
		public static void InitializeForUnitTests()
		{
			var container = new SharpDevelopServiceContainer(ServiceSingleton.FallbackServiceProvider);
			container.AddService(typeof(IPropertyService), new PropertyServiceImpl());
			container.AddService(typeof(IAddInTree), new AddInTreeImpl(null));
			ServiceSingleton.ServiceProvider = container;
		}
		
		/// <summary>
		/// Removes the static SharpDevelop service container, returning to the
		/// state before <see cref="InitializeForUnitTests"/> was called.
		/// </summary>
		public static void TearDownForUnitTests()
		{
			ServiceSingleton.ServiceProvider = ServiceSingleton.FallbackServiceProvider;
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetService<T>() where T : class
		{
			return ServiceSingleton.ServiceProvider.GetService<T>();
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetRequiredService<T>() where T : class
		{
			return ServiceSingleton.ServiceProvider.GetRequiredService<T>();
		}
		
		/// <summary>
		/// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService&lt;T&gt;()</code>,
		/// but does not throw a NullReferenceException when ActiveViewContent is null.
		/// (instead, null is returned).
		/// </summary>
		public static T GetActiveViewContentService<T>() where T : class
		{
			return (T)GetActiveViewContentService(typeof(T));
		}
		
		/// <summary>
		/// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService(type)</code>,
		/// but does not throw a NullReferenceException when ActiveViewContent is null.
		/// (instead, null is returned).
		/// </summary>
		public static object GetActiveViewContentService(Type type)
		{
			var workbench = ServiceSingleton.ServiceProvider.GetService(typeof(IWorkbench)) as IWorkbench;
			if (workbench != null) {
				var activeViewContent = workbench.ActiveViewContent;
				if (activeViewContent != null) {
					return activeViewContent.GetService(type);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the workbench.
		/// </summary>
		public static IWorkbench Workbench {
			get { return GetRequiredService<IWorkbench>(); }
		}
		
		public static IMessageLoop MainThread {
			get { return GetRequiredService<IMessageLoop>(); }
		}
		
		/// <summary>
		/// Gets the status bar.
		/// </summary>
		public static IStatusBarService StatusBar {
			get { return GetRequiredService<IStatusBarService>(); }
		}
		
		public static ILoggingService Log {
			get { return GetRequiredService<ILoggingService>(); }
		}
		
		public static IMessageService MessageService {
			get { return GetRequiredService<IMessageService>(); }
		}
		
		public static IPropertyService PropertyService {
			get { return GetRequiredService<IPropertyService>(); }
		}
		
		public static Core.IResourceService ResourceService {
			get { return GetRequiredService<Core.IResourceService>(); }
		}
		
		public static IEditorControlService EditorControlService {
			get { return GetRequiredService<IEditorControlService>(); }
		}
		
		public static IAnalyticsMonitor AnalyticsMonitor {
			get { return GetRequiredService<IAnalyticsMonitor>(); }
		}
		
		public static IParserService ParserService {
			get { return GetRequiredService<IParserService>(); }
		}
		
		public static IAssemblyParserService AssemblyParserService {
			get { return GetRequiredService<IAssemblyParserService>(); }
		}
		
		public static IFileService FileService {
			get { return GetRequiredService<IFileService>(); }
		}
		
		public static IGlobalAssemblyCacheService GlobalAssemblyCache {
			get { return GetRequiredService<IGlobalAssemblyCacheService>(); }
		}
		
		public static IAddInTree AddInTree {
			get { return GetRequiredService<IAddInTree>(); }
		}
		
		public static IShutdownService ShutdownService {
			get { return GetRequiredService<IShutdownService>(); }
		}
		
		public static ITreeNodeFactory TreeNodeFactory {
			get { return GetRequiredService<ITreeNodeFactory>(); }
		}
		
		public static IClipboard Clipboard {
			get { return GetRequiredService<IClipboard>(); }
		}
		
		public static IWinFormsService WinForms {
			get { return GetRequiredService<IWinFormsService>(); }
		}
		
		public static IBuildService BuildService {
			get { return GetRequiredService<IBuildService>(); }
		}
	}
}
