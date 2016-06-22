// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
