using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Extensibility;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Windows.Markup;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignItem : DependencyObject, INotifyPropertyChanged, IHasContext
	{
		internal DesignItem(DesignContext context, Type type)
		{
			Construct(context, type, null);
		}

		internal DesignItem(DesignContext context, object instance)
		{
			Construct(context, instance.GetType(), instance);
			InitializeNew();
		}

		public Type Type { get; private set; }
		public object Instance { get; private set; }
		public DesignProperty ParentProperty { get; private set; }
		public DesignItem ParentItem { get; private set; }
		public DesignContext Context { get; private set; }
		public DesignProperty Content { get; private set; }
		//public DesignProperty Name { get; private set; }
		public XElement XmlElement;

		Dictionary<MemberId, DesignProperty> properties = new Dictionary<MemberId, DesignProperty>();

		public FrameworkElement View
		{
			get { return Instance as FrameworkElement; }
		}

		public static readonly DependencyProperty IsSelectedProperty =
			Selector.IsSelectedProperty.AddOwner(typeof(DesignItem));

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		string name;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				RaisePropertyChanged("Name");
				RaisePropertyChanged("DisplayName");
			}
		}

		public string DisplayName
		{
			get
			{
				if (Name != null) {
					return Type.Name + " (" + Name + ")";
				}
				return Type.Name;
			}
		}

		public bool InstanceMatchType
		{
			get
			{
				return Instance == null || Instance.GetType() == Type;
			}
		}

		public static DesignItem GetAttachedItem(DependencyObject obj)
		{
			return (DesignItem)obj.GetValue(AttachedItemProperty);
		}

		public static void SetAttachedItem(DependencyObject obj, DesignItem value)
		{
			obj.SetValue(AttachedItemProperty, value);
		}

		public static readonly DependencyProperty AttachedItemProperty =
			DependencyProperty.RegisterAttached("AttachedItem", typeof(DesignItem), typeof(DesignItem),
			new PropertyMetadata(AttachedItemChanged));

		static void AttachedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var item = e.NewValue as DesignItem;
			item.Instance = d;
			item.Initialize();
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsSelectedProperty) {
				if ((bool)e.NewValue) {
					Context.Selection.AddWhenPropertyChanged(this);
				}
				else {
					Context.Selection.RemoveWhenPropertyChanged(this);
				}
			}
		}

		void Construct(DesignContext context, Type type, object instance)
		{
			Context = context;
			Type = type;
			Instance = instance;

			foreach (ContentPropertyAttribute a in type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)) {
				if (a.Name != null) {
					Content = Property(a.Name);
				}
				break;
			}

			//foreach (RuntimeNamePropertyAttribute a in type.GetCustomAttributes(typeof(RuntimeNamePropertyAttribute), true)) {
			//    if (a.Name != null) {
			//        Name = Property(a.Name);
			//    }
			//    break;
			//}
		}

		public void Initialize()
		{
			foreach (var a in MetadataStore.GetAttributes<ItemInitializerAttribute>(Type)) {
				a.ItemInitializer(this);
			}
		}

		public void InitializeNew()
		{
			Initialize();
			foreach (var a in MetadataStore.GetAttributes<NewItemInitializerAttribute>(Type)) {
				a.NewItemInitializer(this);
			}
			var b = MetadataStore.GetAttributes<DefaultSizeAttribute>(Type).FirstOrDefault();
			if (b != null) {
				View.Width = b.DefaultSize.Width;
				View.Height = b.DefaultSize.Height;
			}
		}

		public DesignProperty Property(string name)
		{
			return Property(Type, name);
		}

		public DesignProperty Property(Type type, string name)
		{
			return Property(MemberId.GetMember(type, name));
		}

		public DesignProperty Property(MemberId member)
		{
			DesignProperty result;
			if (!properties.TryGetValue(member, out result)) {
				result = new DesignProperty(this, member);
				properties[member] = result;
			}
			return result;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
