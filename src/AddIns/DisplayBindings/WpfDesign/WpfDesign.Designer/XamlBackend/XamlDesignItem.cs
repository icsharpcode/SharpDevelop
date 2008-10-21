using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Xaml;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	public class XamlDesignItem : DesignItem, IHasAnnotations
	{
		public XamlDesignItem(ObjectNode objectNode, XamlDesignContext designContext)
		{
			this.objectNode = objectNode;
			this.designContext = designContext;
			this.properties = new XamlDesignItemPropertyCollection(this);
		}

		ObjectNode objectNode;
		XamlDesignContext designContext;
		XamlDesignItemPropertyCollection properties;

		public override event ModelChangedEventHandler ModelChanged;

		public ObjectNode ObjectNode
		{
			get { return objectNode; }
		}

		public override object Component
		{
			get { return objectNode.Instance; }
		}

		public override Type ComponentType
		{
			get { return objectNode.Type.SystemType; }
		}

		public override UIElement View
		{
			get { return Component as UIElement; }
		}

		public override DesignContext Context
		{
			get { return designContext; }
		}

		public override DesignItem Parent
		{
			get
			{
				if (objectNode.ParentMember == null) {
					return null;
				}
				var parentObject = objectNode.ParentMember.ParentObject;
				if (parentObject == null) {
					return null;
				}
				if (parentObject.IsRetrieved) {
					parentObject = parentObject.ParentMember.ParentObject;
				}
				return parentObject.GetAnnotation<DesignItem>();
			}
		}

		public override DesignItemProperty ParentProperty
		{
			get
			{
				if (objectNode.ParentMember == null) {
					return null;
				}
				return XamlDesignItemProperty.Create(this, objectNode.ParentMember.Property);
			}
		}

		public override DesignItemPropertyCollection Properties
		{
			get { return properties; }
		}

		public override string Name
		{
			get
			{
				return objectNode.Name;
			}
			set
			{
				objectNode.Name = value;
				var args = new ModelChangedEventArgs() {
					Property = XamlDesignItemProperty.Create(this, objectNode.Property(Directive.Name))
				};
				RaiseModelChanged(args);
			}
		}

		public override DesignItemProperty Content
		{
			get
			{
				if (objectNode.Content != null) {
					return XamlDesignItemProperty.Create(this, objectNode.Content);
				}
				return null;
			}
		}

		internal void RaiseModelChanged(ModelChangedEventArgs e)
		{
			if (ModelChanged != null) {
				ModelChanged(this, e);
			}
			(Context.ModelService as XamlModelService).RaiseModelChanged(e);
		}

		#region IHasAnnotations Members

		public void AnnotateWith<T>(T annotation) where T : class
		{
			objectNode.AnnotateWith(annotation);
		}

		public T GetAnnotation<T>() where T : class
		{
			return objectNode.GetAnnotation<T>();
		}

		#endregion
	}
}
