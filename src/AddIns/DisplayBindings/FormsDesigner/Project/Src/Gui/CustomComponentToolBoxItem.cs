using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

using ICSharpCode.FormsDesigner.Services;

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.FormsDesigner.Gui
{
	public class CustomComponentToolBoxItem : ToolboxItem
	{
		string sourceFileName;
		string className;
		bool initialized;
		
		public CustomComponentToolBoxItem(string sourceFileName, string className)
		{
			this.sourceFileName = sourceFileName;
			this.className = className;
			this.Bitmap = new ToolboxItem(typeof(Component)).Bitmap;
			this.IsTransient = true;
		}
		
		void Init(IDesignerHost host)
		{
			LoggingService.Debug("Initializing MyToolBoxItem: " + className);
			if (host == null) throw new ArgumentNullException("host");
			if (sourceFileName != null) {
				TypeResolutionService typeResolutionService = host.GetService(typeof(ITypeResolutionService)) as TypeResolutionService;
				if (typeResolutionService == null) {
					throw new InvalidOperationException("Cannot initialize CustomComponentToolBoxItem because the designer host does not provide a SharpDevelop TypeResolutionService.");
				}
				
				if (!initialized) {
					Initialize(typeResolutionService.GetType(className));
					initialized = true;
				}
			}
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			Init(host);
			return base.CreateComponentsCore(host);
		}
		
		protected override IComponent[] CreateComponentsCore(IDesignerHost host, System.Collections.IDictionary defaultValues)
		{
			Init(host);
			return base.CreateComponentsCore(host, defaultValues);
		}
	}
}
