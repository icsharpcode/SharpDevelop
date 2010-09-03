// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
