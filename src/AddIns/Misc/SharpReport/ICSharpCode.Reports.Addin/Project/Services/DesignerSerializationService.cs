/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 15.10.2007
 * Zeit: 11:02
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

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
