#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.IO;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
    public class EDMDesignerViewContent : AbstractViewContent, IHasPropertyContainer, IToolsHost
    {
        #region Fields

        private DesignerCanvas _designerCanvas = null;
        private PropertyContainer _propertyContainer = new PropertyContainer();
        private EDMView _edmView = null;
        private object _selection = null;

        #endregion

        #region Properties

        public object Selection
        {
            get { return _selection; }
            set
            {
                if (_selection == null)
                    _propertyContainer.Clear();
                else
                    _propertyContainer.SelectedObject = value;

                _selection = value;
            }
        }

        public Window Window
        {
            get { return Application.Current.MainWindow; }
        }

        public EDMView EDMView
        {
            get { return _edmView; }
        }
		
		public override object Control 
        {
            get { return _designerCanvas; }
		}

        #endregion

        #region Constructor

        public EDMDesignerViewContent(OpenedFile primaryFile)
            : base(primaryFile)
		{           
            if (primaryFile == null)
				throw new ArgumentNullException("primaryFile");
			
			primaryFile.ForceInitializeView(this); // call Load()			
        }

        #endregion

        #region Methods

        public override void Load(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);

            XElement edmxElement = null;
            Action<XElement> readMoreAction = edmxElt => edmxElement = edmxElt;            
            _edmView = new EDMView(file.FileName, readMoreAction);

            EntityTypeDesigner.Init = true;

            DesignerIO.Read(_edmView, edmxElement.Element("DesignerViews"), entityType => new EntityTypeDesigner(entityType), complexType => new ComplexTypeDesigner(complexType));
            
            EntityTypeDesigner.Init = false;

            VisualHelper.DoEvents();

            _designerCanvas = DesignerCanvas.GetDesignerCanvas(this, _edmView, _edmView.DesignerViews.FirstOrDefault());

            CSDLDatabaseTreeViewAdditionalNode.Instance.CSDLViews.Add(_edmView.CSDL);
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);
            //Writer.Write(_designerCanvas.EDMView.EDM, EDMDesignerView.Writer.Write(_designerCanvas.EDMView)).Save(file.FileName);
		}

        internal void ShowPropertiesTab()
        { }

        internal void ShowPropertiesTab(IUIType type)
        { }

        internal void ShowPropertiesTab(UIProperty property)
        { }

        internal void ShowMappingTab()
        { }

        internal void ShowMappingTab(IUIType type)
        { }

        public override void Dispose()
        {
            if (CSDLDatabaseTreeViewAdditionalNode.Instance.CSDLViews.Contains(_edmView.CSDL))
                CSDLDatabaseTreeViewAdditionalNode.Instance.CSDLViews.Remove(_edmView.CSDL);
        }

        #endregion

        #region IHasPropertyContainer

        public PropertyContainer PropertyContainer 
        {
			get { return _propertyContainer; }
		}

		#endregion

        #region IToolsHost

        object IToolsHost.ToolsContent 
        {
			get { return null; }
        }

        #endregion
    }
}
