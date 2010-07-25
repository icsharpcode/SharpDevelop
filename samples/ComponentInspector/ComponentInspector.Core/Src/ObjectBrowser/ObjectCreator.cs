// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.Panels;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	// Creates objects
	public class ObjectCreator
	{
		protected MethodInfo            _entryMethod;
		protected Object                _applThreadContext;
		protected FieldInfo             _applCurrentForm;

		protected IDragDropItem         _sourceNode;
		ObjectTreeNode                  _targetNode;

		protected static ObjectCreator  _objCreator;
		protected static bool           _asyncObjectCreation; 

		protected Form                  _waitingForAppDialog;

		public ObjectCreator()
		{
		}

		ConstructorInfo FindConstructor(ConstructorInfo[] constructors)
		{
			ConstructorInfo constructor = null;

			if (constructors.Length > 1) {
				// Ask them to chose a constructor, if the
				// constructor requires parameters, then select
				// it and get out, otherwise go ahead.
				constructor = ConstructorDialog.
					GetConstructor(constructors);
				if (constructor != null &&
					constructor.GetParameters().Length != 0) {
					AssemblySupport.SelectAssyTab();
					MemberTreeNode.FindMember(constructor).PointToNode();
					return null;
				}
			}
			else
				constructor = constructors[0];

			// This was cancelled
			if (constructor == null)
				return null;

			// Since we need to get parameters, we can't finish
			// this operation now, we refer then to the constructor
			// that they should use and they can drag that 
			// constructor where they want the object to do
			if (constructor.GetParameters().Length != 0)
			{
				BrowserTreeNode selNode = 
					MemberTreeNode.FindMember(constructor);
				ErrorDialog.
					Show("Please select the constructor member " +
						 "of this Type, fill in the parameters, " +
						 "and then drag " +
						 "the constructor to where you " +
						 "want this object " + 
						 "to be created.",
						 "Provide Parameters",
						 MessageBoxIcon.Information);
				AssemblySupport.SelectAssyTab();
				selNode.PointToNode();
				return null;
			}
			return constructor;
		}

		// Thread that runs the entry point for the application
		void EntryRun()
		{
			// Run the application in the specified directory
			string dir = ComponentInspectorProperties.ApplicationWorkingDirectory;
			Directory.CreateDirectory(dir);
			Directory.SetCurrentDirectory(dir);

			// Get the application thread context for this thread;
			// This will create it if its not present
			Type t = typeof(Application).GetNestedType("ThreadContext", ReflectionHelper.ALL_BINDINGS);
			MethodInfo m = t.GetMethod("FromCurrent", ReflectionHelper.ALL_STATIC_BINDINGS);
			_applCurrentForm = t.GetField("currentForm", ReflectionHelper.ALL_STATIC_BINDINGS);
			_applThreadContext = m.Invoke(null, null);

			try {
				Object fieldPropValue;

				// Call the main
				object[] parameters = ObjectBrowser.ParamPanel.GetParameterValues(!Constants.IGNORE_EXCEPTION, ParamPanel.SET_MEMBER, out fieldPropValue);
				_entryMethod.Invoke(null, parameters);
			} catch (Exception ex) {
				lock (_objCreator) {
					CloseWaitingForAppDialog();
				}
				_asyncObjectCreation = false;

				Exception showException = ex;
				// Remove the useless wrapper exception
				if (showException is TargetInvocationException)
					showException = ex.InnerException;

				ErrorDialog.Show(showException,
								 "Error running entry point "
								 + _entryMethod,
								 MessageBoxIcon.Error);
			}
		}

		void CreateFromEntry(Assembly assy)
		{
			Thread t = null;
			lock (this) {
				_entryMethod = assy.EntryPoint;

				ThreadStart start = new ThreadStart(EntryRun);
				t = new Thread(start);
				t.Start();

				_asyncObjectCreation = true;

			   // Now indicate we are wating for the application to start.
				_waitingForAppDialog = new WaitingForAppDialog();
			}

			_waitingForAppDialog.ShowDialog();
			lock (this) {
				// Only kill it if we are waiting for it still, the
				// dialog is closed either by the cancel button, or
				// by being closed when the app is up
				if (_asyncObjectCreation) {
					// Cancel it
					_asyncObjectCreation = false;
					t.Abort();
				}
			}
		}

		const int SLEEP_TIME = 500;

		// This is called by idle to finish the case of running
		// an application in a different thread
		public static void CheckOutstandingCreation()
		{
			if (_objCreator != null && _asyncObjectCreation) {
				// Not ready yet, try again
				if (_objCreator._applThreadContext == null) {
					Thread.Sleep(SLEEP_TIME);
					return;
				}

				Object obj = _objCreator._applCurrentForm.GetValue(_objCreator._applThreadContext);

				// Not ready yet, try again
				if (obj == null) {
					Thread.Sleep(SLEEP_TIME);
					return;
				}

				_objCreator.FinishObjectCreation(obj);
				lock (_objCreator) {
					_asyncObjectCreation = false;
					_objCreator._waitingForAppDialog.Close();
				}
				_objCreator = null;
			}
		}

		internal static ObjectInfo CreateObject(IDragDropItem sourceNode, ObjectTreeNode targetNode)
		{
			ObjectCreator objCreator = new ObjectCreator();
			_objCreator = objCreator;
			return objCreator.CreateObjectInternal(sourceNode, targetNode);
		}

		internal const bool THROW = true;

		internal static bool CheckCreateType(Type t, IDragDropItem sourceNode, bool doThrow)
		{
			if (t == null) {
				String msg = "(This is a bug, please report this) "
					+ "Unable to determine .NET type for " 
					+ sourceNode + new StackTrace(true);
				if (doThrow)
					throw new Exception(msg);
				ErrorDialog.Show(msg,
								"Unable to Determine Type",
								 MessageBoxIcon.Error);
				return false;
			}

			if (!t.IsClass || t.IsAbstract) {
				String msg = "Cannot create an object of type " + t
					+ " because it is not a class, "
					+ "or is an abstract class";
				if (doThrow)
					throw new Exception(msg);
				ErrorDialog.Show(msg,
								"Type is not Class",
								 MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		// Creates the object of the specified class, and wraps a tree node
		// around it
		ObjectInfo CreateObjectInternal(IDragDropItem sourceNode, ObjectTreeNode targetNode)
		{
			_sourceNode = sourceNode;
			_targetNode = targetNode;

			Object obj = null;
			ConstructorInfo constructor = null;
			ConstructorInfo[] constructors = null;

			try {
				if (sourceNode is ITargetType && !((ITargetType)sourceNode).IsMember) {
					ITargetType targetTypeNode = (ITargetType)sourceNode;
					Type t = targetTypeNode.Type;
					if (!CheckCreateType(t, sourceNode, !THROW))
						return null;

					// String is a special case, we just do it with the
					// string parameter value
					if (t.Equals(typeof(String))) {
						ObjectBrowser.ParamPanel.GetParameterValues(!Constants.IGNORE_EXCEPTION, ParamPanel.SET_MEMBER, out obj);
					} else {
						constructors = t.GetConstructors(ReflectionHelper.ALL_BINDINGS);
						if (constructors.Length == 0) {
							obj = Activator.CreateInstance(t);
						} else {
							constructor = FindConstructor(constructors);
							if (constructor == null)
								return null;
						}
					}
				} else if (sourceNode is MemberTreeNode) {
					constructor = (ConstructorInfo)((MemberTreeNode)sourceNode).Member;
				} else if (sourceNode is AssemblyTreeNode) {
					CreateFromEntry(((AssemblyTreeNode)sourceNode).Assembly);
					// This gets finished when the event processing
					// is finished and we go to idle
					return null;
				} else {
					throw new Exception("Bug: invalid node being dragged: " + sourceNode.GetType());
				}

				// Invoke the constructor with the parameters 
				// defined in the param panel
				if (obj == null) {
					Object fieldPropValue;
					obj = constructor.Invoke(ObjectBrowser.ParamPanel.GetParameterValues(!Constants.IGNORE_EXCEPTION,
						ParamPanel.SET_MEMBER, out fieldPropValue));
				}

				return FinishObjectCreation(obj);
			} catch (Exception ex) {
				Exception showException = ex;
				// Remove the useless wrapper exception
				if (showException is TargetInvocationException)
					showException = ex.InnerException;

				ErrorDialog.Show(showException,
								 "Error creating object",
								 MessageBoxIcon.Error);
				return null;
			}
		}

		ObjectInfo FinishObjectCreation(Object obj)
		{
			// Generate a name
			if (obj is Control && !_asyncObjectCreation) {
				Control c = (Control)obj;
				c.Name = CompNumber.GetCompName(obj.GetType());
				// This one can't have the text set to anything but a date
				if (!(c is DateTimePicker))
					c.Text = c.Name;
			}

			ObjectInfo objInfo = _targetNode.AddNewObject(obj, _sourceNode);

			// This is the case when the control is created not by dragging
			// it to the design panel, but dragging it to the object
			// tree somewhere
			if (_sourceNode is IDesignSurfaceNode) {
				IDesignSurfaceNode dNode = (IDesignSurfaceNode)_sourceNode;
				if (obj is Control) {
					Control control = (Control)obj;

					// Some of them don't have them, need to pick something
					// so its visible
					Size defaultSize;
					
					PropertyInfo p = typeof(Control).GetProperty("DefaultSize", ReflectionHelper.ALL_BINDINGS);
					defaultSize = (Size)p.GetValue(control, null);
					if (defaultSize.Equals(Size.Empty)) {
						TraceUtil.WriteLineWarning(this, "No DefaultSize specified for " + control);
						control.Size = new Size(200, 200);
					}

					if (dNode.OnDesignSurface) {
						// Give the user a change with AxWebBrowser
						if (control.GetType().Name.Equals("AxWebBrowser")) {
							ErrorDialog.Show
								("Before calling methods on the WebBrowser "
								 + "object, please turn off Design Mode.  "
								 + "This works around a known problem where "
								 + "the browser control does not work "
								 + "initially correctly in design mode.  "
								 + "The problem only occurs on the "
								 + "first method invocation; you may turn on "
								 + "design mode after the first method call "
								 + "is completely finished.",
								 "Please turn off Design Mode",
								 MessageBoxIcon.Warning);
						}

						try {
							// The control may complain if it does not
							// like where it is being added
							ObjectBrowser.ImagePanel.AddControl(objInfo, control);
						} catch (Exception ex) {
							_targetNode.RemoveObject(objInfo.Obj);
							throw new Exception
								("There was an error adding this control " 
								 + "to the design surface.  You might want "
								 + "using the Action menu (right-click) "
								 + "to not have the control created "
								 + "on the design surface", ex);
						}
					}
					control.Show();
				}
			}
			return objInfo;
		}
		
		void CloseWaitingForAppDialog()
		{
			if (_waitingForAppDialog.InvokeRequired) {
				_waitingForAppDialog.Invoke(new MethodInvoker(CloseWaitingForAppDialog));
			} else {
				Console.WriteLine("CloseWaitingForAppDialog");
				_waitingForAppDialog.Close();
			}
		}
	}
}
