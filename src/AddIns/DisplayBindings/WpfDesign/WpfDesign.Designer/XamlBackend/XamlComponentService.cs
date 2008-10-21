using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.Xaml;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	class XamlComponentService : IComponentService
	{		
		public XamlComponentService(XamlDesignContext context)
		{
			this.context = context;
		}

		XamlDesignContext context;

		DesignItem RegisterComponentForDesigner(ObjectNode objectNode)
		{			
			XamlDesignItem item = new XamlDesignItem(objectNode, context);
			objectNode.Annotate<DesignItem>(item);
			if (ComponentRegistered != null) 
			{
				ComponentRegistered(this, new DesignItemEventArgs(item));
			}
			return item;
		}
		
		public DesignItem RegisterXamlComponentRecursive(ObjectNode objectNode)
		{
			foreach (var node in objectNode.Descendants().OfType<ObjectNode>()) 
			{
				RegisterComponentForDesigner(node);
			}
			return RegisterComponentForDesigner(objectNode);
		}

		#region IComponentService Members

		public event EventHandler<DesignItemEventArgs> ComponentRegistered;
		
		public DesignItem RegisterComponentForDesigner(object component)
		{	
			return RegisterComponentForDesigner(context.Document.CreateObject(component));
		}

		#endregion
	}
}
