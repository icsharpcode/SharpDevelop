// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{

	// This class is used only within ParamPanel, it should be an 
	// inner class, but I wanted to move it to a separate file

	internal class ParamControlInfo : IHasControl
	{
		protected const int         STRUCT_LABEL_OFFSET = 20;

		protected const String      STRING_ARRAY = "String[]";


		// Name of the parameter
		internal String             _name;

		// Used for _valueType below
		protected const int         VALUE_STRUCT = 1;
		protected const int         VALUE_SCALAR_INTPTR = 2;
		protected const int         VALUE_SCALAR = 3;
		protected const int         VALUE_SCALAR_STRING_ARRAY = 4;
		protected const int         VALUE_COLOR = 5;
		protected const int         VALUE_OBJECT = 6;
		protected const int         VALUE_ENUM = 7;
		protected const int         VALUE_ENUM_MULTI = 8;
		protected const int         VALUE_OBJECT_SCALAR = 9;

		internal int                _valueType;

		internal Type               _type;

		// This is a property indexer paremeter, its used only
		// on both a get and set
		internal bool               _propertyIndexer;

		// An optional parameter
		internal bool               _optional;


		// This is the value for the field or property, its
		// not really a parameter and it gets mapped to the 
		// fieldPropValue argument in ObjectInfo.Invoke()
		internal bool               _fieldPropValue;

		// This is the panel that gets added to the main
		// panel and contains all of the controls related to
		// the parameter
		internal Control            _controlPanel;

		// The control that is put on the panel for this
		// parameter.  This is typically the same as the
		// value control, but might be a container of value
		// controls in some cases.
		internal Control            _renderControl;


		// Optional additional information
		internal Control            _additionalInfoControl;

		// This is the control that contains the value of the
		// parameter as entered by the user
		internal Control            _valueControl;
		internal Control            _valueControl2;

		// Used for enums, this is the list of values corresponding
		// to the enum types
		// Also used for objects the values of each of the objects
		// presented of the specified type
		internal ArrayList          _values;


		// The initial value of this parameter
		internal Object             _value;

		// If this type of a struct (IsValueType && !IsPrimitive)
		// this is a list of the controls associated each each member
		// of the struct
		internal ArrayList          _structValueControls;

		// This may not be specified
		internal ParameterInfo      _paramInfo;

		internal ParamControlInfo(Type paramType, 
								  ParameterInfo paramInfo)
		{
			_type = paramType;
			_paramInfo = paramInfo;
			if (_paramInfo != null)
				_optional = _paramInfo.IsOptional;
			_values = new ArrayList();
		}

		public Control GetControl()
		{
			return _controlPanel;
		}

		public Control GetRenderControl()
		{
			if (_renderControl == null)
				return _valueControl;
			return _renderControl;
		}

		public Control GetAdditionalInfoControl()
		{
			return _additionalInfoControl;
		}


		// Returns the height of the legend, if any
		internal int InsertAdditionalLegend
			(Control.ControlCollection controls)
		{
			if (_valueType == VALUE_SCALAR_STRING_ARRAY)
			{
				Label l = new Label();
				l.Dock = DockStyle.Top;
				l.Text = StringParser.Parse("${res:ComponentInspector.ParamControlInfo.SeparateStringsWithSpaces}");
				l.AutoSize = true;
				controls.Add(l);
				return l.Height;
			}
			return 0;
		}


		protected void GetParamValueControlEnum()
		{
			_values.AddRange(Enum.GetValues(_type));

			FlagsAttribute flags = (FlagsAttribute)
				Attribute.GetCustomAttribute(_type,
											 typeof(FlagsAttribute));

			if (flags != null)
			{
				// A bit field
				CheckedListBox cb = new CheckedListBox();
				cb.CheckOnClick = true;
				cb.Items.AddRange(Enum.GetNames(_type));
				_valueControl = cb;
				_valueType = VALUE_ENUM_MULTI;
			}
			else
			{
				// Not a bit field, use a normal combo box
				ComboBox cb = new ComboBox();
				cb.DropDownStyle = ComboBoxStyle.DropDownList;
				cb.Items.AddRange(Enum.GetNames(_type));
				cb.SelectedIndex = 0;
				_valueControl = cb;
				_valueType = VALUE_ENUM;
			}

			SetEnumValue();
		}


		// Sets the enum control to the current _value
		protected void SetEnumValue()
		{
			if (_valueType == VALUE_ENUM_MULTI)
			{
				CheckedListBox cb = (CheckedListBox)_valueControl;
				Array enumVals = Enum.GetValues(_type);

				// Clear everything out
				for (int i = 0; i < enumVals.Length; i++)
					cb.SetItemChecked(i, false);

				// Set the checked values
				if (_value != null)
				{
					UInt64 bitValues = Convert.ToUInt64(_value);
					UInt64 bitValue;
					int i = 0;
					foreach (Object enumVal in enumVals)
					{
						bitValue = Convert.ToUInt64(enumVal);
						if ((bitValues & bitValue) != 0)
							cb.SetItemChecked(i, true);
						i++;
					}
				}
			}
			else
			{
				ComboBox cb = (ComboBox)_valueControl;
				if (_value != null)
					cb.SelectedItem = Enum.GetName(_type, _value);
				else
					cb.SelectedItem = -1;
			}
		}


		protected void SetTextBoxControl(TextBox tb,
										 Object value,
										 Type type)
		{
			if (type.Name.Equals(STRING_ARRAY))
			{
				if (value != null)
				{
					StringBuilder bld = new StringBuilder();
					String[] sa = (String[])value;
					for (int i = 0; i < sa.Length; i++)
					{
						if (i > 0)
							bld.Append(" ");
						bld.Append(sa[i]);
					}
					tb.Text = bld.ToString();
				}
			}
			else if (value is String)
			{
				tb.Text = (String)value;
			}
			else if (value is IFormattable)
			{
				tb.Text = ((IFormattable)value).
					ToString(null, null);
			}
			else if (value is IntPtr)
			{
				// Get the number for it
				tb.Text = ((IntPtr)value).ToInt64().ToString();
			}
			else if (value != null)
			{
				tb.Text = value.ToString();
			}

			if (value == null)
				tb.Text = null;

			// Make sure we get right alignment is its a number
			if (value is IFormattable ||
				value is IntPtr ||
				typeof(IFormattable).IsAssignableFrom(type) ||
				typeof(IntPtr).IsAssignableFrom(type))
			{
				tb.TextAlign = HorizontalAlignment.Right;
			}
		}


		protected int GetStructControls(String prefix,
										Object value,
										FieldInfo[] fields,
										ArrayList controls,
										int maxWidth)
		{
			foreach (FieldInfo field in fields)
			{
				int width = 0;
				Object fieldValue = null;

				if (value != null)
					fieldValue = field.GetValue(value);
				Type fieldType = field.FieldType;
				if (fieldType.IsPrimitive ||
					fieldType.Equals(typeof(String)))
				{
					Panel panel = new Panel();
					panel.Dock = DockStyle.Top;

					// The name of the control
					Label l = new Label();
					l.Dock = DockStyle.Right;
					l.Text = prefix + field.Name;
					l.AutoSize = true;
					panel.Controls.Add(l);

					// The value for the control
					TextBox tb = new TextBox();
					SetTextBoxControl(tb, fieldValue, fieldType);
					tb.Dock = DockStyle.Right;
					_structValueControls.Add(tb);
					panel.Controls.Add(tb);

					width = l.Width;

					panel.Height = tb.Height;

					Panel parentPanel = (Panel)_valueControl;
					controls.Add(panel);
					parentPanel.Height += panel.Height;
				}
				else 
				{
					// Don't infinately recurse
					if (!fieldType.Equals(_type))
					{
						width = GetStructControls(field.Name + ".",
												  fieldValue,
												  fieldType.GetFields
												  (ReflectionHelper.ALL_BINDINGS),
												  controls,
												  maxWidth);
					}

				}


				if (width > maxWidth)
					maxWidth = width;
			}   
			return maxWidth;
		}
										
		protected const int COLOR_IMAGE_SIZE = 12;


		protected void SetColorImage(Color c, Button b)
		{
			Image colorImage = new Bitmap(COLOR_IMAGE_SIZE, COLOR_IMAGE_SIZE);
			Graphics g = Graphics.FromImage(colorImage);
			g.Clear(c);
			g.Dispose();
			b.Image = colorImage;
			b.Text = c.ToString();
			b.ImageAlign = ContentAlignment.MiddleLeft;
			b.TextAlign = ContentAlignment.MiddleRight;
		}


		protected void ColorButtonClicked(Object sender, EventArgs e)
		{
			ParamControlInfo pi = (ParamControlInfo)((Button)sender).Tag;
			ColorDialog d = new ColorDialog();
			if (pi._value == null)
				d.Color = new Color();
			else
				d.Color = (Color)pi._value;
			d.ShowDialog();
			pi._value = d.Color;
			SetColorImage(d.Color, (Button)pi._valueControl);
		}



		// Get the parameter control for an object
		protected void GetParamValueControlObject()
		{
			ComboBox cb = new ComboBox();
			cb.DropDownStyle = ComboBoxStyle.DropDownList;

			// For object, add a text field so that a scalar value
			// may be entered as well
			if (ReflectionHelper.TypeEqualsObject(_type))
			{
				Panel p = new Panel();

				cb.Dock = DockStyle.Fill;
				p.Controls.Add(cb);

				Splitter splitter = new Splitter();
				splitter.Dock = DockStyle.Left;
				splitter.Width = Utils.SPLITTER_SIZE;
				p.Controls.Add(splitter);

				TextBox tb = new TextBox();
				tb.Dock = DockStyle.Left;
				tb.Width = 60;
				p.Controls.Add(tb);

				_renderControl = p;
				_valueControl2 = tb;
				_valueType = VALUE_OBJECT_SCALAR;

				p.Height = tb.Height;

				// Initially only enable the text box
				p.MouseDown += new MouseEventHandler(ObjectValueEntered);
				cb.Tag = tb;
				tb.Tag = cb;

				// Panel for the type label
				p = new Panel();
				Label l = new Label();
				l.TextAlign = ContentAlignment.TopRight;
				l.Dock = DockStyle.Right;
				l.Text = StringParser.Parse("${res:ComponentInspector.ParamControlInfo.SelectObjectLabel}");
				l.AutoSize = true;
				p.Controls.Add(l);

				l = new Label();
				l.Dock = DockStyle.Left;
				l.Text = StringParser.Parse("${res:ComponentInspector.ParamControlInfo.ValueLabel}");
				l.AutoSize = true;
				p.Controls.Add(l);

				p.Height = l.Height;
				p.Anchor = AnchorStyles.Right | AnchorStyles.Left;
				p.Width = ObjectBrowser.ParamPanel.ClientSize.Width;
				p.Dock = DockStyle.Top;
				_additionalInfoControl = p;
			}
			else
			{
				_valueType = VALUE_OBJECT;
			}

			_valueControl = cb;

			SetObjectValue();
		}



		// Used to help with the duplicate node elimination
		protected class ObjNode
		{
			internal Object         _obj;
			internal TreeNode       _node;
			internal int            _pathCount;

			internal ObjNode(Object obj, TreeNode node)
			{
				_obj = obj;
				_node = node;
				TreeNode parent = _node.Parent;
				while (parent != null)
				{
					_pathCount++;
					parent = parent.Parent;
				}
			}

			public override bool Equals(Object other)
			{
				ObjNode on = (ObjNode)other;
				// due to bug in cursor throwing
				try
				{
					if (on._obj.Equals(_obj))
						return true;
				}
				catch
				{
					return false;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return _obj.GetHashCode();
			}

		}

		// Wired values in the Object ComboBox
		protected const int OBJECT_NULL = 0;
		protected const int OBJECT_MISSING = 1;


		// Sets the value of the current object control
		protected void SetObjectValue()
		{
			ArrayList nodes = 
				ObjectTreeNode.GetNodesOfType(_type);
			ComboBox cb = (ComboBox)_valueControl;

			// Get rid of the nodes that refer to duplicate or null
			// objects; for duplicates, choose the node with the
			// shortest path
			ArrayList objects = new ArrayList();
			for (int i = 0; i < nodes.Count; )
			{
				ObjectTreeNode node = (ObjectTreeNode)nodes[i];
				Object obj = node.ObjectInfo.Obj;
				if (obj == null)
				{
					// The remaining nodes move up, so we 
					// don't increment the index
					nodes.Remove(node);
					continue;
				}

				ObjNode objNode = new ObjNode(obj, node);
				ObjNode objNodeFound = 
					(ObjNode)Utils.FindHack(objects, objNode);
				if (objNodeFound == null)
				{
					objects.Add(objNode);
					i++;
				}
				else
				{
					// See if this node is better than the one
					// previously found, nodes move up in both
					// cases so don't increment index
					if (objNode._pathCount < objNodeFound._pathCount)
					{
						nodes.Remove(objNodeFound._node);
						objects.Remove(objNodeFound);
						objects.Add(objNode);
					}
					else
					{
						nodes.Remove(node);
					}
				}
			}

			// Now measure all of the remaining nodes to figure
			// out how big to make the combo box
			Graphics g = cb.CreateGraphics();
			int maxWidth = 0;
			foreach (TreeNode node in nodes)
			{
				TraceUtil.WriteLineVerbose(null,
										   "SetObjectValue looking node: " 
										   + ((BrowserTreeNode)node).GetName());
				int width = (int)g.MeasureString(node.ToString(),
												 cb.Font).Width;
				if (width > maxWidth)   
					maxWidth = width;   
			}
			g.Dispose();


			// The first value is "null" to allow the object to 
			// be set to null
			// Second is Missing.Value to allow it to be optional
			_values.Clear();
			_values.Add(null);
			_values.Add(Missing.Value);
			_values.AddRange(nodes);

			if (maxWidth > cb.Width)
				cb.DropDownWidth = maxWidth;
			cb.Items.Clear();
			cb.Items.Add("null");
			cb.Items.Add("Missing.Value");
			cb.Items.AddRange(nodes.ToArray());

			if (_valueControl2 != null)
				((TextBox)_valueControl2).Text = "";

			// Select the object if it exists
			if (_value != null)
			{
				bool found = false;
				foreach (ObjectTreeNode node in nodes)
				{
					if (_value == node.ObjectInfo.Obj)
					{
						cb.SelectedItem = node;
						found = true;
						// Enable the combo box
						cb.Enabled = true;
						if (_valueControl2 != null)
							_valueControl2.Enabled = false;
						break;
					}
				}

				// Just set the text box to be the string value of whatever
				// this is
				if (!found)
				{
					if (_valueControl2 != null)
					{
						cb.Enabled = false;
						_valueControl2.Enabled = true;
						((TextBox)_valueControl2).Text = _value.ToString();
					}
				}
			}
			else
			{
				// Make the default for optional parameters missing
				// so nothing has to be done to specify them
				if (_optional)
				{
					cb.Enabled = true;
					if (_valueControl2 != null)
						_valueControl2.Enabled = false;
					cb.SelectedIndex = OBJECT_MISSING;
				}
				else
				{
					if (_valueControl2 != null)
					{
						// Enable text
						cb.Enabled = false;
						_valueControl2.Enabled = true;
					}
					cb.SelectedIndex = OBJECT_NULL;
				}
			}
		}

		// This handles mouse events for the panel that contains
		// a text box for the object value and a combo box to allow
		// an object to be selected.  This switches the enabling between
		// the two.
		protected void ObjectValueEntered(Object sender, MouseEventArgs e)
		{
			Control control = (Control)sender;
			Control clicked = control.GetChildAtPoint(new Point(e.X, e.Y));

			if (clicked != null && clicked.Tag != null)
			{
				clicked.Enabled = true;
				clicked.Focus();
				((Control)clicked.Tag).Enabled = false;
			}
		}

		// Returns true if this type is a struct that is not specially
		// handled, and therefore we don't allow its values to be updated
		// on the member that points to the struct
		internal static bool IsUnhandledStruct(Type type)
		{
			//  NOTE - keep in sync with the checks below

			if (ReflectionHelper.IsStruct(type))
			{
				if (typeof(Color).IsAssignableFrom(type))
					return false;
				return true;
			}
			return false;
		}


		internal void GetParamValueControl()
		{
			Type type = _type;


			// NOTE - keep this in sync with the above method

			if (typeof(ValueType).IsAssignableFrom(type) ||
				type.Equals(typeof(String)) || 
				type.Name.Equals(STRING_ARRAY))
			{
				// The order of these statements is important, for example
				// a boolean is a primitive, but it want's special representation

				if (typeof(Enum).IsAssignableFrom(type))
				{
					GetParamValueControlEnum();
				}
				else if (typeof(Boolean).IsAssignableFrom(type))
				{
					ComboBox cb = new ComboBox();
					cb.DropDownStyle = ComboBoxStyle.DropDownList;
					_valueType = VALUE_OBJECT;
					_valueControl = cb;
					_values.AddRange(new Boolean[] {false, true});
					cb.Items.AddRange(new Object[] {"false", "true"});
					if (_value != null)
					{
						if ((Boolean)_value)
							cb.SelectedIndex = 1;
						else
							cb.SelectedIndex = 0;
					}
					else
					{
						// Make the default false
						cb.SelectedIndex = 0;
					}
				}
				else if (typeof(Color).IsAssignableFrom(type))
				{
					// A button with the color that selects a color dialog
					Button b = new Button();
					_valueControl = b;
					_valueType = VALUE_COLOR;
					b.Width = 150;
					b.Height = 32;
					b.Tag = this;
					b.Click += new EventHandler(ColorButtonClicked);

					Color color;
					if (_value == null)
						color = new Color();
					else
						color = (Color)_value;
					SetColorImage(color, b);
				}
				else if (type.IsPrimitive ||
						 type.Equals(typeof(String)) ||
						 type.Name.Equals(STRING_ARRAY))
				{
					TextBox tb = new TextBox();
					SetTextBoxControl(tb, _value, type);
					_valueControl = tb;

					// IntPtr requires special conversion, see below
					if (type.Equals(typeof(IntPtr)))
						_valueType = VALUE_SCALAR_INTPTR;
					else if (type.Name.Equals(STRING_ARRAY))
						_valueType = VALUE_SCALAR_STRING_ARRAY;
					else
						_valueType = VALUE_SCALAR;
				}
				else 
				{
					// This is a value type that's not a primitive, 
					// make a field for each of the members, basically
					// its a struct
					FieldInfo[] fields = 
						type.GetFields(ReflectionHelper.ALL_BINDINGS);
					Panel panel = new Panel();
					panel.Dock = DockStyle.Top;
					_valueControl = panel;
					_valueType = VALUE_STRUCT;
					_structValueControls = new ArrayList();

					// These are the actual controls that get added to the
					// overall panel (created above).  These are different
					// than the structvaluecontrols which are the actual
					// containers of the field values.
					ArrayList controls = new ArrayList();

					// The max width of a label, use it to calculate
					// the location of the text boxes
					int maxWidth = 
						GetStructControls("", 
										  _value,
										  fields, controls, 0);

					foreach (Control c in _structValueControls)
						c.Left = maxWidth + STRUCT_LABEL_OFFSET;

					// Add the struct items
					Utils.AddControls(panel, controls);
				}
			}
			else
			{
				GetParamValueControlObject();
			}

			if (_valueControl == null)
			{
				_valueType = VALUE_SCALAR;
				_valueControl = new TextBox();
			}
		}


		// Gets the value of fields in a nested struct based
		// on the values of the associated controls
		protected Object GetStructValues(Type type,
										 FieldInfo[] fields,
										 ref int controlIndex)
		{
			Object obj = AppDomain.CurrentDomain.
				CreateInstanceFrom(type.Assembly.CodeBase,
								   type.FullName);
			obj = ((ObjectHandle)obj).Unwrap();

			foreach (FieldInfo field in fields)
			{
				Object fieldValue;
				Type fieldType = field.FieldType;
				if (fieldType.IsPrimitive ||
					fieldType.Equals(typeof(String)))
				{
					TextBox tb = (TextBox)_structValueControls[controlIndex++];

					try
					{
						fieldValue = Convert.ChangeType(tb.Text, fieldType);
					}
					catch (Exception ex)
					{
						throw new Exception("Error converting value "
											+ "for struct member " 
											+ field.Name, ex);
					}
				}
				else 
				{
					// Don't infinately recurse
					if (!fieldType.Equals(_type))
					{
						fieldValue = GetStructValues
							(fieldType,
							 fieldType.GetFields
							 (ReflectionHelper.ALL_BINDINGS),
							 ref controlIndex);
					}
					else
						continue;

				}

				if (TraceUtil.If(this, TraceLevel.Verbose))
				{
					Trace.WriteLine("struct field: " + field 
									+ " val: " + fieldValue);
				}

				field.SetValue(obj, fieldValue);
			}   
			return obj;
		}
										

		protected Object GetObjControlValue(Control control)
		{
			int index = ((ComboBox)control).SelectedIndex;

			// No value was selected
			if (index == -1)
				return null;

			Object value = _values[index];
			// Unwrap
			if (value is ObjectTreeNode)
				value = ((ObjectTreeNode)value).ObjectInfo.Obj;
			return value;
		}

		internal Object GetValue()
		{
			Object value = null;
			Control control = _valueControl;

			switch (_valueType)
			{
			case VALUE_STRUCT:
				FieldInfo[] fields = _type.GetFields
					(ReflectionHelper.ALL_BINDINGS);
				int controlIndex = 0;
				value = GetStructValues(_type, fields, 
										ref controlIndex);
				break;

			case VALUE_SCALAR_INTPTR:
				value = Convert.ChangeType(((TextBox)control).Text, 
										   typeof(Int64));
				value = new IntPtr((Int64)value); 
				break;

			case VALUE_SCALAR:
				value = Convert.ChangeType(((TextBox)control).Text, 
										   _type);
				break;

			case VALUE_SCALAR_STRING_ARRAY:
				{
					// Use space to separate
					value = ((TextBox)control).Text.
						Split(new char[] {' '});
				}
				break;

			case VALUE_COLOR:
				// This is for color
				value = _value;
				break;

			case VALUE_OBJECT:
			case VALUE_ENUM:
				value = GetObjControlValue(control);
				break;


			case VALUE_OBJECT_SCALAR:
				// Either the control to select the object is 
				// enabled, or the text box (_valueControl2) is 
				// enabled that contains a scalar value.
				if (control.Enabled)
				{
					value = GetObjControlValue(control);
				}
				else
				{
					value = _valueControl2.Text;
					// See if this can be a number
					// FIXME - we should support more than just
					// integers and strings, but later
					try
					{
						Object numVal = Convert.ToInt32(value);
						value = numVal;
					}
					catch 
					{
								// Ignore the error and just go with the
								// string value;
					}
				}
				break;


			case VALUE_ENUM_MULTI:
				CheckedListBox cb = (CheckedListBox)control;
				UInt64 bitValues = 0;
				int i = 0;
				foreach (Object enumVal in Enum.GetValues(_type))
					{
						if (cb.GetItemChecked(i))
							bitValues |= Convert.ToUInt64(enumVal);
						i++;
					}
				value = Enum.ToObject(_type, bitValues);
				break;

			default:
				throw new Exception("Unexpected value type: " 
									+ _valueType);
			}
			return value;

		}


		// Updates the control with the specified value
		internal void SetValue(Object value)
		{
			Control control = _valueControl;
			_value = value;

			switch (_valueType)
			{
			case VALUE_STRUCT:
				throw new Exception
					("(bug)Setting struct values not supported");

			case VALUE_SCALAR:
			case VALUE_SCALAR_INTPTR:
			case VALUE_SCALAR_STRING_ARRAY:
				SetTextBoxControl((TextBox)control, value, _type);
				break;

			case VALUE_COLOR:
				SetColorImage((Color)value, (Button)_valueControl);
				break;

			case VALUE_OBJECT:
			case VALUE_OBJECT_SCALAR:
				SetObjectValue();
				break;

			case VALUE_ENUM:
			case VALUE_ENUM_MULTI:
				SetEnumValue();
				break;

			default:
				throw new Exception("Unexpected value type: " 
									+ _valueType);
			}


		}
	}
}
