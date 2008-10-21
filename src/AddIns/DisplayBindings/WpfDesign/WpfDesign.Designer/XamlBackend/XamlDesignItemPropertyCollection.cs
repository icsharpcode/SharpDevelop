using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	class XamlDesignItemPropertyCollection : DesignItemPropertyCollection
	{
		public XamlDesignItemPropertyCollection(XamlDesignItem item)
		{
			this.item = item;
		}

		XamlDesignItem item;

		public override DesignItemProperty GetProperty(string name)
		{
			return XamlDesignItemProperty.Create(item, item.ObjectNode.Property(name));
		}

		public override DesignItemProperty GetAttachedProperty(Type ownerType, string name)
		{
			return XamlDesignItemProperty.Create(item, item.ObjectNode.Property(ownerType, name));
		}

		public override IEnumerator<DesignItemProperty> GetEnumerator()
		{
			yield break;
		}
	}
}
