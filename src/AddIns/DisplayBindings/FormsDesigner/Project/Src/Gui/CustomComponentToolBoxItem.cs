// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Runtime.Serialization;

using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner.Gui
{
	[Serializable]
	public class CustomComponentToolboxItem : ToolboxItem
	{
		string sourceFileName;
		string className;
		bool initialized;
		
		public CustomComponentToolboxItem(string sourceFileName, string className)
		{
			this.sourceFileName = sourceFileName;
			this.className = className;
			this.Bitmap = new ToolboxItem(typeof(Component)).Bitmap;
			this.IsTransient = true;
		}
		
		protected CustomComponentToolboxItem(SerializationInfo info, StreamingContext context)
		{
			Deserialize(info, context);
		}
		
		protected override void Deserialize(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.Deserialize(info, context);
			sourceFileName = info.GetString("sourceFileName");
			className = info.GetString("className");
			initialized = info.GetBoolean("initialized");
		}
		
		protected override void Serialize(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.Serialize(info, context);
			info.AddValue("sourceFileName", sourceFileName);
			info.AddValue("className", className);
			info.AddValue("initialized", initialized);
		}
		
		void Init(IDesignerHost host)
		{
			((IFormsDesignerLoggingService)host.GetService(typeof(IFormsDesignerLoggingService))).Debug("Initializing MyToolBoxItem: " + className);
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
