// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
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
			
			WpfToolbox.Instance.AddProjectDlls(file);

			ProjectService.ProjectItemAdded += ProjectService_ProjectItemAdded;
			
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			this.IsActiveViewContentChanged += OnIsActiveViewContentChanged;
		}

		void ProjectService_ProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.ProjectItem is ReferenceProjectItem)
				WpfToolbox.Instance.AddProjectDlls(this.Files[0]);
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
			
			_stream = new MemoryStream();
			stream.CopyTo(_stream);
			stream.Position = 0;
			
			if (designer == null) {
				// initialize designer on first load
				designer = new DesignSurface();
				this.UserContent = designer;
				InitPropertyEditor();
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
				try {
					settings.ReportErrors = UpdateTasks;
					designer.LoadDesigner(r, settings);
					
					designer.ContextMenuOpening += (sender, e) => MenuService.ShowContextMenu(e.OriginalSource as UIElement, designer, "/AddIns/WpfDesign/Designer/ContextMenu");
					
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
		
		private MemoryStream _stream;
		bool wasChangedInDesigner;
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			if (wasChangedInDesigner && designer.DesignContext != null) {
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
					FindReferenceService.RenameSymbol(controlSymbol, propertyGridView.PropertyGrid.Name, new DummyProgressMonitor())
						.ObserveOnUIThread()
						.Subscribe(error => SD.MessageService.ShowError(error.Message), // onNext
						           ex => SD.MessageService.ShowException(ex), // onError
						           // onCompleted
						           () => {
						           	foreach (var fileName in reparseFileNameList) {
						           		SD.ParserService.ParseAsync(fileName).FireAndForget();
						           	}
						           }
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
			ProjectService.ProjectItemAdded -= ProjectService_ProjectItemAdded;

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
