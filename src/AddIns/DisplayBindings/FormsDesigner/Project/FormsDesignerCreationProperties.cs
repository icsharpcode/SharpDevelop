// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerCreationProperties : MarshalByRefObject
	{
		public string FileName { get; set; }
		public ITypeLocator TypeLocator { get; set; }
		public IGacWrapper GacWrapper { get; set; }
		public ICommandProvider Commands { get; set; }
		public IFormsDesigner FormsDesignerProxy { get; set; }
		public IFormsDesignerLoggingService Logger { get; set; }
		public SharpDevelopDesignerOptions Options { get; set; }
		public IResourceStore ResourceStore { get; set; }
	}
}
