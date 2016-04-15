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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Designer;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.AddIn.Options;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// IViewContent implementation that hosts the WPF designer.
	/// </summary>
	public class WpfViewContent : AbstractViewContentHandlingLoadErrors, IHasPropertyContainer, IToolsHost, IOutlineContentHost
	{
		public WpfViewContent(OpenedFile file) : base(file)
		{
			SharpDevelopTranslations.Init();
			
			BasicMetadata.Register();
			
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			this.IsActiveViewContentChanged += OnIsActiveViewContentChanged;
			
			var compilation = SD.ParserService.GetCompilationForFile(file.FileName);
			_path = Path.GetDirectoryName(compilation.MainAssembly.UnresolvedAssembly.Location);
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}
		
		static WpfViewContent()
		{
			DragDropExceptionHandler.UnhandledException += delegate(object sender, ThreadExceptionEventArgs e) {
				ICSharpCode.Core.MessageService.ShowException(e.Exception);
			};
		}
		
		DesignSurface designer;
		List<SDTask> tasks = new List<SDTask>();
		
		public DesignSurface DesignSurface {
			get { return designer; }
		}
		
		public DesignContext DesignContext {
			get { return designer.DesignContext; }
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			wasChangedInDesigner = false;
			Debug.Assert(file == this.PrimaryFile);
			SD.AnalyticsMonitor.TrackFeature(typeof(WpfViewContent), "Load");
			
			_stream = new MemoryStream();
			stream.CopyTo(_stream);
			stream.Position = 0;
			
			if (designer == null) {
				// initialize designer on first load
				designer = new DesignSurface();
				this.UserContent = designer;
				InitPropertyEditor();
				InitWpfToolbox();
			}
			this.UserContent = designer;
			if (outline != null) {
				outline.Root = null;
			}
			
			
			using (XmlTextReader r = new XmlTextReader(stream)) {
				XamlLoadSettings settings = new XamlLoadSettings();
				settings.DesignerAssemblies.Add(typeof(WpfViewContent).Assembly);
				settings.CustomServiceRegisterFunctions.Add(
					delegate(XamlDesignContext context) {
						context.Services.AddService(typeof(IUriContext), new FileUriContext(this.PrimaryFile));
						context.Services.AddService(typeof(IPropertyDescriptionService), new PropertyDescriptionService(this.PrimaryFile));
						context.Services.AddService(typeof(IEventHandlerService), new SharpDevelopEventHandlerService(this));
						context.Services.AddService(typeof(ITopLevelWindowService), new WpfAndWinFormsTopLevelWindowService());
						context.Services.AddService(typeof(ChooseClassServiceBase), new IdeChooseClassService());
					});
				settings.TypeFinder = MyTypeFinder.Create(this.PrimaryFile);
				settings.CurrentProjectAssemblyName = SD.ProjectService.CurrentProject.AssemblyName;
				
				try
				{
					if (WpfEditorOptions.EnableAppXamlParsing)
					{
						var appXaml = SD.ProjectService.CurrentProject.Items.FirstOrDefault(x => x.FileName.GetFileName().ToLower() == ("app.xaml"));
						if (appXaml != null)
						{
							var f = appXaml as FileProjectItem;
							OpenedFile a = SD.FileService.GetOrCreateOpenedFile(f.FileName);

							var xml = XmlReader.Create(a.OpenRead());
							var doc = new XmlDocument();
							doc.Load(xml);
							var node = doc.FirstChild.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "Application.Resources");

							foreach (XmlAttribute att in doc.FirstChild.Attributes.Cast<XmlAttribute>().ToList())
							{
								if (att.Name.StartsWith("xmlns")) {
									foreach (var childNode in node.ChildNodes.OfType<XmlNode>()) {
										childNode.Attributes.Append(att);
									}
								}
							}

							var appXamlXml = XmlReader.Create(new StringReader(node.InnerXml));
							var appxamlContext = new XamlDesignContext(appXamlXml, settings);
							
							//var parsed = XamlParser.Parse(appXamlXml, appxamlContext.ParserSettings);
							var dict = (ResourceDictionary) appxamlContext.RootItem.Component;// parsed.RootInstance;
							designer.DesignPanel.Resources.MergedDictionaries.Add(dict);
						}
					}
				}
				catch (Exception ex)
				{
					LoggingService.Error("Error in loading app.xaml", ex);
				}

				try
				{
					settings.ReportErrors = UpdateTasks;
					designer.LoadDesigner(r, settings);
					
					designer.DesignPanel.ContextMenuHandler = (contextMenu) => {
						var newContextmenu = new ContextMenu();
						var sdContextMenuItems = MenuService.CreateMenuItems(newContextmenu, designer, "/AddIns/WpfDesign/Designer/ContextMenu", "ContextMenu");
						foreach(var entry in sdContextMenuItems)
							newContextmenu.Items.Add(entry);
						newContextmenu.Items.Add(new Separator());
						
						var items = contextMenu.Items.Cast<Object>().ToList();
						contextMenu.Items.Clear();
						foreach(var entry in items)
							newContextmenu.Items.Add(entry);
						
						designer.DesignPanel.ContextMenu = newContextmenu;
					};
					
					if (outline != null && designer.DesignContext != null && designer.DesignContext.RootItem != null) {
						outline.Root = OutlineNode.Create(designer.DesignContext.RootItem);
					}
					
					propertyGridView.PropertyGrid.SelectedItems = null;
					designer.DesignContext.Services.Selection.SelectionChanged += OnSelectionChanged;
					designer.DesignContext.Services.GetService<UndoService>().UndoStackChanged += OnUndoStackChanged;
				} catch (Exception e) {
					this.UserContent = new WpfDocumentError(e);
				}
			}
		}
		
		System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var assemblyName = (new AssemblyName(args.Name));
			string fileName = Path.Combine(_path, assemblyName.Name) + ".dll";
			if (File.Exists(fileName))
				return typeResolutionService.LoadAssembly(fileName);
			return null;
		}
		
		private TypeResolutionService typeResolutionService = new TypeResolutionService();
		private string _path;
		private MemoryStream _stream;
		bool wasChangedInDesigner;
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			if (wasChangedInDesigner && designer.DesignContext != null) {
				SD.AnalyticsMonitor.TrackFeature(typeof(WpfViewContent), "Save");
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = SD.EditorControlService.GlobalOptions.IndentationString;
				settings.NewLineOnAttributes = true;
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings)) {
					designer.SaveDesigner(xmlWriter);
				}
			} else {
				_stream.Position = 0;
				using (var reader = new StreamReader(new UnclosableStream(_stream))) {
					using (var writer = new StreamWriter(stream)) {
						writer.Write(reader.ReadToEnd());
					}
				}
			}
		}
		
		public static List<SDTask> DllLoadErrors = new List<SDTask>();
		void UpdateTasks(XamlErrorService xamlErrorService)
		{
			Debug.Assert(xamlErrorService != null);
			foreach (SDTask task in tasks) {
				TaskService.Remove(task);
			}
			
			tasks.Clear();
			
			foreach (XamlError error in xamlErrorService.Errors) {
				var task = new SDTask(PrimaryFile.FileName, error.Message, error.Column - 1, error.Line, SharpDevelop.TaskType.Error);
				tasks.Add(task);
				TaskService.Add(task);
			}
			
			TaskService.AddRange(DllLoadErrors);
			
			if (xamlErrorService.Errors.Count != 0) {
				SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
		
		void OnUndoStackChanged(object sender, EventArgs e)
		{
			wasChangedInDesigner = true;
			this.PrimaryFile.MakeDirty();
		}
		
		#region Property editor / SelectionChanged
		
		PropertyGridView propertyGridView;
		
		void InitPropertyEditor()
		{
			propertyGridView = new PropertyGridView();
			propertyContainer.PropertyGridReplacementContent = propertyGridView;
			propertyGridView.PropertyGrid.PropertyChanged += OnPropertyGridPropertyChanged;
		}

		void InitWpfToolbox()
		{
			WpfToolbox.Instance.AddProjectDlls(Files[0]);
			SD.ProjectService.ProjectItemAdded += OnReferenceAdded;
		}
		
		void OnReferenceAdded(object sender, ProjectItemEventArgs e)
		{
			if (!(e.ProjectItem is ReferenceProjectItem)) return;
			if (e.Project != SD.ProjectService.FindProjectContainingFile(Files[0].FileName)) return;
			WpfToolbox.Instance.AddProjectDlls(Files[0]);
		}
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			propertyGridView.PropertyGrid.SelectedItems = DesignContext.Services.Selection.SelectedItems;
		}

		void OnPropertyGridPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (propertyGridView.PropertyGrid.ReloadActive) return;
			if (e.PropertyName == "Name") {
				if (!propertyGridView.PropertyGrid.IsNameCorrect) return;
				
				// get the XAML file
				OpenedFile file = this.Files.FirstOrDefault(f => f.FileName.ToString().EndsWith(".xaml", StringComparison.OrdinalIgnoreCase));
				if (file == null) return;
				
				// parse the XAML file
				ParseInformation info = SD.ParserService.Parse(file.FileName);
				if (info == null) return;
				ICompilation compilation = SD.ParserService.GetCompilationForFile(file.FileName);
				var designerClass = info.UnresolvedFile.TopLevelTypeDefinitions[0]
					.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly))
					.GetDefinition();
				if (designerClass == null) return;
				var reparseFileNameList = designerClass.Parts.Select(p => new ICSharpCode.Core.FileName(p.UnresolvedFile.FileName)).ToArray();
				
				// rename the member
				ISymbol controlSymbol = designerClass.GetFields(f => f.Name == propertyGridView.PropertyGrid.OldName, GetMemberOptions.IgnoreInheritedMembers)
					.SingleOrDefault();
				if (controlSymbol != null) {
					AsynchronousWaitDialog.ShowWaitDialogForAsyncOperation(
						"${res:SharpDevelop.Refactoring.Rename}",
						progressMonitor =>
						FindReferenceService.RenameSymbol(controlSymbol, propertyGridView.PropertyGrid.Name, progressMonitor)
						.ObserveOnUIThread()
						.Subscribe(error => SD.MessageService.ShowError(error.Message), // onNext
						           ex => SD.MessageService.ShowException(ex), // onError
						           // onCompleted
						           () => {
						           	foreach (var fileName in reparseFileNameList) {
						           		SD.ParserService.ParseAsync(fileName).FireAndForget();
						           	}
						           }
						          )
					);

				}
			}
		}
		
		static bool IsCollectionWithSameElements(ICollection<DesignItem> a, ICollection<DesignItem> b)
		{
			return ContainsAll(a, b) && ContainsAll(b, a);
		}
		
		static bool ContainsAll(ICollection<DesignItem> a, ICollection<DesignItem> b)
		{
			foreach (DesignItem item in a) {
				if (!b.Contains(item))
					return false;
			}
			return true;
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get { return propertyContainer; }
		}
		#endregion
		
		public object ToolsContent {
			get { return WpfToolbox.Instance.ToolboxControl; }
		}
		
		public override void Dispose()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			SD.ProjectService.ProjectItemAdded -= OnReferenceAdded;

			propertyContainer.Clear();
			base.Dispose();
		}
		
		void OnIsActiveViewContentChanged(object sender, EventArgs e)
		{
			if (IsActiveViewContent) {
				if (designer != null && designer.DesignContext != null) {
					WpfToolbox.Instance.ToolService = designer.DesignContext.Services.Tool;
				}
			}
		}
		
		Outline outline;
		
		public Outline Outline {
			get {
				if (outline == null) {
					outline = new Outline();
					if (DesignSurface != null && DesignSurface.DesignContext != null && DesignSurface.DesignContext.RootItem != null) {
						outline.Root = OutlineNode.Create(DesignSurface.DesignContext.RootItem);
					}
					// see 3522
					outline.AddCommandHandler(ApplicationCommands.Delete,
					                          () => ApplicationCommands.Delete.Execute(null, designer));
				}
				return outline;
			}
		}
		
		public object OutlineContent {
			get { return this.Outline; }
		}
	}
}
