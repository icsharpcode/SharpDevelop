// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.PropertyGrid;
using System.Reflection;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors
{
	[TypeEditor(typeof(byte))]
	[TypeEditor(typeof(sbyte))]
	[TypeEditor(typeof(decimal))]
	[TypeEditor(typeof(double))]
	[TypeEditor(typeof(float))]
	[TypeEditor(typeof(int))]
	[TypeEditor(typeof(uint))]
	[TypeEditor(typeof(long))]
	[TypeEditor(typeof(ulong))]
	[TypeEditor(typeof(short))]
	[TypeEditor(typeof(ushort))]
	public partial class NumberEditor
	{
		static NumberEditor()
		{
			minimums[typeof(byte)] = byte.MinValue;
			minimums[typeof(sbyte)] = sbyte.MinValue;
			minimums[typeof(decimal)] = (double)decimal.MinValue;
			minimums[typeof(double)] = double.MinValue;
			minimums[typeof(float)] = float.MinValue;
			minimums[typeof(int)] = int.MinValue;
			minimums[typeof(uint)] = uint.MinValue;
			minimums[typeof(long)] = long.MinValue;
			minimums[typeof(ulong)] = ulong.MinValue;
			minimums[typeof(short)] = short.MinValue;
			minimums[typeof(ushort)] = ushort.MinValue;

			maximums[typeof(byte)] = byte.MaxValue;
			maximums[typeof(sbyte)] = sbyte.MaxValue;
			maximums[typeof(decimal)] = (double)decimal.MaxValue;
			maximums[typeof(double)] = double.MaxValue;
			maximums[typeof(float)] = float.MaxValue;
			maximums[typeof(int)] = int.MaxValue;
			maximums[typeof(uint)] = uint.MaxValue;
			maximums[typeof(long)] = long.MaxValue;
			maximums[typeof(ulong)] = ulong.MaxValue;
			maximums[typeof(short)] = short.MaxValue;
			maximums[typeof(ushort)] = ushort.MaxValue;
		}

		public NumberEditor()
		{
			InitializeComponent();
			DataContextChanged += new DependencyPropertyChangedEventHandler(NumberEditor_DataContextChanged);
		}

		static Dictionary<Type, double> minimums = new Dictionary<Type, double>();
		static Dictionary<Type, double> maximums = new Dictionary<Type, double>();

		public PropertyNode PropertyNode {
			get { return DataContext as PropertyNode; }
		}

		void NumberEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (PropertyNode == null) return;
			var type = PropertyNode.FirstProperty.ReturnType;

			var range = Metadata.GetValueRange(PropertyNode.FirstProperty);
			if (range == null) {
				range = new NumberRange() { Min = 0, Max = double.MaxValue };
			}

			if (range.Min == double.MinValue) {
				Minimum = minimums[type];
			}
			else {
				Minimum = range.Min;
			}

			if (range.Max == double.MaxValue) {
				Maximum = maximums[type];
			}
			else {
				Maximum = range.Max;
			}

			if (Minimum == 0 && Maximum == 1) {
				DecimalPlaces = 2;
				SmallChange = 0.01;
				LargeChange = 0.1;
			}
			else {
				ClearValue(DecimalPlacesProperty);
				ClearValue(SmallChangeProperty);
				ClearValue(LargeChangeProperty);
			}
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			TextBox textBox=Template.FindName("PART_TextBox",this) as TextBox;
			if(textBox!=null)
				textBox.TextChanged += TextValueChanged;
		}
		
		private void TextValueChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if(PropertyNode==null)
				return;
			if(textBox==null)
				return;
			double val;
			if(double.TryParse(textBox.Text, out val)){
				if(PropertyNode.FirstProperty.TypeConverter.IsValid(textBox.Text)){
			   	   	if(val >= Minimum && val <= Maximum || double.IsNaN(val)){
			   	   		textBox.Foreground=Brushes.Black;
			   	   		textBox.ToolTip=textBox.Text;
			   	   	}else{
			   	   		textBox.Foreground = Brushes.DarkBlue;
			   	   		textBox.ToolTip = "Value should be in between "+Minimum+" and "+Maximum;			   	   		
			   	   	}			   	   	
		   	   }else{
			   	   	textBox.Foreground = Brushes.DarkRed;
			   	   	textBox.ToolTip = "Cannot convert to Type : " + PropertyNode.FirstProperty.ReturnType.Name;
		   	   }
		   	}else{
			   	textBox.Foreground = Brushes.DarkRed;
			   	textBox.ToolTip = string.IsNullOrWhiteSpace(textBox.Text)? null:"Value does not belong to any numeric type";
		   }
			   
		}

		ChangeGroup group;

		protected override void OnDragStarted()
		{
			group = PropertyNode.Context.OpenGroup("drag number", 
				PropertyNode.Properties.Select(p => p.DesignItem).ToArray());
		}

		protected override void OnDragCompleted()
		{
			group.Commit();
		}
	}
}
