// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.IO;
using System.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer
{
    public class EDMView : EDMObjectBase
    {
        private CSDLView _csdl;
        private SSDLView _ssdl;
        private EventedObservableCollection<DesignerView> _designerViews;

        public EDMView(string edmxPath, Action<XElement> readMoreAction)
        {
            EDM = EDMXIO.Read(edmxPath, readMoreAction);
        }

        public EDMView(Stream stream, Action<XElement> readMoreAction)
        {
            EDM = EDMXIO.Read(stream, readMoreAction);
        }
        
        public EDMView(XDocument edmxDocument, Action<XElement> readMoreAction)
        {
        	EDM = EDMXIO.Read(edmxDocument, readMoreAction);
        }

        public EDM EDM { get; private set; }

        public CSDLView CSDL
        {
            get
            {
                if (_csdl == null)
                    _csdl = new CSDLView { CSDL = EDM.CSDLContainer, EDMView = this };
                return _csdl;
            }
        }

        public SSDLView SSDL
        {
            get
            {
                if (_ssdl == null)
                    _ssdl = new SSDLView { SSDL = EDM.SSDLContainer };
                return _ssdl;
            }
        }

        public EventedObservableCollection<DesignerView> DesignerViews
        {
            get
            {
                if (_designerViews == null)
                {
                    _designerViews = new EventedObservableCollection<DesignerView>();
                    _designerViews.ItemAdded += designerView =>
                        {
                            Action<ITypeDesigner> typeDeleted = typeDesigner => 
                                {
                                    CSDL.TypeDeleted += entityType =>
                                    {
                                        if (entityType == typeDesigner.UIType)
                                            designerView.TypeDesignersLocations.Remove(typeDesigner);
                                    };
                                };

                            foreach (var typeDesigner in designerView.TypeDesignersLocations)
                                typeDeleted(typeDesigner);
                            designerView.TypeDesignersLocations.ItemAdded += typeDeleted;
                        };
                }
                return _designerViews;
            }
        }
    }
}
