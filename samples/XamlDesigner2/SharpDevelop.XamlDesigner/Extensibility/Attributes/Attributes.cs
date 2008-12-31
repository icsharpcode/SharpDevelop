using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Extensibility.Attributes
{
	public class PropertyEditorAttribute : Attribute
	{
		public DataTemplate EditorTemplate;
	}

	public class TypeReplacerAttribute : Attribute
	{
		public Type TypeReplacer;
	}

	public class DefaultSizeAttribute : Attribute
	{
		public Size DefaultSize;
	}

	public class ValueRangeAttribute : Attribute
	{
		public ValueRange ValueRange;
	}

	public class PopularAttribute : Attribute
	{
	}

	public class ContainerTypeAttribute : Attribute
	{
		public Type ContainerType;
	}

	public class ItemInitializerAttribute : Attribute
	{
		public Action<DesignItem> ItemInitializer;
	}

	public class NewItemInitializerAttribute : Attribute
	{
		public Action<DesignItem> NewItemInitializer;
	}

	public class StandardValuesAttribute : Attribute
	{
		public Type Type;
		public StandardValue[] StandardValues;
	}

	public class StandardValue
	{
		public object Instance { get; set; }
		public string Text { get; set; }

		public override string ToString()
		{
			return Text;
		}
	}

	public class ValueRange
	{
		public double Min;
		public double Max;
	}
}
