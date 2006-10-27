// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{
	// Form to get the values of the parameters
	internal class ParamPanel : Panel
	{
		protected const int PARAM_SPACING = 5;
		protected ArrayList			 _paramInfos;

		// The node for which we are set up
		protected TreeNode			  _node;

		internal ParamPanel()
		{
			_paramInfos = new ArrayList();
			AutoScroll = true;
		}

		// Used to clear when another node is selected where there
		// might be no parameters
		internal void Clear()
		{
			Controls.Clear();
			_paramInfos.Clear();
			_node = null;
		}


		// Adds a message to this panel
		protected void AddMessage(String message)
		{
			Label l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = message;
			l.AutoSize = true;
			l.Font = new Font(l.Font, FontStyle.Bold);
			Controls.Add(l);
		}

		internal void Setup(String invokeActionName, EventHandler invokeDelegate, TreeNode node, ParameterInfo[] parameters)
		{
			Setup(invokeActionName, invokeDelegate, node, null, null, parameters);
		}


		internal void Setup(String invokeActionName, EventHandler invokeDelegate, TreeNode node, Object obj, MemberInfo memberInfo, ParameterInfo[] parameters)
		{
			Setup(invokeActionName, null, invokeDelegate, null, node, obj, memberInfo, parameters);
		}

		internal void Setup(String invokeActionName,
							String invokeActionName2,
							EventHandler invokeDelegate,
							EventHandler invokeDelegate2,
							TreeNode node,
							Object obj,
							MemberInfo memberInfo,
							ParameterInfo[] parameters)
		{
			// We are already all set
			if (_node == node)
				return;

			Clear();
			_node = node;

			// Add the parameters of the method or property indexers
			if (parameters != null) {
				for (int i = 0; i < parameters.Length; i++) {
					ParameterInfo param = parameters[i];
					Object propIndexValue = null;

					// See if the current object value of the node is
					// set to something, and if so, put the index
					// value in the index parameter
					if (memberInfo is PropertyInfo) {
						ObjectTypeTreeNode otNode = (ObjectTypeTreeNode)node;
						if (otNode.CurrentPropIndexValues != null) {
							propIndexValue =  otNode.CurrentPropIndexValues[i];
						}
					}

					ParamControlInfo pci = AddParam(propIndexValue, param.ParameterType, param.Name, param, !FIELD_PROP_VALUE);

					if (memberInfo is PropertyInfo)
						pci._propertyIndexer = true;
				}
			}

			// String class is a special case for a type node; 
			if (node is TypeTreeNode) {
				Type type = ((TypeTreeNode)node).Type;
				if (type.Equals(typeof(String))) {
					AddParam(null, typeof(String), "value", null, FIELD_PROP_VALUE);
				}
			}

			// Handle the field/property value "parameters"
			if (memberInfo != null) {
				if (!SetupFieldPropValues(node, obj, memberInfo, parameters))
					return;
			}

			ArrayList paramList = new ArrayList();
			foreach (ParamControlInfo param in _paramInfos)
				paramList.Add(param);

			// Supress set button on indexed properties, which
			// is always the 2nd button
			if (memberInfo is PropertyInfo) {
				PropertyInfo propInfo = (PropertyInfo)memberInfo;
				if (ParamControlInfo.IsUnhandledStruct(propInfo.PropertyType)) {
					invokeActionName2 = null;
				}
			}

			// Calc tab offset to make up for the buttons below
			int tabOffset = 0;
			if (invokeActionName != null)
				tabOffset++;
			if (invokeActionName2 != null)
				tabOffset++;

			Utils.AddHasControls(this, paramList, tabOffset);

			if (invokeActionName != null || invokeActionName2 != null) {
				Panel bp = new Panel();
				bp.Dock = DockStyle.Top;
				bp.Height = Utils.BUTTON_HEIGHT;
				bp.Width = Width;

				Button b;
				if (invokeActionName != null) {
					b = Utils.MakeButton(invokeActionName);
					b.Dock = DockStyle.Right;
					b.Click += invokeDelegate;
					bp.Controls.Add(b);
					bp.TabIndex = --tabOffset;
				}

				if (invokeActionName2 != null) {
					b = Utils.MakeButton(invokeActionName2);
					b.Dock = DockStyle.Right;
					b.Click += invokeDelegate2;
					bp.Controls.Add(b);
					bp.TabIndex = --tabOffset;
				}

				Controls.Add(bp);
			}
		}

		// Returns true if processing should continue, false otherwise
		internal bool SetupFieldPropValues(TreeNode node,
										   Object obj,
										   MemberInfo memberInfo,
										   ParameterInfo[] parameters)
		{

			if (memberInfo is PropertyInfo) {
				if (((PropertyInfo)memberInfo).CanWrite) {
					PropertyInfo pi = (PropertyInfo)memberInfo;

					// If not indexed, a struct property is treated like
					// a struct field; nothing is done in the param panel.
					// If indexed, we need to show the index
					if (ParamControlInfo.IsUnhandledStruct(pi.PropertyType)) {
						if (parameters.Length > 0)
							return true;
						return false;
					}

					if (obj == null) {
						AddParam(null, pi.PropertyType, "value", null, FIELD_PROP_VALUE);
					} else {
						ObjectTypeTreeNode otNode = (ObjectTypeTreeNode)node;
						Object value = null;
						try {
							value = pi.GetValue(obj, otNode.CurrentPropIndexValues);
						} catch (Exception ex) {
							TraceUtil.WriteLineInfo
								(this, 
								 "Exception in getting property "
								 + "value for " + pi + ": " + ex);

							// Use the value from ObjectInfo, this is
							// necessary for the property that can only
							// be set, we want to show the last value we
							// set it to, if possible
							value = otNode.ObjectInfo.Obj;
						}
						AddParam(value, pi.PropertyType, "value", null, FIELD_PROP_VALUE);
					}
				} else {
					// Not indexed, nothing to invoke, if index, 
					if (parameters == null || parameters.Length == 0) {
						AddMessage("Property is read only");
						return false;
					}
				}
			} else if (memberInfo is FieldInfo) {
				if (!((FieldInfo)memberInfo).IsLiteral) {
					FieldInfo fi = (FieldInfo)memberInfo;

					// Structs are handled by expanding their members
					if (ParamControlInfo.IsUnhandledStruct(fi.FieldType))
						return false;

					if (obj == null) {
						AddParam(null, fi.FieldType, "value", null, FIELD_PROP_VALUE);
					} else {
						Object value = null;
						try {
							value = fi.GetValue(obj);
						} catch (Exception ex) {
							TraceUtil.WriteLineInfo
								(this, 
								 "Exception in getting field "
								 + "value for " + fi + ": " + ex);

								// Just ignore it
						}
						AddParam(value, fi.FieldType, "value", null, FIELD_PROP_VALUE);
					}
				} else {
					AddMessage("Constant; cannot be changed");
					return false;
				}
			} else if (memberInfo is EventInfo) {
				return false;
			}
			return true;
		}

		protected const bool FIELD_PROP_VALUE = true;


		protected ParamControlInfo AddParam(Object value, 
											Type paramType, 
											String name,
											ParameterInfo paramInfo,
											bool fieldPropValue)
		{
			ParamControlInfo pi = new ParamControlInfo(paramType, paramInfo);
			Label l;
			int h = 0;
			Panel panel = new Panel();
			panel.Dock = DockStyle.Top;

			pi._name = name;
			pi._value = value;
			pi._fieldPropValue = fieldPropValue;

			// In reverse order (sigh)...

			// Spacing
			l = new Label();
			l.Dock = DockStyle.Fill;
			l.Height = PARAM_SPACING;
			h += l.Height;
			panel.Controls.Add(l);

			// For the input
			pi.GetParamValueControl();
			Control con = pi.GetRenderControl();
			//con.Dock = DockStyle.Right;
			con.Width = ClientSize.Width;
			con.Anchor = AnchorStyles.Right | AnchorStyles.Left;
			h += con.Height;

			Panel controlPanel = new Panel();
			controlPanel.Dock = DockStyle.Top;
			controlPanel.Width = ClientSize.Width;
			controlPanel.Height = con.Height;
			controlPanel.Controls.Add(con);
			panel.Controls.Add(controlPanel);

			//panel.Controls.Add(con);

			// Additional legend for string
			h += pi.InsertAdditionalLegend(panel.Controls);

			// Optional additional information
			Control additionalInfo = pi.GetAdditionalInfoControl();
			if (additionalInfo != null) {
				h += additionalInfo.Height;
				panel.Controls.Add(additionalInfo);
			}

			// The parameter type
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = pi._type.ToString();
			// FIXME - here is where we put the special
			// handling for nulls too
			l.AutoSize = true;
			h += l.Height;
			panel.Controls.Add(l);

			// The name of the parameter
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = name;
			l.AutoSize = true;
			l.Font = new Font(l.Font, FontStyle.Bold);
			h += l.Height;
			panel.Controls.Add(l);

			panel.Height = h;
			pi._controlPanel = panel;
			_paramInfos.Add(pi);

			TraceUtil.WriteLineVerbose(this, "Param control: " 
									   + name + " " + paramType);
			return pi;
		}

		internal const bool SET_MEMBER = true;
			
		// Gets the values of the parameters for the specified method
		internal Object[] GetParameterValues(bool ignoreException,
											 bool setMember,
											 out Object fieldPropValue)
		{
			ArrayList values = new ArrayList();
			ParamControlInfo currentPi = null;
			fieldPropValue = null;

			try {
				foreach (ParamControlInfo pi in _paramInfos) {
					// So the exception handler can know which value has
					// the problem
					currentPi = pi;
					Object value = pi.GetValue();

					if (pi._fieldPropValue)
						fieldPropValue = value;
					else
						values.Add(value);
					TraceUtil.WriteLineVerbose
						(this,
						 "Value: " + value + " type: " 
						 + ((value != null) 
							? value.GetType().ToString() : ""));
				}


			} catch (Exception ex) {
				if (!ignoreException) {
					ErrorDialog.Show(ex,
									 currentPi._name 
									 + " Parameter - Error converting value", 
									 MessageBoxIcon.Error);
				}
				return null;

			}
			return values.ToArray();
		}


		// Used after a get operation on an indexed property to 
		// make sure the parameter value is current
		internal void UpdatePropertyValue(Object value)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("Param - UpdatePropertyValue: " + value);

			foreach (ParamControlInfo pi in _paramInfos) {
				if (pi._fieldPropValue) {
					pi.SetValue(value);
					break;
				}
			}
		}
	}
}
