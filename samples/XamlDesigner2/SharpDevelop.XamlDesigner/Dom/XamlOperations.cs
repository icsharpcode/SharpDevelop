using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using System.IO;
using System.Windows;
using System.Xaml.Schema;
using SharpDevelop.XamlDesigner.Dom.TypeReplacement;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.Dom
{
	class XamlOperations
	{
		//static XamlOperations()
		//{
		//    var parent = TypeDescriptor.GetProvider(typeof(Window));
		//    TypeDescriptor.AddProvider(new SimulateTypeDescriptionProvider(parent), typeof(Window));
		//}

		public static DesignItem Parse(DesignContext context)
		{
			var text = context.TextHolder.Text;
			var nodes = new XamlValidatingReader(new XamlXmlReader(new StringReader(text))).ReadToEnd();
			Dictionary<XamlNode, DesignItem> map = new Dictionary<XamlNode, DesignItem>();
			var stack = new Stack<object>();
			DesignItem currentItem = null;
			DesignItem root = null;

			foreach (var node in nodes) {
				switch (node.NodeType) {
					case XamlNodeType.Record:
						var type = (node as	XamlTypedStartRecordNode).RecordType;
						var clrType = type.GetClrType();
						var item = new DesignItem(context, clrType);
						if (stack.Count == 0) {
							root = item;
						}
						else {
							var parentProperty = stack.Peek() as DesignProperty;
							if (parentProperty.Collection != null) {
								parentProperty.Collection.ParserAdd(item);
							}
							else {
								parentProperty.ParserSetValue(item);
							}
						}
						map[node] = item;
						stack.Push(item);
						currentItem = item;
						break;
					case XamlNodeType.EndRecord:
						var type2 = (node as XamlTypedEndRecordNode).RecordType;
						var clrType2 = type2.GetClrType();
						if (typeof(Window).IsAssignableFrom(clrType2)) {
							XamlClrProperties.SetClrType(type2, typeof(DesignTimeWindow));
						}
						currentItem = stack.Pop() as DesignItem;
						break;
					case XamlNodeType.Member:
						var memberIdentifier = (node as XamlTypedStartMemberNode).MemberIdentifier;
						var schemaType = XamlSchemaTypeResolver.Default.Resolve(
							new TypeReference(memberIdentifier.TypeName, null));
						DesignProperty property = null;
						if (schemaType != null) {
							property = currentItem.Property(schemaType.GetClrType(), memberIdentifier.MemberName);
						}
						else {
							property = currentItem.Property(memberIdentifier.MemberName);
						}
						stack.Push(property);
						break;
					case XamlNodeType.EndMember:
						stack.Pop();
						break;
					case XamlNodeType.Atom:
						var atomItem = context.CreateItem((node as XamlAtomNode).Value);
						(stack.Peek() as DesignProperty).ParserSetValue(atomItem);
						break;
				}
			}

			var list = new List<XamlNode>();
			var attachedItemId = MemberIdentifier.Get(typeof(DesignItem), "AttachedItem");

			foreach (var node in nodes) {
				list.Add(node);
				if (node.NodeType == XamlNodeType.Record) {
					list.Add(new XamlStartMemberNode() { MemberIdentifier = attachedItemId });
					list.Add(new XamlAtomNode() { Value = map[node] });
					list.Add(new XamlEndMemberNode() { MemberIdentifier = attachedItemId });
				}
			}

			var reader = new XamlValidatingReader(new XamlReader(list));
			XamlServices.Load(reader);
			return root;
		}

		public static void SetValue(DesignProperty property)
		{
			var propertyId = property.MemberId as PropertyId;
			if (propertyId != null) {
				if (property.IsSet) {
					propertyId.Descriptor.SetValue(property.ParentItem.Instance, property.Value.Instance);
				}
				else {
					propertyId.Descriptor.ResetValue(property.ParentItem.Instance);
				}
			}
		}

		public static void Insert(DesignItemCollection collection, int index, DesignItem item)
		{
		}

		public static void Remove(DesignItemCollection collection, DesignItem item)
		{
		}

		public static DesignItem Clone(DesignItem item)
		{
			throw new NotImplementedException();
		}

		//class Resolver : XamlSchemaTypeResolver
		//{
		//    public override SchemaType Resolve(TypeReference typeReference)
		//    {
		//        var result = base.Resolve(typeReference);
		//        var type = result.GetClrType();
		//        if (typeof(Window).IsAssignableFrom(type)) {
		//            //result = GenerateSchemaType(typeof(DesignTimeWindow));
		//            XamlClrProperties.SetClrType(result, typeof(DesignTimeWindow));
		//        }
		//        else if (type == typeof(Application)) {
		//            result = GenerateSchemaType(typeof(DesignTimeApplication));
		//        }
		//        return result;
		//    }
		//}
	}
}
