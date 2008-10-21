using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Xaml;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	public class XamlDesignItemProperty : DesignItemProperty
	{
		public static XamlDesignItemProperty Create(XamlDesignItem designItem, XamlProperty xamlProperty)
		{
			var result = xamlProperty.GetAnnotation<XamlDesignItemProperty>();
			if (result == null) {
				result = new XamlDesignItemProperty(designItem, xamlProperty);
				xamlProperty.AnnotateWith(result);
			}
			return result;
		}

		XamlDesignItemProperty(XamlDesignItem designItem, XamlProperty xamlProperty)
		{
			this.designItem = designItem;
			this.xamlProperty = xamlProperty;
			if (IsCollection) {
				this.collectionElements = new XamlDesignItemCollection(this);
			}
		}

		XamlDesignItem designItem;
		XamlProperty xamlProperty;
		XamlDesignItemCollection collectionElements;
		static IList<DesignItem> emptyCollectionElements = new DesignItem[] { };

		public override event EventHandler IsSetChanged;
		public override event EventHandler ValueChanged;
		public override event EventHandler ValueOnInstanceChanged;

		public XamlDesignItem XamlDesignItem
		{
			get { return designItem; }
		}

		public XamlProperty XamlProperty
		{
			get { return xamlProperty; }
		}

		public XamlMember XamlMember
		{
			get { return xamlProperty.Member; }
		}

		public override string Name
		{
			get { return XamlMember.Name; }
		}

		public override Type ReturnType
		{
			get { return XamlMember.ValueType.SystemType; }
		}

		public override Type DeclaringType
		{
			get { return XamlMember.OwnerType.SystemType; }
		}

		public override string Category
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsCollection
		{
			get { return XamlMember.ValueType.IsCollection; }
		}

		public override bool IsEvent
		{
			get { return XamlMember.IsEvent; }
		}

		public override IList<DesignItem> CollectionElements
		{
			get { return collectionElements ?? emptyCollectionElements; }
		}

		public override DesignItem Value
		{
			get
			{
				if (xamlProperty.IsSet) {
					return xamlProperty.Value.GetAnnotation<DesignItem>();
				}
				return null;
			}
		}

		public override object ValueOnInstance
		{
			get
			{
				return xamlProperty.ValueOnInstance;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsSet
		{
			get { return xamlProperty.IsSet; }
		}

		public override bool IsNameProperty
		{
			get { return XamlProperty.Member.IsNameProperty; }
		}

		public override DesignItem DesignItem
		{
			get { return designItem; }
		}

		public override DependencyProperty DependencyProperty
		{
			get { return null; }
		}

		public override bool IsAdvanced
		{
			get { throw new NotImplementedException(); }
		}

		public override void SetValue(object value)
		{
			var newValue = xamlProperty.PrepareValue(value);
			var action = new PropertyChangeAction(this, newValue, true);
			designItem.Context.UndoService.Execute(action);
		}

		public override void Reset()
		{
			var action = new PropertyChangeAction(this, null, false);
			designItem.Context.UndoService.Execute(action);
			xamlProperty.Reset();
		}

		void SetValueCore(XamlValue value)
		{
			xamlProperty.Set(value);
			var args = new ModelChangedEventArgs() {
				Property = this
			};
			designItem.RaiseModelChanged(args);
		}

		void ResetCore()
		{
			if (xamlProperty.IsSet) {
				xamlProperty.Reset();
				var args = new ModelChangedEventArgs() {
					Property = this,
				};
				designItem.RaiseModelChanged(args);
			}
		}

		class PropertyChangeAction : IUndoAction
		{
			public PropertyChangeAction(XamlDesignItemProperty designProperty, XamlValue newValue, bool newIsSet)
			{
				this.designProperty = designProperty;
				this.newValue = newValue;
				this.newIsSet = newIsSet;

				oldIsSet = designProperty.IsSet;
				oldValue = designProperty.XamlProperty.Value;
			}

			XamlDesignItemProperty designProperty;
			XamlValue oldValue;
			XamlValue newValue;
			bool oldIsSet;
			bool newIsSet;

			public string Title
			{
				get
				{
					if (newIsSet)
						return "Set " + designProperty.Name;
					else
						return "Reset " + designProperty.Name;
				}
			}

			public void Do()
			{
				if (newIsSet)
					designProperty.SetValueCore(newValue);
				else
					designProperty.ResetCore();
			}

			public void Undo()
			{
				if (oldIsSet)
					designProperty.SetValueCore(oldValue);
				else
					designProperty.ResetCore();
			}

			public IEnumerable<DesignItem> AffectedItems
			{
				get
				{
					return new DesignItem[] { designProperty.DesignItem };
				}
			}

			public bool MergeWith(PropertyChangeAction other)
			{
				if (designProperty.XamlMember == other.designProperty.XamlMember) {
					newIsSet = other.newIsSet;
					newValue = other.newValue;
					return true;
				}
				return false;
			}
		}
	}
}
