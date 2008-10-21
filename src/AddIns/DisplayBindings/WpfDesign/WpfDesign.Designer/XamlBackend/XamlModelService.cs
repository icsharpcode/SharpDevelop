using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Xaml;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	public class XamlModelService : IModelService
	{
		public XamlModelService(XamlDesignContext context)
		{
			this.context = context;
		}

		public void Initialize()
		{
			context.Document.RootChanged += new EventHandler(doc_RootChanged);
			UpdateRoot();
		}

		XamlDesignContext context;
		DesignItem root;

		public event EventHandler RootChanged;
		public event ModelChangedEventHandler ModelChanged;
		public event EventHandler<DesignItemEventArgs> ItemCreated;

		public DesignItem Root
		{
			get { return root; }
			set { Document.Root = (value as XamlDesignItem).ObjectNode; }
		}

		XamlDocument Document
		{
			get { return context.Document; }
		}

		DesignPanel DesignPanel
		{
			get { return context.DesignPanel as DesignPanel; }
		}

		public string ClassName
		{
			get
			{
				if (Document.Root != null) {
					return Document.Root.Property(Directive.Class).ValueText;
				}
				return null;
			}
		}

		public DesignItem CreateItem(object component)
		{
			return CreateItem(Document.CreateObject(component));
		}

		DesignItem CreateItemRecursive(ObjectNode objectNode)
		{
			foreach (var node in objectNode.Descendants().OfType<ObjectNode>()) {
				CreateItem(node);
			}
			return CreateItem(objectNode);
		}

		DesignItem CreateItem(ObjectNode objectNode)
		{
			var result = objectNode.GetAnnotation<DesignItem>();
			if (result == null) {
				result = new XamlDesignItem(objectNode, context);
				objectNode.AnnotateWith<DesignItem>(result);
				if (ItemCreated != null) {
					ItemCreated(this, new DesignItemEventArgs(result));
				}
			}
			return result;
		}

		internal void RaiseModelChanged(ModelChangedEventArgs e)
		{
			if (ModelChanged != null) {
				ModelChanged(this, e);
			}
		}

		void doc_RootChanged(object sender, EventArgs e)
		{
			UpdateRoot();
		}

		void UpdateRoot()
		{
			root = null;
			DesignPanel.Child = null;

			if (Document.Root != null) {
				root = CreateItemRecursive(Document.Root);
				DesignPanel.Child = root.View;
			}

			context.SelectionService.Select(null);

			if (RootChanged != null) {
				RootChanged(this, EventArgs.Empty);
			}
		}
	}
}
