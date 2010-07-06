using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AvalonDock
{
    internal class FloatingDocumentPane : DocumentPane
    {
        static FloatingDocumentPane()
        {
            Pane.ShowHeaderProperty.OverrideMetadata(typeof(FloatingDocumentPane), new FrameworkPropertyMetadata(false));
        }
        
        internal FloatingDocumentPane(DocumentFloatingWindow floatingWindow, DocumentContent documentToTransfer)
        {
            _floatingWindow = floatingWindow;
            _documentToTransfer = documentToTransfer;
        }

        DocumentContent _documentToTransfer = null;
        DocumentPane _previousPane = null;

        internal DocumentPane PreviousPane
        {
            get
            { return _previousPane; }
        }

        int _arrayIndexPreviousPane = -1;

        internal int ArrayIndexPreviousPane
        {
            get { return _arrayIndexPreviousPane; }
        }

        protected override void OnInitialized(EventArgs e)
        {
            _previousPane = _documentToTransfer.ContainerPane as DocumentPane;

            if (_documentToTransfer != null && _documentToTransfer.FloatingWindowSize.IsEmpty)
            {
                if (_previousPane != null)
                    _documentToTransfer.FloatingWindowSize = new Size(_previousPane.ActualWidth, _previousPane.ActualHeight);
                else
                    _documentToTransfer.FloatingWindowSize = new Size(400.0, 400.0);
            }

            if (_documentToTransfer != null && !_documentToTransfer.FloatingWindowSize.IsEmpty)
            {
                _floatingWindow.Width = _documentToTransfer.FloatingWindowSize.Width;
                _floatingWindow.Height = _documentToTransfer.FloatingWindowSize.Height;
            }


            if (_previousPane != null)
            {
                //setup window size
                _floatingWindow.Width = _documentToTransfer.ContainerPane.ActualWidth;
                _floatingWindow.Height = _documentToTransfer.ContainerPane.ActualHeight;

                //save current content position in container pane
                _arrayIndexPreviousPane = _previousPane.Items.IndexOf(_documentToTransfer);
                SetValue(ResizingPanel.ResizeWidthProperty, _previousPane.GetValue(ResizingPanel.ResizeWidthProperty));
                SetValue(ResizingPanel.ResizeHeightProperty, _previousPane.GetValue(ResizingPanel.ResizeHeightProperty));

                //Style = _previousPane.Style;
                AttachStyleFromPane(_previousPane);

                //remove content from container pane
                _previousPane.RemoveContent(_arrayIndexPreviousPane);
            }
            

            //add content to my temporary pane
            Items.Add(_documentToTransfer);

            _documentToTransfer.SetIsFloating(true);

            LayoutTransform = (MatrixTransform)_documentToTransfer.TansformToAncestor();

            base.OnInitialized(e);
        }

        void AttachStyleFromPane(DocumentPane copyFromPane)
        {
            if (copyFromPane == null)
                return;

            //Binding bnd = new Binding("Style");
            //bnd.Source = copyFromPane;
            //bnd.Mode = BindingMode.OneWay;

            //SetBinding(StyleProperty, bnd);
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _documentToTransfer.SetIsFloating(false);
            base.OnUnloaded(sender, e);
        }
        

        DocumentFloatingWindow _floatingWindow = null;

        public DocumentFloatingWindow FloatingWindow
        {
            get { return _floatingWindow; }
        }

        public override DockingManager GetManager()
        {
            return _floatingWindow.Manager;
        }

        protected override bool IsSurfaceVisible
        {
            get
            {
                return false;
            }
        }

        protected override void CheckItems(System.Collections.IList newItems)
        {
            foreach (object newItem in newItems)
            {
                if (!(newItem is DocumentContent))
                    throw new InvalidOperationException("FloatingDocumentPane can contain only DocumentContents!");
            }

            if (Items.Count == 0 && FloatingWindow != null)
                FloatingWindow.Close(true);
        }

        public override void Dock()
        {
            var contentsToRedock = Items.Cast<DocumentContent>().ToArray();

            foreach (var cntToRedock in contentsToRedock)
                cntToRedock.Show();
            
            base.Dock();
        }
    }
}
