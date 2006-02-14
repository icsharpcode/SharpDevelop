// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Debugger.Wrappers
{
	class TrackedObjectMetaData
	{
		public Type ObjectType;
		public int RefCount;
		
		public TrackedObjectMetaData(Type objectType, int refCount)
		{
			this.ObjectType = objectType;
			this.RefCount = refCount;
		}
	}
	
	public static class ResourceManager
	{
		static bool trace;
		static Dictionary<object, TrackedObjectMetaData> trackedCOMObjects = new Dictionary<object, TrackedObjectMetaData>();
		
		public static bool TraceMessagesEnabled {
			get {
				return trace;
			}
			set {
				trace = value;
			}
		}
		
		public static void TrackCOMObject(object comObject, Type type)
		{
			if (!Marshal.IsComObject(comObject)) {
				if (trace) Trace("Will not be tracked: {0}", type.Name);
			} else {
				TrackedObjectMetaData metaData;
				if (trackedCOMObjects.TryGetValue(comObject, out metaData)) {
					metaData.RefCount += 1;
				} else {
					metaData = new TrackedObjectMetaData(type,1);
					trackedCOMObjects.Add(comObject, metaData);
				}
				if (trace) Trace("AddRef {0,2}: {1}", metaData.RefCount, type.Name);
			}
		}
		
		public static void ReleaseCOMObject(object comObject, Type type)
		{
			TrackedObjectMetaData metaData;
			if (trackedCOMObjects.TryGetValue(comObject, out metaData)) {
				metaData.RefCount -= 1;
				if (metaData.RefCount == 0) {
					Marshal.FinalReleaseComObject(comObject);
					trackedCOMObjects.Remove(comObject);
				}
				if (trace) Trace("Release {0,2}: {1}", metaData.RefCount, type.Name);
			} else {
				if (trace) Trace("Was not tracked: {0}", type.Name);
			}
		}
		
		public static void ReleaseAllTrackedCOMObjects()
		{
			if (trace) Trace("Releasing {0} tracked COM objects... ", trackedCOMObjects.Count);
			while(trackedCOMObjects.Count > 0) {
				foreach (KeyValuePair<object, TrackedObjectMetaData> pair in trackedCOMObjects) {
					Marshal.FinalReleaseComObject(pair.Key);
					if (trace) Trace(" * Releasing {0} ({1} references)", pair.Value.ObjectType.Name, pair.Value.RefCount);
					trackedCOMObjects.Remove(pair.Key);
					break;
				}
			}
			if (trace) Trace(" * Done");
		}
		
		public static event EventHandler<MessageEventArgs> TraceMessage;
		
		static void Trace(string msg, params object[] pars)
		{
			if (TraceMessage != null && trace) {
				string message = String.Format("COM({0,-3}): {1}", trackedCOMObjects.Count, String.Format(msg, pars));
				TraceMessage(null, new MessageEventArgs(null, message));
			}
		}
	}
}
