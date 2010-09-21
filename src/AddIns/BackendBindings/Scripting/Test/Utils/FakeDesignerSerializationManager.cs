// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeDesignerSerializationManager : IDesignerSerializationManager
	{
		public Type TypeToReturnFromGetType;
		public string TypeNamePassedToGetType;
		public object InstanceToReturnFromGetInstance;
		public string NamePassedToGetInstance;
		public object InstanceToReturnFromCreateInstance;
		public Type TypePassedToCreateInstance;
		public ICollection ArgumentsPassedToCreateInstance;
		public string NamePassedToCreateInstance;
		public bool AddToContainerPassedToCreateInstance;
		
		public event ResolveNameEventHandler ResolveName;
		
		protected virtual void OnResolveName(ResolveNameEventArgs e)
		{
			if (ResolveName != null) {
				ResolveName(this, e);
			}
		}
		
		public event EventHandler SerializationComplete;
		
		protected virtual void OnSerializationComplete(EventArgs e)
		{
			if (SerializationComplete != null) {
				SerializationComplete(this, e);
			}
		}
		
		public ContextStack Context {
			get {
				throw new NotImplementedException();
			}
		}
		
		public PropertyDescriptorCollection Properties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void AddSerializationProvider(IDesignerSerializationProvider provider)
		{
			throw new NotImplementedException();
		}
		
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			TypePassedToCreateInstance = type;
			ArgumentsPassedToCreateInstance = arguments;
			NamePassedToCreateInstance = name;
			AddToContainerPassedToCreateInstance = addToContainer;
			return InstanceToReturnFromCreateInstance;
		}
		
		public object GetInstance(string name)
		{
			NamePassedToGetInstance = name;
			return InstanceToReturnFromGetInstance;
		}
		
		public string GetName(object value)
		{
			throw new NotImplementedException();
		}
		
		public object GetSerializer(Type objectType, Type serializerType)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string typeName)
		{
			TypeNamePassedToGetType = typeName;
			return TypeToReturnFromGetType;
		}
		
		public void RemoveSerializationProvider(IDesignerSerializationProvider provider)
		{
			throw new NotImplementedException();
		}
		
		public void ReportError(object errorInformation)
		{
			throw new NotImplementedException();
		}
		
		public void SetName(object instance, string name)
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}
