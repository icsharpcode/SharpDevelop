// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.Core.Implementation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;
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
			var container = new SharpDevelopServiceContainer();
			container.AddFallbackProvider(ServiceSingleton.FallbackServiceProvider);
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
			var disposableServiceContainer = ServiceSingleton.ServiceProvider as IDisposable;
			if (disposableServiceContainer != null)
				disposableServiceContainer.Dispose();
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
		/// Returns a task that gets completed when the service is initialized.
		/// 
		/// This method does not try to initialize the service -- if no other code forces the service
		/// to be initialized, the task will never complete.
		/// </summary>
		/// <remarks>
		/// This method can be used to solve cyclic dependencies in service initialization.
		/// </remarks>
		public static Task<T> GetFutureService<T>() where T : class
		{
			return GetRequiredService<SharpDevelopServiceContainer>().GetFutureService<T>();
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
		
		/// <inheritdoc see="IWorkbench"/>
		public static IWorkbench Workbench {
			get { return GetRequiredService<IWorkbench>(); }
		}
		
		/// <summary>
		/// Gets the <see cref="IMessageLoop"/> representing the main UI thread.
		/// </summary>
		public static IMessageLoop MainThread {
			get { return GetRequiredService<IMessageLoop>(); }
		}
		
		/// <inheritdoc see="IStatusBarService"/>
		public static IStatusBarService StatusBar {
			get { return GetRequiredService<IStatusBarService>(); }
		}
		
		/// <inheritdoc see="ILoggingService"/>
		public static ILoggingService Log {
			get { return GetRequiredService<ILoggingService>(); }
		}
		
		/// <inheritdoc see="IMessageService"/>
		public static IMessageService MessageService {
			get { return GetRequiredService<IMessageService>(); }
		}
		
		/// <inheritdoc see="IPropertyService"/>
		public static IPropertyService PropertyService {
			get { return GetRequiredService<IPropertyService>(); }
		}
		
		/// <inheritdoc see="Core.IResourceService"/>
		public static Core.IResourceService ResourceService {
			get { return GetRequiredService<Core.IResourceService>(); }
		}
		
		/// <inheritdoc see="IEditorControlService"/>
		public static IEditorControlService EditorControlService {
			get { return GetRequiredService<IEditorControlService>(); }
		}
		
		/// <inheritdoc see="IAnalyticsMonitor"/>
		public static IAnalyticsMonitor AnalyticsMonitor {
			get { return GetRequiredService<IAnalyticsMonitor>(); }
		}
		
		/// <inheritdoc see="IParserService"/>
		public static IParserService ParserService {
			get { return GetRequiredService<IParserService>(); }
		}
		
		/// <inheritdoc see="IAssemblyParserService"/>
		public static IAssemblyParserService AssemblyParserService {
			get { return GetRequiredService<IAssemblyParserService>(); }
		}
		
		/// <inheritdoc see="IFileService"/>
		public static IFileService FileService {
			get { return GetRequiredService<IFileService>(); }
		}
		
		/// <inheritdoc see="IGlobalAssemblyCacheService"/>
		public static IGlobalAssemblyCacheService GlobalAssemblyCache {
			get { return GetRequiredService<IGlobalAssemblyCacheService>(); }
		}
		
		/// <inheritdoc see="IAddInTree"/>
		public static IAddInTree AddInTree {
			get { return GetRequiredService<IAddInTree>(); }
		}
		
		/// <inheritdoc see="IShutdownService"/>
		public static IShutdownService ShutdownService {
			get { return GetRequiredService<IShutdownService>(); }
		}
		
		/// <inheritdoc see="ITreeNodeFactory"/>
		public static ITreeNodeFactory TreeNodeFactory {
			get { return GetRequiredService<ITreeNodeFactory>(); }
		}
		
		/// <inheritdoc see="IClipboard"/>
		public static IClipboard Clipboard {
			get { return GetRequiredService<IClipboard>(); }
		}
		
		/// <inheritdoc see="IWinFormsService"/>
		public static IWinFormsService WinForms {
			get { return GetRequiredService<IWinFormsService>(); }
		}
		
		/// <inheritdoc see="IBuildService"/>
		public static IBuildService BuildService {
			get { return GetRequiredService<IBuildService>(); }
		}
		
		/// <inheritdoc see="IBookmarkManager"/>
		public static IBookmarkManager BookmarkManager {
			get { return GetRequiredService<IBookmarkManager>(); }
		}
		
		/// <inheritdoc see="IDisplayBindingService"/>
		public static IDisplayBindingService DisplayBindingService {
			get { return GetRequiredService<IDisplayBindingService>(); }
		}
		
		/// <inheritdoc see="IProjectService"/>
		public static IProjectService ProjectService {
			get { return GetRequiredService<IProjectService>(); }
		}
		
		/// <inheritdoc see="ILanguageService"/>
		public static ILanguageService LanguageService {
			get { return GetRequiredService<ILanguageService>(); }
		}
		
		/// <inheritdoc see="IUIService"/>
		public static IUIService UIService {
			get { return GetRequiredService<IUIService>(); }
		}
		
		/// <inheritdoc see="IMSBuildEngine"/>
		public static IMSBuildEngine MSBuildEngine {
			get { return GetRequiredService<IMSBuildEngine>(); }
		}
		
		/// <inheritdoc see="ITemplateService"/>
		public static ITemplateService Templates {
			get { return GetRequiredService<ITemplateService>(); }
		}
		
		/// <inheritdoc see="IFileSystem"/>
		public static IFileSystem FileSystem {
			get { return GetRequiredService<IFileSystem>(); }
		}
		
		/// <inheritdoc see="IOutputPad"/>
		public static IOutputPad OutputPad {
			get { return GetRequiredService<IOutputPad>(); }
		}
		
		/// <inheritdoc see="IClassBrowser"/>
		public static IClassBrowser ClassBrowser {
			get { return GetRequiredService<IClassBrowser>(); }
		}
	}
}
