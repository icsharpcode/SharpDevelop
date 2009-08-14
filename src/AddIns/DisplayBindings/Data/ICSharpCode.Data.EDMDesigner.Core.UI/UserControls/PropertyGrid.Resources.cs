#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    partial class PropertyGrid
    {
        public static string Description
        {
            get { return "Description"; }
        }

        private string GetPropertyName(string propertyName)
        {
            return null;// PropertyGridResources.ResourceManager.GetString(propertyName, PropertyGridResources.Culture);
        }

        private string GetPropertyDescription(string propertyName)
        {
            return null;// PropertyGridResources.ResourceManager.GetString(string.Concat(propertyName, "Description"), PropertyGridResources.Culture);
        }
    }
}
