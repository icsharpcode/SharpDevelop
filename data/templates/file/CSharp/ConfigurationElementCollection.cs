${StandardHeader.C#}
using System;
using System.Configuration;


namespace ${StandardNamespace}
{
	/// <summary>
	/// A collection of ${ClassName}Element(s).
	/// </summary>
	public sealed class ${ClassName}Collection : ConfigurationElementCollection
	{
		#region Properties

		/// <summary>
		/// Gets the CollectionType of the ConfigurationElementCollection.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}
	   

		/// <summary>
		/// Gets the Name of Elements of the collection.
		/// </summary>
		protected override string ElementName
		{
		get { return "${ClassName}"; }
		}
			   
	   
		/// <summary>
		/// Retrieve and item in the collection by index.
		/// </summary>
		public ${ClassName}Element this[int index]
		{
			get   { return (${ClassName}Element)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}


		#endregion

		/// <summary>
		/// Adds a ${ClassName}Element to the configuration file.
		/// </summary>
		/// <param name="element">The ${ClassName}Element to add.</param>
		public void Add(${ClassName}Element element)
		{
			BaseAdd(element);
		}
	   
	   
		/// <summary>
		/// Creates a new ${ClassName}Element.
		/// </summary>
		/// <returns>A new <c>${ClassName}Element</c></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ${ClassName}Element();
		}

	   
	   
		/// <summary>
		/// Gets the key of an element based on it's Id.
		/// </summary>
		/// <param name="element">Element to get the key of.</param>
		/// <returns>The key of <c>element</c>.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((${ClassName}Element)element).Name;
		}
	   
	   
		/// <summary>
		/// Removes a ${ClassName}Element with the given name.
		/// </summary>
		/// <param name="name">The name of the ${ClassName}Element to remove.</param>
		public void Remove (string name) {
			base.BaseRemove(name);
		}

	}
}

