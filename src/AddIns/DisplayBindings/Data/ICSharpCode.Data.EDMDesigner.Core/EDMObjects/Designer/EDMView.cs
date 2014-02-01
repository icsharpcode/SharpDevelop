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
