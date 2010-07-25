// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Types
{

    // Represents a type that returns an EventHandlerList
	internal class EventHandlerListTypeHandler : BaseTypeHandler
	{

        protected static FieldInfo          _headField;
        protected static FieldInfo          _nextField;
        protected static FieldInfo          _handlerField;

        protected ArrayList                 _previousValues;

        static EventHandlerListTypeHandler()
        {
            _headField = typeof(EventHandlerList).
                GetField("head", 
                         BindingFlags.Instance | 
                         BindingFlags.NonPublic);

            Type leType = ReflectionHelper.GetType
                ("System.ComponentModel.EventHandlerList+ListEntry");
            _nextField = leType.
                GetField("next", 
                         BindingFlags.Instance | 
                         BindingFlags.NonPublic);
            _handlerField = leType.
                GetField("handler", 
                         BindingFlags.Instance | 
                         BindingFlags.NonPublic);
        }

        internal EventHandlerListTypeHandler
            (TypeHandlerManager.TypeHandlerInfo info,
             ObjectTreeNode node) : base(info, node)
        {
        }

        public override bool IsCurrent()
        {
            ArrayList currentValues = (ArrayList)GetChildren();

            bool current = Utils.IsArrayListEqual(_previousValues,
                                                  currentValues);

            _previousValues = currentValues;
            return current;
        }


        // Returns ObjectInfo objects for the children
        // Here we flatten out each delegate that will get an event
        public override ICollection GetChildren()
		{
            ArrayList retVal = new ArrayList();

            EventHandlerList obj =  (EventHandlerList)_node.ObjectInfo.Obj;

            if (obj != null)
            {
                Object listEntry = _headField.GetValue(obj);
                while (listEntry != null)
                {
                    Delegate handler = 
                        (Delegate)_handlerField.GetValue(listEntry);

                    // These can be null if the list was contracted
                    if (handler != null)
                    {
                        Delegate[] invList = handler.GetInvocationList();

                        foreach (Delegate d in invList)
                        {
                            // Skip the registrations associated with tracing
                            Assembly assembly = d.Target.GetType().Assembly;
                            if (assembly.GetName().Name.
                                Equals(AssemblySupport.DUMMY_ASSY_NAME))
                                continue;

                            ObjectInfo newObjInfo = ObjectInfoFactory.
                                GetObjectInfo(false, d);

                            // Name the thing according to its delegate
                            newObjInfo.ObjectName = 
                                d.Method.Name + " | " + 
                                d.Target.ToString();
                            retVal.Add(newObjInfo);
                        }
                    }

                    listEntry = _nextField.GetValue(listEntry);
                }
            }
            return retVal;
        }

        // Allocates the correct type of node
        // Expects an ObjectInfo object
        public override BrowserTreeNode AllocateChildNode(ObjectInfo objInfo)
        {
            return new ObjectTreeNode(_node.IsComNode, objInfo);
        }


        public override bool HasChildren()
        {
            if (_node.ObjectInfo.Obj != null)
            {
                EventHandlerList e = (EventHandlerList)_node.ObjectInfo.Obj;
                if (_headField.GetValue(e) != null)
                    return true;
            }
            return false;
        }
	}

}
