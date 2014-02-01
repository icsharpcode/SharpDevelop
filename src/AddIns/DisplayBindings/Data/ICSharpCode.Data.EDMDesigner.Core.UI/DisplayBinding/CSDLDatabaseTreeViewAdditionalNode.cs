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

using System.Collections;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.UI.UserControls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
    public class CSDLDatabaseTreeViewAdditionalNode : IDatabasesTreeViewAdditionalNode
    {
        #region Fields

        private static CSDLDatabaseTreeViewAdditionalNode _instance = null;
        private ObservableCollection<CSDLView> _csdlViews = new ObservableCollection<CSDLView>();

        #endregion

        #region Constructor

        static CSDLDatabaseTreeViewAdditionalNode()
        {
            ResourceDictionaryLoader.LoadResourceDictionary("/DisplayBinding/CSDLDatabaseTreeViewNodeResourceDictionary.xaml");
        }

        #endregion

        #region Properties

        public static CSDLDatabaseTreeViewAdditionalNode Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CSDLDatabaseTreeViewAdditionalNode();
                }

                return _instance;
            }
        }

        public string Name
        {
            get { return "CSDL Models"; }
        }

        public string DataTemplate
        {
            get { return "CSDLViewTreeViewDataTemplate"; }
        }

        public IEnumerable Items
        {
            get { return _csdlViews; }
        }

        internal ObservableCollection<CSDLView> CSDLViews
        {
            get { return _csdlViews; }
        }

        #endregion
    }
}
