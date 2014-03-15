/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.02.2014
 * Time: 19:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.ComponentModel.Design;

using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignerBinding;
using ICSharpCode.Reporting.Addin.Services;

namespace ICSharpCode.Reporting.Addin.Views
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class DesignerView : AbstractViewContent
	{
		readonly IDesignerGenerator generator;
		bool unloading;
		Panel panel;
		ReportDesignerLoader loader;
		DefaultServiceContainer defaultServiceContainer;
		DesignSurface designSurface;
		
		public DesignerView()
		{
		}
		
		
		public DesignerView (OpenedFile openedFile,IDesignerGenerator generator) : base(openedFile){
			if (openedFile == null) {
				throw new ArgumentNullException("openedFile");
			}
			LoggingService.Info("DesignerView: Load from: " + openedFile.FileName);
			
			TabPageText = ResourceService.GetString("SharpReport.Design");
			
			this.generator = generator;
			this.generator.Attach(this);
			
		}

	
		void LoadDesigner (Stream stream) {
			LoggingService.Info("ReportDesigner LoadDesigner_Start");
			panel = CreatePanel();
			defaultServiceContainer = CreateAndInitServiceContainer();
			
			LoggingService.Info("Create DesignSurface and add event's");
			designSurface = CreateDesignSurface(defaultServiceContainer);
			SetDesignerEvents();
			
			var ambientProperties = new AmbientProperties();
			defaultServiceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			
			defaultServiceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService());
			defaultServiceContainer.AddService(typeof(ITypeDiscoveryService),new TypeDiscoveryService());
			                                   
			defaultServiceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService),
				new ICSharpCode.Reporting.Addin.Services.MenuCommandService(panel,this.designSurface ));
			
			defaultServiceContainer.AddService(typeof(MemberRelationshipService),new DefaultMemberRelationshipService());
			defaultServiceContainer.AddService(typeof(OpenedFile),base.PrimaryFile);
			
			LoggingService.Info("Load DesignerOptionService");
			var designerOptionService = CreateDesignerOptions();
			defaultServiceContainer.AddService( typeof( DesignerOptionService ), designerOptionService );
			
			LoggingService.Info("Create ReportDesignerLoader"); 
			
			this.loader = new ReportDesignerLoader(generator,stream);
			this.designSurface.BeginLoad(this.loader);
			if (!designSurface.IsLoaded) {
				//				throw new FormsDesignerLoadException(FormatLoadErrors(designSurface));
				LoggingService.Error("designer not loaded");
			}
          
			LoggingService.Info("ReportDesigner LoadDesigner_End");
		}	

		
		Panel CreatePanel ()
		{
			var ctl = new Panel();
			ctl.Dock = DockStyle.Fill;
			ctl.BackColor = System.Drawing.Color.LightBlue;
			return ctl;
		}

		
		DefaultServiceContainer CreateAndInitServiceContainer()
		{
			var serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(IUIService), new UIService());
			serviceContainer.AddService(typeof(IToolboxService),new ToolboxService());
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			return serviceContainer;
		}
		
		
		void SetDesignerEvents()
		{
			designSurface.Loading += DesignerLoading;
			designSurface.Loaded += DesignerLoaded;
			designSurface.Flushed += DesignerFlushed;
			designSurface.Unloading += DesingerUnloading;
		}
		
		
		static WindowsFormsDesignerOptionService CreateDesignerOptions()
		{
			var designerOptionService = new System.Windows.Forms.Design.WindowsFormsDesignerOptionService();
			designerOptionService.Options.Properties.Find("UseSmartTags", true).SetValue(designerOptionService, true);
			designerOptionService.Options.Properties.Find("ShowGrid", true).SetValue(designerOptionService, false);
			designerOptionService.Options.Properties.Find("UseSnapLines", true).SetValue(designerOptionService, true);
			return designerOptionService;
		}
	
		#region DesignerEvents
		
		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: DesignerLoader loading...");
			this.unloading = false;
		}
		
		
		void DesignerLoaded(object sender, LoadedEventArgs e)
		{
			LoggingService.Debug("ReportDesigner: DesignerLoaded...");
		}

		void DesignerFlushed(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: DesignerFlushed");
		}

		
		void DesingerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: DesignernUnloading...");
		}
		
		#endregion
		
		#region Design surface manager (static)
		
		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();
		
		static DesignSurface CreateDesignSurface(IServiceProvider serviceProvider)
		{
			return designSurfaceManager.CreateDesignSurface(serviceProvider);
		}
		
		#endregion
		
		
		#region overrides
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override object Control {
			get {return panel;}
		}
		
		
		/// <summary>
		/// Creates a new DesignerView object
		/// </summary>
		
		
		/// <summary>
		/// Loads a new file into MyView
		/// </summary>

		public override void Load(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("ReportDesigner: Load from: " + file.FileName);
			base.Load(file, stream);
			LoadDesigner(stream);
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			// TODO: Clean up resources in this method
			// Control.Dispose();
			base.Dispose();
		}
		
		#endregion
	}
	
}
