#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
    public class EDMDesignerDisplayBinding : IDisplayBinding
    {
        public bool CanCreateContentForFile(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".edmx", StringComparison.OrdinalIgnoreCase);
        }

        public IViewContent CreateContentForFile(OpenedFile file)
        {
            return new EDMDesignerViewContent(file);
        }
    }
}
