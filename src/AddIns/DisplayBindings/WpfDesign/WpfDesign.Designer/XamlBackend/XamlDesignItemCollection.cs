using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Xaml;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	class XamlDesignItemCollection : Collection<DesignItem>
	{
		public XamlDesignItemCollection(XamlDesignItemProperty designProperty)
		{
			this.designProperty = designProperty;

			foreach (var objectNode in designProperty.XamlProperty.Collection.OfType<ObjectNode>())
			{
				this.Items.Add(objectNode.GetAnnotation<DesignItem>());
			}
		}

		XamlDesignItemProperty designProperty;

		protected override void InsertItem(int index, DesignItem item)
		{
			designProperty.DesignItem.Context.UndoService.Execute(new InsertAction(this, index, item));
		}

		protected override void RemoveItem(int index)
		{
			designProperty.DesignItem.Context.UndoService.Execute(new RemoveAction(this, index));
		}

		protected override void SetItem(int index, DesignItem item)
		{
			RemoveItem(index);
			InsertItem(index, item);
		}

		protected override void ClearItems()
		{
			while (Count > 0)
			{
				RemoveItem(Count - 1);
			}
		}

		void InsertCore(int index, DesignItem item)
		{
			this.Items.Insert(index, item);
			designProperty.XamlProperty.Collection.Insert(index, (item as XamlDesignItem).ObjectNode);
			
			var args = new ModelChangedEventArgs() {
				Property = designProperty,
				NewItem = item
			};
			designProperty.XamlDesignItem.RaiseModelChanged(args);
		}

		void RemoveCore(int index, DesignItem item)
		{
			this.Items.RemoveAt(index);
			designProperty.XamlProperty.Collection.RemoveAt(index);
			
			var args = new ModelChangedEventArgs() {
				Property = designProperty,
				OldItem = item
			};
			designProperty.XamlDesignItem.RaiseModelChanged(args);
		}

		class InsertAction : IUndoAction
		{
			public InsertAction(XamlDesignItemCollection collection, int index, DesignItem item)
			{
				this.collection = collection;
				this.index = index;
				this.item = item;
			}

			XamlDesignItemCollection collection;
			int index;
			DesignItem item;

			public IEnumerable<DesignItem> AffectedItems
			{
				get { return new DesignItem[] { item }; }
			}

			public string Title
			{
				get { return "Insert into collection"; }
			}

			public void Do()
			{
				collection.InsertCore(index, item);
			}

			public void Undo()
			{
				collection.RemoveCore(index, item);
			}
		}

		class RemoveAction : IUndoAction
		{
			public RemoveAction(XamlDesignItemCollection collection, int index)
			{
				this.collection = collection;
				this.index = index;
				this.item = collection[index];
			}

			XamlDesignItemCollection collection;
			int index;
			DesignItem item;

			public IEnumerable<DesignItem> AffectedItems
			{
				get { return new DesignItem[] { item }; }
			}

			public string Title
			{
				get { return "Remove from collection"; }
			}

			public void Do()
			{
				collection.RemoveCore(index, item);
			}

			public void Undo()
			{
				collection.InsertCore(index, item);
			}
		}
	}
}
