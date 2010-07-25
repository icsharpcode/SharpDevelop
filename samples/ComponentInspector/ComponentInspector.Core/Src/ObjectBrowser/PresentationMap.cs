// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser
{
	// Information about the member type
	internal class PresentationInfo
	{
		internal int                    _iconIndex;
		internal int                    _sortOrder;

		// K(qualified key) V(PresentationInfo)
		// Used for cases where further qualification is necessary
		// to get the image, like the method visibility or property
		// accessability.
		internal Hashtable              _qualifiedImages;
		internal IntermediateNodeType   _intermediateNodeType;
	}

	// Maps member types to the correct icon
	internal class PresentationMap
	{
		// Types for icons not represented by a class
		internal const String FOLDER_OPEN           = "com_folder_open";
		internal const String FOLDER_CLOSED         = "com_folder_closed";
		internal const String COM_FOLDER_APPID      = "com_folder_appid";
		internal const String COM_FOLDER_CLASS      = "com_folder_class";
		internal const String COM_FOLDER_INTERFACE  = "com_folder_interface";
		internal const String COM_FOLDER_TYPELIB    = "com_folder_typelib";
		internal const String COM_METHOD            = "com_method";
		internal const String COM_PROPERTY          = "com_property";
		internal const String COM_VARIABLE          = "com_variable";
		internal const String BASE_CLASS            = "base_class";
		internal const String COM_BASEINT           = "com_baseint";
		internal const String COM_EVENTINT          = "com_eventint";
		internal const String COM_TYPELIB           = "com_typelib";

		static internal ImageList _icons = new ImageList();

		// K(object that idenfities icon) V(PresentationInfo)
		static internal Hashtable _presentationToIndex = new Hashtable();

		// Property accessability constants
		internal const int PROP_READ_ONLY = 1;
		internal const int PROP_READ_WRITE = 2;
		internal const int PROP_WRITE_ONLY = 3;
		
		static ResourceManager  _rm;

		static PresentationMap()
		{
			_rm = new ResourceManager("ComponentInspector.Core.Resources.BitmapResources", typeof(PresentationMap).Assembly); 
			
			int index = 0;

			// Add the icons for the member types with the right sort order
			AddPresInfo(_presentationToIndex, MemberTypes.Constructor, 
						index, 0,
						"vxmethod_icon", 
						IntermediateNodeType.INTNODE_METHOD);

			AddPresInfo(_presentationToIndex, MemberTypes.Event, 
						++index, 6,
						"vxevent_icon", 
						IntermediateNodeType.INTNODE_EVENT);

			AddPresInfo(_presentationToIndex, MemberTypes.Field, 
						++index, 2,
						"vxfield_icon", 
						IntermediateNodeType.INTNODE_FIELD);

			AddPresInfo(_presentationToIndex, MemberTypes.Method, 
						++index, 5,
						"vxmethod_icon", 
						IntermediateNodeType.INTNODE_METHOD);

			AddPresInfo(_presentationToIndex, MemberTypes.NestedType, 
						++index, 10,
						"vxclass_icon");

			AddPresInfo(_presentationToIndex, MemberTypes.TypeInfo, 
						++index, 1,
						"vxtypedef_icon");

			AddPresInfo(_presentationToIndex, BASE_CLASS, 
						++index, 1,
						"vxclass_icon",
						IntermediateNodeType.INTNODE_BASECLASS);

			// Property gets icons for visibility
			AddPresInfo(_presentationToIndex, MemberTypes.Property, 
						++index, 4,
						"vxproperty_icon", 
						IntermediateNodeType.INTNODE_PROPERTY);

			// FIXME - finish this when I get the icons

			// Add the other kinds of icons, using our TreeNode classes
			// as the indexing object
			AddPresInfo(_presentationToIndex, typeof(AssemblyTreeNode),
						++index, 0,
						"vxassembly_icon");

			AddPresInfo(_presentationToIndex, typeof(TypeTreeNode),
						++index, 0, 
						"vxclass_icon");

			AddPresInfo(_presentationToIndex, typeof(NamespaceTreeNode),
						++index, 0,
						"vxnamespace_icon");

			AddPresInfo(_presentationToIndex, typeof(ModuleTreeNode),
						++index, 0,
						"vxmodule_icon");

			AddPresInfo(_presentationToIndex, typeof(ObjectTreeNode),
						++index, 0,
						"gears");

			// Add the kinds for the various types
			AddPresInfo(_presentationToIndex, ObjectInfo.CLASS, 
						++index, 4,
						"vxclass_icon",
						IntermediateNodeType.INTNODE_CLASS);

			AddPresInfo(_presentationToIndex, ObjectInfo.ENUM, 
						++index, 2,
						"vxenum_icon",
						IntermediateNodeType.INTNODE_ENUM);

			AddPresInfo(_presentationToIndex, ObjectInfo.INTERFACE,
						++index, 4,
						"vxinterface_icon",
						IntermediateNodeType.INTNODE_INTERFACE);

			AddPresInfo(_presentationToIndex, ObjectInfo.STRUCT,
						++index, 3,
						"vxstruct_icon",
						IntermediateNodeType.INTNODE_STRUCT);

			// Icon for the gac tree
			AddPresInfo(_presentationToIndex, typeof(GacList),
						++index, 0,
						"gac");

			// Folder icons
			AddPresInfo(_presentationToIndex, FOLDER_CLOSED,
						++index, 0, "FolderClosed");

			AddPresInfo(_presentationToIndex, FOLDER_OPEN,
						++index, 0, "FolderOpened");

			// COM
			AddPresInfo(_presentationToIndex, COM_TYPELIB,
						++index, 0,
						"TypeLib");

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_COCLASS,
						++index, 14,
						"vxclass_icon",
						IntermediateNodeType.INTNODE_COM_COCLASS);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_ENUM,
						++index, 2,
						"vxenum_icon",
						IntermediateNodeType.INTNODE_COM_ENUM);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_INTERFACE,
						++index, 8,
						"vxinterface_icon",
						IntermediateNodeType.INTNODE_COM_INTERFACE);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_DISPATCH,
						++index, 10,
						"DispInterface",
						IntermediateNodeType.INTNODE_COM_DISPINTERFACE);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_RECORD,
						++index, 4,
						"vxstruct_icon",
						IntermediateNodeType.INTNODE_COM_STRUCT);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_UNION,
						++index, 18,
						"vxunion_icon",
						IntermediateNodeType.INTNODE_COM_UNION);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_ALIAS,
						++index, 16,
						"vxtypedef_icon",
						IntermediateNodeType.INTNODE_COM_TYPEDEF);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_MODULE,
						++index, 6,
						"ComModule",
						IntermediateNodeType.INTNODE_COM_MODULE);

			AddPresInfo(_presentationToIndex, COM_BASEINT,
						++index, 3,
						"vxinterface_icon",
						IntermediateNodeType.INTNODE_COM_BASEINT);

			AddPresInfo(_presentationToIndex, COM_EVENTINT,
						++index, 12,
						"vxevent_icon",
						IntermediateNodeType.INTNODE_COM_EVENTINT);

			AddPresInfo(_presentationToIndex, COM_METHOD,
						++index, 0, "vxmethod_icon");

			AddPresInfo(_presentationToIndex, COM_PROPERTY,
						++index, 0, "vxproperty_icon");

			AddPresInfo(_presentationToIndex, COM_VARIABLE,
						++index, 0, "vxfield_icon");

			AddPresInfo(_presentationToIndex, COM_FOLDER_APPID,
						++index, 0, "ComClassFolder");

			AddPresInfo(_presentationToIndex, COM_FOLDER_INTERFACE,
						++index, 0, "ComInterfaceFolder");

			AddPresInfo(_presentationToIndex, COM_FOLDER_CLASS,
						++index, 0, "ComClassFolder");

			AddPresInfo(_presentationToIndex, COM_FOLDER_TYPELIB,
						++index, 0, "ComTypeLibFolder");
		}
		
		/**

		internal static void AddComImageStrip(ImageList imageList)

		{

			Bitmap image = (Bitmap)GetImage("activex");

			image.MakeTransparent();

			imageList.Images.AddStrip(image);



			// COM - the image indexes correspond to the image strip

			AddPresInfo(_presentationToIndex, typeof(ComTypeLibTreeNode),

						2, 0, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_COCLASS,

						22, 4, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_ENUM,

						17, 2, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_INTERFACE,

						20, 4, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_DISPATCH,

						20, 4, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_RECORD,

						18, 3, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_UNION,

						24, 3, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_ALIAS,

						23, 3, null);

			AddPresInfo(_presentationToIndex, TYPEKIND.TKIND_MODULE,

						19, 3, null);



			// Folder icons

			AddPresInfo(_presentationToIndex, COM_FOLDER_CLOSED,

						0, 0, null);

			AddPresInfo(_presentationToIndex, COM_FOLDER_OPEN,

						1, 0, null);

			AddPresInfo(_presentationToIndex, COM_METHOD,

						14, 0, null);

			AddPresInfo(_presentationToIndex, COM_PROPERTY,

						15, 0, null);



			int imageCount = imageList.Images.Count;



			image = (Bitmap)GetImage("activexOuter");

			image.MakeTransparent();

			imageList.Images.AddStrip(image);



			AddPresInfo(_presentationToIndex, COM_FOLDER_APPID,

						imageCount + 13, 0, null);

			AddPresInfo(_presentationToIndex, COM_FOLDER_INTERFACE,

						imageCount + 15, 0, null);

			AddPresInfo(_presentationToIndex, COM_FOLDER_CLASS,

						imageCount + 13, 0, null);

			AddPresInfo(_presentationToIndex, COM_FOLDER_TYPELIB,

						imageCount + 14, 0, null);

		}

		**/

		internal static ImageList ImageList {
			get {
				return _icons;
			}
		}

		internal static PresentationInfo GetInfo(Object obj)
		{
			return (PresentationInfo)_presentationToIndex[obj];
		}

		internal static Icon GetApplicationIcon()
		{
			return (Icon)_rm.GetObject(_imageRoot + "CompInsp");
		}

		protected static void AddPresInfo(Hashtable hash,
										  Object obj,
										  int index,
										  int order,
										  String iconName,
										  int intNode)

		{
			PresentationInfo pi = AddPresInfo(hash, obj, index, order, iconName);
			pi._intermediateNodeType = IntermediateNodeType.GetNodeType(intNode);
			pi._intermediateNodeType.PresentationInfo = pi;
		}

		protected static Object GetImage(String iconName)
		{
			String resName = _imageRoot + iconName;
			return _rm.GetObject(resName);
		}

		protected static PresentationInfo AddPresInfo(Hashtable hash,
													  Object obj,
													  int index,
													  int order,
													  String iconName)
		{
			PresentationInfo mt = new PresentationInfo();
			mt._iconIndex = index;
			mt._sortOrder = order;

			if (index == 0)
				mt._qualifiedImages = new Hashtable();

			hash.Add(obj, mt);

			if (iconName != null) {
				Object icon = GetImage(iconName);

				if (icon is Bitmap)
					((Bitmap)icon).MakeTransparent();

				if (icon is Icon)
					_icons.Images.Add((Icon)icon);
				else
					_icons.Images.Add((Image)icon);
			}
			return mt;
		}
		
		protected const String _imageRoot = "ComponentInspector.";
	}
}

