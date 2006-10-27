// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class EventLogList : ListView
	{
		protected const int             PADDING = 10;
		
		// This is bumped everytime we go into the idle
		// loop.
		protected static int            _incarnationNo = 1;
		protected static int            _prevIncarnationNo;

		// This is monotonicaly increasing as event trace messages
		// are presented.
		protected static int            _incarnationSeqNo;

		// Keeps track of the objects/events being logged
		protected Hashtable             _loggerInstanceHash;

		// Keeps track of objects that have any events being logged
		protected Hashtable             _loggerObjectHash;
		
		protected Hashtable             _loggerTypeHash;

		protected ListView              _eventsBeingLogged;

		protected TabPage               _eventLogTabPage;
		protected TabPage               _eventsBeingLoggedTabPage;

		protected Control               _eblText;

		// Used to make sure the type names are unique
		protected int                   _delegateSequenceNo;


		// Positions of the subitems
		protected const int             EVENT = 0;
		protected const int             PARAMS = 1;
		protected const int             INCARNATION = 2;
		protected const int             TIME = 3;

		internal TabPage EventLogTabPage
		{
			get {
				return _eventLogTabPage;
			}
		}

		internal TabPage EventsBeingLoggedTabPage
		{
			get {
				return _eventsBeingLoggedTabPage;
			}
		}

		internal ListView EventsBeingLogged
		{
			get {
				return _eventsBeingLogged;
			}
		}

		internal EventLogList(Control parent)
		{
			this.Parent = parent;
			
			CommonInit(this);

			Dock = DockStyle.Fill;
			BorderStyle = BorderStyle.None;

			InitEventsBeingLogged();

			_eventLogTabPage = new TabPage();
			_eventLogTabPage.Controls.Add(this);
			_eventLogTabPage.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.EventsTab}");
			_eventLogTabPage.BorderStyle = BorderStyle.None;

			_eventsBeingLoggedTabPage = new TabPage();
			_eventsBeingLoggedTabPage.Controls.Add(_eblText);
			_eventsBeingLoggedTabPage.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.EventsBeingLoggedTab}");
			_eventsBeingLoggedTabPage.BorderStyle = BorderStyle.None;

			_loggerInstanceHash = new Hashtable();
			_loggerObjectHash = new Hashtable();
			_loggerTypeHash = new Hashtable();

			MenuItem mi = new MenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.ClearMenuItem}");
			mi.Click += new EventHandler(DeleteClick);
			ContextMenu.MenuItems.Add(mi);

			mi = new MenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.ClearAllMenuItem}");
			mi.Click += new EventHandler(DeleteAllClick);
			ContextMenu.MenuItems.Add(mi);


			// Keep in sync with the position constants above
			ColumnHeader ch;
			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.EventColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 90;
			Columns.Add(ch);

			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.ParametersColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 215;
			Columns.Add(ch);

			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.IncrementColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 35;
			Columns.Add(ch);

			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.TimeColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 75;
			Columns.Add(ch);
		}

		protected void CommonInit(ListView lv)
		{
			lv.FullRowSelect = true;
			lv.MultiSelect = true;
			lv.HideSelection = false;
			lv.ContextMenu = new ContextMenu();
			lv.View = View.Details;
			lv.BorderStyle = BorderStyle.None;
			lv.Dock = DockStyle.Fill;
		}

		protected void InitEventsBeingLogged()
		{
			_eventsBeingLogged = new ListView();

			String desc = StringParser.Parse("${res:ComponentInspector.EventLogList.NoEventsBeingLoggedMessage}");

			_eblText = Utils.MakeDescText(desc, Parent);
			_eblText.Dock = DockStyle.Fill;
			
			CommonInit(_eventsBeingLogged);
			_eventsBeingLogged.SelectedIndexChanged += new EventHandler(EBLSelectedIndexChanged);

			MenuItem mi;

			mi = new MenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.StopLoggingMenuItem}");
			mi.Click += new EventHandler(EBLDeleteClick);
			_eventsBeingLogged.ContextMenu.MenuItems.Add(mi);

			mi = new MenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.StopAllLoggingMenuItem}");
			mi.Click += new EventHandler(EBLDeleteAllClick);
			_eventsBeingLogged.ContextMenu.MenuItems.Add(mi);

			ColumnHeader ch;
			ch = new ColumnHeader();
			ch.Text = "Object";
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 175;
			_eventsBeingLogged.Columns.Add(ch);

			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.EventColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			ch.Width = 200;
			_eventsBeingLogged.Columns.Add(ch);
		}

		// Called from the thread idle loop to get a new raw
		// incarnation number
		public static void NewIncarnation()
		{
			lock (typeof(EventLogList)) {
				_incarnationNo++;
			}
		}

		// Used when the incarnation number is needed, produces
		// the monotonic number
		protected static int TraceIncarnation()
		{
			lock (typeof(EventLogList)) {
				if (_incarnationNo != _prevIncarnationNo) {
					_prevIncarnationNo = _incarnationNo;
					_incarnationSeqNo++;
				}
				return _incarnationSeqNo;
			}
		}

		protected void DeleteClick(object sender, EventArgs e)
		{
			foreach (ListViewItem li in SelectedItems)
				Items.Remove(li);
			DetailPanel.Clear();
		}


		protected void DeleteAllClick(object sender, EventArgs e)
		{
			Items.Clear();
			DetailPanel.Clear();
		}

		protected void EBLDeleteClickCommon(ICollection items)
		{
			foreach (ListViewItem li in items)
			{
				ToggleLogging((ObjectEvent)li.Tag, null, STOP);
			}
		}

		protected void EBLDeleteClick(object sender, EventArgs e)
		{
			EBLDeleteClickCommon(_eventsBeingLogged.SelectedItems);
		}

		protected void EBLDeleteAllClick(object sender, EventArgs e)
		{
			EBLDeleteClickCommon(_eventsBeingLogged.Items);
		}

		internal void EBLSelectedIndexChanged(Object sender,
										   EventArgs e)
		{
			DetailPanel.Clear();

			if (_eventsBeingLogged.SelectedItems.Count == 0)
				return;
			ListViewItem li = (ListViewItem)_eventsBeingLogged.
				SelectedItems[0];

			DetailPanel.AddLink("Event", 
								!ObjectBrowser.INTERNAL,
								18,
								ObjMemberLinkHelper.OMLHelper,
								li.Tag);
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			GetDetailText();
			base.OnSelectedIndexChanged(e);
		}

		protected void GetDetailText()
		{
			DetailPanel.Clear();

			if (SelectedItems.Count == 0)
				return;
			LoggerListViewItem li = (LoggerListViewItem)SelectedItems[0];
			ObjectEvent oe = (ObjectEvent)li.Tag;

			DetailPanel.AddLink("Event", 
								!ObjectBrowser.INTERNAL,
								18,
								ObjMemberLinkHelper.OMLHelper,
								oe);

			// The event parameters
			// FIXME - add then in reverse order, the detail panel
			// seems to reverse them
			for (int i = li._objects.Length - 1; i >=0; i--)
			{
				ParameterInfo p = oe._eventParams[i];

				Object obj = li._objects[i];
				if (obj != null)
				{
					if (!obj.GetType().IsValueType &&
						!(obj is String))
					{
						DetailPanel.AddLink(p.Name,
											!ObjectBrowser.INTERNAL,
											20,
											ObjLinkHelper.OLHelper,
											li._objects[i]);
					}
					else
					{
						DetailPanel.Add(p.Name,
										!ObjectBrowser.INTERNAL,
										20,
										li._objects[i].ToString());
					}
				}
				else
				{
					DetailPanel.Add(p.Name,
									!ObjectBrowser.INTERNAL,
									20,
									"");
				}
			}

			DetailPanel.Add("Incarnation", 
							!ObjectBrowser.INTERNAL,
							40,
							li.SubItems[INCARNATION].Text);

			DetailPanel.Add("Time", 
							!ObjectBrowser.INTERNAL,
							50,
							li.SubItems[TIME].Text);

			// Fixup the stack trace to display on detail panel
			StringBuilder stackTrace = 
				new StringBuilder(li._stackTrace.ToString());
			// Get rid of first newline
			stackTrace.Remove(0, 2);
			// And last newline
			stackTrace.Remove(stackTrace.Length - 2, 2);
			// And the tab in the front of the lines
			stackTrace.Replace("\tat ", "");

			DetailPanel.Add("Stack Trace", 
							!ObjectBrowser.INTERNAL,
							60,
							stackTrace.ToString());

		}

		// Should be internal, but compiler gives warning
		// FIXME - make an interface and hook this up
		public  void DoTabSelected()
		{
			GetDetailText();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			GetDetailText();
		}

		protected class LoggerListViewItem : ListViewItem
		{
			internal StackTrace             _stackTrace;
			internal Object[]               _objects;

			public LoggerListViewItem(String text) : base(text)
			{
			}
		}


		// Used to keep track of which events/objects are being logged
		internal class ObjectEvent : IObjectMember
		{
			internal Object                 _object;
			internal EventInfo              _eventInfo;
			internal ParameterInfo[]        _eventParams;
			internal Type                   _eventReturnType;

			public Object Obj
			{
				get
					{
						return _object;
					}
			}

			public MemberInfo Member
			{
				get
					{
						return _eventInfo;
					}
			}


			internal ObjectEvent(Object obj, 
								 EventInfo eventInfo)
			{
				_object = obj;
				_eventInfo = eventInfo;
			}

			internal void SetEventParams()
			{
				Type delegateType = _eventInfo.EventHandlerType;

				// The parameters of the invoke method are those that
				// need to be created for the method that is delegated
				MethodInfo invokeMeth = delegateType.GetMethod("Invoke", ReflectionHelper.ALL_BINDINGS);
				_eventParams = invokeMeth.GetParameters();
				_eventReturnType = invokeMeth.ReturnType;
			}

			public override bool Equals(Object other)
			{
				if (other is ObjectEvent)
				{
					ObjectEvent otherOe = (ObjectEvent)other;
					bool eq = false;
					if (_object != null && otherOe._object != null) {
						eq = otherOe._object.Equals(_object);
					}
					if (eq)
					{
						if (_eventInfo != null && 
							otherOe._eventInfo != null)
						{
							eq = ReflectionHelper.
								IsMemberEqual(otherOe._eventInfo,
											  _eventInfo);
						}
						// If they are both null, then this is equal
					}
					return eq;
				}
				return false;
			}

			public override int GetHashCode()
			{
				int hashCode = 0;
				if (_object != null)
					hashCode += _object.GetHashCode();
				if (_eventInfo != null)
					hashCode += _eventInfo.GetHashCode();
				return hashCode;
			}

			public override String ToString()
			{
				return _object.ToString() + " ev: " + _eventInfo.Name;
			}

		}

		internal class ObjectEventCountHolder
		{
			internal int                _count;
		}

		// This class is the base class for the object
		// that actually does the event logging.  A subclass of this, with 
		// a new method called "Logger" is added for each type of event
		// that is logged.
		public class EventLogger
		{
			public delegate void LoggerBaseHandler(Object[] objects);
			
			public EventLogList             _eventLog;
			internal ObjectEvent            _objectEvent;

			// WARNING: Make sure this it the only method in this class, since
			// the method is not accessed by name.  See below, call to 
			// GetMethods.

			// This logs the actual events
			public void LoggerBase(Object[] objects)
			{
				if (_eventLog.InvokeRequired) {
					_eventLog.Invoke(new LoggerBaseHandler(LoggerBase), new object[] {objects});
				} else {
					LoggerListViewItem li = 
						new LoggerListViewItem(_objectEvent._eventInfo.Name);
					// Skip two frames and get the source information if available
					li._stackTrace = new StackTrace(2, true);
					li._objects = objects;
					li.Tag = _objectEvent;
	
					lock (_eventLog)
					{
						if (TraceUtil.If(this, TraceLevel.Info))
							Trace.Write(_objectEvent.ToString());
						_eventLog.Items.Add(li);
						_eventLog.EnsureVisible(_eventLog.Items.Count - 1);
	
						// Get correct incarnation number
						int incNo = TraceIncarnation();
	
						// Build the event parameter string
						StringBuilder dataStr = new StringBuilder();
						for (int i = 0; i < objects.Length; i++)
						{
							if (TraceUtil.If(this, TraceLevel.Info))
								Trace.Write(", " + objects[i]);
							if (i > 0)
								dataStr.Append(", ");
							if (objects[i] != null) {
								// Using object.ToString seems to hang so use the type
								// name instead.
								//dataStr.Append(objects[i].ToString());
								dataStr.Append(objects[i].GetType().FullName);
							}
						}
						TraceUtil.WriteLineInfo(this, "");
	
						li.SubItems.Add(dataStr.ToString());
						li.SubItems.Add(incNo.ToString());
						li.SubItems.Add(DateTime.Now.ToString("HH:mm:ss.ff"));
					}
				}
			}
		}

		protected Type CreateEventHandlerDelegate
			(EventInfo eventInfo,
			 ParameterInfo[] delegateParams,
			 Type returnType)
		{
			// Create a type with the right kind of event 
			// handler method
			TypeBuilder tb = AssemblySupport.ModBuilder.
				DefineType(eventInfo.EventHandlerType.Name
						   + _delegateSequenceNo++,
						   TypeAttributes.Class
						   | TypeAttributes.Public);
			tb.SetParent(typeof(EventLogger));
			tb.DefineDefaultConstructor(MethodAttributes.Public);

			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.Write("Created delegate: " 
							+ eventInfo.EventHandlerType.Name);
			}

			// Paremeters for the event handler method
			Type[] parameterTypes = new Type[delegateParams.Length];
			for (int i = 0; i < delegateParams.Length; i++)
			{
				parameterTypes[i] = delegateParams[i].ParameterType; 
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.Write(", " + delegateParams[i].Name 
								+ " [" + delegateParams[i].ParameterType.Name
								+ "]");
					if (delegateParams[i].IsIn)
						Trace.Write(" (in) ");
					if (delegateParams[i].IsOut)
						Trace.Write(" (out) ");
				}
			}

			TraceUtil.WriteLineInfo(this, "");

			// Get the "LoggerBase" method, which is the only declared
			// method.  We don't use the name to avoid problems with
			// obfuscation
			MethodInfo loggerMeth = typeof(EventLogger).
				GetMethods(BindingFlags.Public
						   | BindingFlags.Instance
						   | BindingFlags.DeclaredOnly)[0];

			// The event handler method, Calls the "LoggerBase"
			// method to do the actual logging
			MethodBuilder mb = tb.DefineMethod("Logger", MethodAttributes.Public, returnType, parameterTypes);
			ILGenerator gen = mb.GetILGenerator();

			gen.DeclareLocal(typeof(Object[]));

			// Create the object array output of the right size
			gen.Emit(OpCodes.Ldc_I4, delegateParams.Length);
			gen.Emit(OpCodes.Newarr, typeof(Object));

			// Save object array
			gen.Emit(OpCodes.Stloc_0);

			// Process each of the parameters
			for (int i = 0; i < delegateParams.Length; i++)
			{
				// Object array
				gen.Emit(OpCodes.Ldloc_0);

						// Index into array
				gen.Emit(OpCodes.Ldc_I4, i);

				// Parameter to store into array
				gen.Emit(OpCodes.Ldarg, i + 1);

				// Ref params have to be derefed
				if (parameterTypes[i].IsByRef)
				{
					gen.Emit(OpCodes.Ldind_Ref);
				}

				// Value types have to be boxed
				if (typeof(ValueType).IsAssignableFrom
					(parameterTypes[i]))
				{
					gen.Emit(OpCodes.Box, parameterTypes[i]);
				}

				// Store into array
				gen.Emit(OpCodes.Stelem_Ref);
			}

			// Load this
			gen.Emit(OpCodes.Ldarg_0);

			// Load object array parameter, call, return
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Call, loggerMeth);

			// Return false if there is a return value associated with the
			// event.
			if (!returnType.Equals(typeof(void)))
				gen.Emit(OpCodes.Ldc_I4, 0);

			gen.Emit(OpCodes.Ret);

			Type handlerType = tb.CreateType();
			return handlerType;
		}

		internal Delegate GetEventLoggerDelegate(ObjectEvent objectEvent)
		{
			EventInfo eventInfo = objectEvent._eventInfo;
			Type handlerType;
			Type delegateType = eventInfo.EventHandlerType;

			objectEvent.SetEventParams();

			lock (this)
			{
				handlerType = (Type)_loggerTypeHash[eventInfo.EventHandlerType];
				if (handlerType == null)
				{
					handlerType = CreateEventHandlerDelegate(eventInfo, objectEvent._eventParams,
						 objectEvent._eventReturnType);
					_loggerTypeHash.Add(eventInfo.EventHandlerType, handlerType);
				}
			}

			// Create the setup the logger object
			EventLogger logger = (EventLogger)Activator.CreateInstance(handlerType);
			AddLoggerInstance(objectEvent, logger);
			logger._eventLog = this;
			logger._objectEvent = objectEvent;

			Delegate d = Delegate.CreateDelegate(delegateType, logger, "Logger");
			TraceUtil.WriteLineInfo(this, "Found delegate: " + delegateType);
			return d;
		}

		internal const bool         START = true;
		internal const bool         STOP = false;

		internal void AddLoggerInstance(ObjectEvent oe, EventLogger logger)								
		{
			lock (this)
			{
				_loggerInstanceHash.Add(oe, logger);
				ObjectEventCountHolder h = (ObjectEventCountHolder)_loggerObjectHash[oe._object];
				if (h == null)
				{
					h = new ObjectEventCountHolder();
					_loggerObjectHash.Add(oe._object, h);
				}
				h._count++;
			}
		}

		internal void RemoveLoggerInstance(ObjectEvent oe)
		{
			lock (this)
			{
				_loggerInstanceHash.Remove(oe);
				ObjectEventCountHolder h = (ObjectEventCountHolder)_loggerObjectHash[oe._object];
				h._count--;
				if (h._count == 0) {
					_loggerObjectHash.Remove(oe._object);
				}
			}
		}


		internal EventLogger GetLoggerInstance(ObjectEvent oe)
		{
			lock (this)
			{
				return (EventLogger)_loggerInstanceHash[oe];
			}
		}


		internal bool IsLogging(Object obj,
								EventInfo eventInfo)
		{
			ObjectEvent oe = new ObjectEvent(obj, eventInfo);
			if (GetLoggerInstance(oe) != null)
			{
				return true;
			}
			return false;
		}


		// Gets the number of logging events for the specified object
		internal int LoggingCount(Object obj)
		{
			lock (this)
			{
				ObjectEventCountHolder h =
					(ObjectEventCountHolder)_loggerObjectHash[obj];
				if (h != null)
				{
					return h._count;
				}
				return 0;
			}
		}


		// Note: We can't store the node anywhere because it is subject
		// to change if it gets invalidated, so we can only use the
		// node for the life of this method
		internal void ToggleLogging(IEventLoggingNode node,
									bool start)
		{
			ObjectEvent oe = new ObjectEvent(node.LogEventObject,
											 node.LogEventInfo);
			ToggleLogging(oe, node.LogObjectName, start);
		}

		// Change the state of the logger with respect to event
		// logging
		internal void ToggleLogging(ObjectEvent oe,
									String name,
									bool start)
		{
			TraceUtil.WriteLineInfo(this, "Event " + oe + " ToggleLogging start: " + start);

			if (start)
			{
				if (IsLogging(oe._object, oe._eventInfo))
					return;

				// Make a separate object for each event to keep the event
				// name
				Delegate d = GetEventLoggerDelegate(oe);
				try
				{
					oe._eventInfo.AddEventHandler(oe._object, d);
				}
				catch (Exception ex)
				{
					TraceUtil.WriteLineWarning(this, "Exception while enabling logging: " + name + " " + ex);

					Console.WriteLine("Inner: " + ex.InnerException);

					Exception showException = ex;
					// Remove the useless wrapper exception
					if (showException is TargetInvocationException) {
						showException = ex.InnerException;
					}

					String moreText = "";
					if (showException is InvalidCastException)
					{
						moreText = "  If this is an Excel event, this "
							+ "exception is likely related to "
							+ "Microsoft problem Q316653 ("
							+ "http://support.microsoft.com/default.aspx?"
							+ "scid=kb;en-us;Q316653)"
							+ ".  "
							+ "To use events in Excel, please modify "
							+ "the assembly " + 
							oe._object.GetType().Assembly.GetName().Name
							+ " (in the converted assemblies directory) "
							+ "according to the instructions for that "
							+ "problem.  Once you have modified the assembly "
							+ "and restarted the Inspector, event "
							+ "tracing will work.";
					}

					// Undo the GetEventLoggerDelegate
					RemoveLoggerInstance(oe);
					throw new Exception("Error enabling logging for " + oe
										+ moreText,
										showException);
				}


				ListViewItem li = new ListViewItem(name);
				li.Tag = oe;
				_eventsBeingLogged.Items.Add(li);
				if (_eventsBeingLogged.Items.Count == 1)
				{
					_eventsBeingLoggedTabPage.Controls.Clear();
					_eventsBeingLoggedTabPage.Controls.Add(_eventsBeingLogged);
				}
				li.SubItems.Add(oe._eventInfo.Name);
			}
			else
			{
				if (!IsLogging(oe._object, oe._eventInfo))
					return;

				for (int i = 0; i < _eventsBeingLogged.Items.Count; i++)
				{
					ListViewItem li = 
						(ListViewItem)_eventsBeingLogged.Items[i];
					if (li.Tag.Equals(oe))
					{
						_eventsBeingLogged.Items.Remove(li);
						break;
					}
				}

				if (_eventsBeingLogged.Items.Count == 0)
				{
					_eventsBeingLoggedTabPage.Controls.Clear();
					_eventsBeingLoggedTabPage.Controls.Add(_eblText);
				}

				EventLogger logger = GetLoggerInstance(oe);
				Delegate d = Delegate.CreateDelegate(oe._eventInfo.EventHandlerType, logger, "Logger");
				oe._eventInfo.RemoveEventHandler(oe._object, d);
				RemoveLoggerInstance(oe);
			}
		}
	}
}
