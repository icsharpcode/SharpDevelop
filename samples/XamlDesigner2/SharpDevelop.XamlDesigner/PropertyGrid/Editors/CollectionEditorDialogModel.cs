using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	class CollectionEditorDialogModel : ViewModel
	{
		public CollectionEditorDialogModel(DesignItemCollection collection)
		{
			Collection = collection;
		}

		public DesignItemCollection Collection { get; private set; }

		Type selectedType;

		public Type SelectedType
		{
			get
			{
				return selectedType;
			}
			set
			{
				selectedType = value;
				RaisePropertyChanged("SelectedType");
				RaisePropertyChanged("CanAdd");
			}
		}

		DesignItem currentItem;

		public DesignItem CurrentItem
		{
			get
			{
				return currentItem;
			}
			set
			{
				currentItem = value;
				RaisePropertyChanged("CurrentItem");
				RaisePropertyChanged("CanMoveUp");
				RaisePropertyChanged("CanMoveDown");
				RaisePropertyChanged("CanRemove");
			}
		}

		public bool CanAdd
		{
			get { return SelectedType != null; }
		}

		public bool CanMoveUp
		{
			get { return CurrentItem != null && CurrentItem != Collection[0]; }
		}

		public bool CanMoveDown
		{
			get { return CurrentItem != null && CurrentItem != Collection.Last(); }
		}

		public bool CanRemove
		{
			get { return CurrentItem != null; }
		}

		public void Add()
		{
		}

		public void MoveUp()
		{
		}

		public void MoveDown()
		{
		}

		public void Remove()
		{
		}
	}
}
