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
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
	
namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// This class implements the IDesignerSerializationService interface.
	/// A designer loader that does not derive from the CodeDOMDesignerLoader class 
	/// (e.g. the XmlDesignerLoader) need to create an instance of this class
	/// and it to the available services otherwise cut/copy and paste will not work.
	/// </summary>
	public class DesignerSerializationService : IDesignerSerializationService
	{
		IServiceProvider serviceProvider;
		public DesignerSerializationService(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			this.serviceProvider = serviceProvider;
		}
		
		/// <summary>
		/// Deserializes the serialization data object and returns a 
		/// collection of objects represented by that data. 
		/// </summary>
		public ICollection Deserialize(object serializationData)
		{
			IDesignerHost host = (IDesignerHost)this.serviceProvider.GetService(typeof(IDesignerHost));
			if (host != null) {
				ComponentSerializationService serializationService = (ComponentSerializationService)serviceProvider.GetService(typeof(ComponentSerializationService));
				return serializationService.Deserialize((SerializationStore)serializationData,host.Container);
			}
			return null;
		}
		
		/// <summary>
		/// Serializes a collection of objects and stores them in a 
		/// serialization data object. 
		/// </summary>
		public object Serialize(ICollection objects)
		{
			if (objects == null) {
				throw new ArgumentNullException("objects");
			}
			ComponentSerializationService serializationService = (ComponentSerializationService)serviceProvider.GetService(typeof(ComponentSerializationService));
			SerializationStore store = serializationService.CreateStore();
			foreach (object value in objects) {
				serializationService.Serialize(store, value);
			}
			store.Close();
			return store;
		}
	}
}
