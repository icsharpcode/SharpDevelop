using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom.UndoSystem;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignProperty : ViewModel, IHasContext
	{
		internal DesignProperty(DesignItem item, MemberId member)
		{
			ParentItem = item;
			MemberId = member;

			if (Utils.IsCollection(member.ValueType)) {
				value = new DesignItemCollection(item.Context, member.ValueType);
			}
		}

		public MemberId MemberId { get; private set; }
		public DesignItem ParentItem { get; private set; }

		public DesignContext Context
		{
			get { return ParentItem.Context; }
		}

		DesignItem value;

		public DesignItem Value
		{
			get { return value; }
			set { SetValue(value); }
		}

		public object ValueObject
		{
			get 
			{
				if (Value != null) {
					return Value.Instance;
				}
				var propertyId = MemberId as PropertyId;
				if (propertyId != null && ParentItem.InstanceMatchType) {
					return propertyId.Descriptor.GetValue(ParentItem.Instance);
				}
				return null;
			}
		}

		public bool IsSet
		{
			get { return Value != null; }
		}

		public DesignItemCollection Collection
		{
			get { return Value as DesignItemCollection; }
		}

		public void Reset()
		{
			SetValue(DependencyProperty.UnsetValue);
		}

		public void SetValue(object value)
		{
			var valueItem = value as DesignItem;
			
			if (valueItem == null) {
				//var text = value as string;
				//if (text != null &&
				//    !MemberId.ValueType.IsAssignableFrom(typeof(string)) &&
				//    MemberId.ValueSerializer != null &&					
				//    MemberId.ValueSerializer.CanConvertFromString(text, null)) {

				//    value = MemberId.ValueSerializer.ConvertFromString(text, null);
				//}
				if (value != DependencyProperty.UnsetValue) {
					valueItem = Context.CreateItem(value);
				}
			}

			Context.UndoManager.Execute(new PropertyAction(this, valueItem));
		}

		internal void SetValueCore(DesignItem value)
		{
			this.value = value;

			XamlOperations.SetValue(this);

			RaisePropertyChanged("Value");
			RaisePropertyChanged("ValueObject");
			RaisePropertyChanged("Collection");
			RaisePropertyChanged("IsSet");
		}

		internal void ParserSetValue(DesignItem value)
		{
			this.value = value;
		}
	}
}
