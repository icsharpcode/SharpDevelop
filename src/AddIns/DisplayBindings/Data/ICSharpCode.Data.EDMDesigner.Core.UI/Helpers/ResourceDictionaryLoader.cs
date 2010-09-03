// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using System.Windows;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Helpers
{
    internal class ResourceDictionaryLoader
    {
        public static void LoadResourceDictionary(string resourceDictionaryPath)
        {
            if (Application.Current == null || Application.Current.Resources == null || Application.Current.Resources.MergedDictionaries == null)
                return;
            
            Uri resourceDictionaryUri = new Uri("/ICSharpCode.Data.EDMDesigner.Core.UI;component" + resourceDictionaryPath, UriKind.RelativeOrAbsolute);

            if (Application.Current.Resources.MergedDictionaries.FirstOrDefault(resourceDictionary => resourceDictionary.Source.OriginalString == resourceDictionaryUri.OriginalString) == null)
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = resourceDictionaryUri;
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}
