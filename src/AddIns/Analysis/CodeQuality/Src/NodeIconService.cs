using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.CodeQualityAnalysis
{
    public static class NodeIconService
    {
        private static readonly BitmapSource Namespace = GetImage("Icons.16x16.NameSpace");
        private static readonly BitmapSource Assembly = GetImage("Icons.16x16.Assembly");

        private static readonly BitmapSource Class = GetImage("Icons.16x16.Class");
        private static readonly BitmapSource InternalClass = GetImage("Icons.16x16.InternalClass");
        private static readonly BitmapSource ProtectedClass = GetImage("Icons.16x16.ProtectedClass");
        private static readonly BitmapSource PrivateClass = GetImage("Icons.16x16.PrivateClass");

        private static readonly BitmapSource Struct = GetImage("Icons.16x16.Struct");
        private static readonly BitmapSource InternalStruct = GetImage("Icons.16x16.InternalStruct");
        private static readonly BitmapSource ProtectedStruct = GetImage("Icons.16x16.ProtectedStruct");
        private static readonly BitmapSource PrivateStruct = GetImage("Icons.16x16.PrivateStruct");

        private static readonly BitmapSource Interface = GetImage("Icons.16x16.Interface");
        private static readonly BitmapSource InternalInterface = GetImage("Icons.16x16.InternalInterface");
        private static readonly BitmapSource ProtectedInterface = GetImage("Icons.16x16.ProtectedInterface");
        private static readonly BitmapSource PrivateInterface = GetImage("Icons.16x16.PrivateInterface");

        private static readonly BitmapSource Enum = GetImage("Icons.16x16.Enum");
        private static readonly BitmapSource InternalEnum = GetImage("Icons.16x16.InternalEnum");
        private static readonly BitmapSource ProtectedEnum = GetImage("Icons.16x16.ProtectedEnum");
        private static readonly BitmapSource PrivateEnum = GetImage("Icons.16x16.PrivateEnum");

        private static readonly BitmapSource Delegate = GetImage("Icons.16x16.Delegate");
        private static readonly BitmapSource InternalDelegate = GetImage("Icons.16x16.InternalDelegate");
        private static readonly BitmapSource ProtectedDelegate = GetImage("Icons.16x16.ProtectedDelegate");
        private static readonly BitmapSource PrivateDelegate = GetImage("Icons.16x16.PrivateDelegate");

        private static readonly BitmapSource Method = GetImage("Icons.16x16.Method");
        private static readonly BitmapSource ProtectedMethod = GetImage("Icons.16x16.ProtectedMethod");
        private static readonly BitmapSource PrivateMethod = GetImage("Icons.16x16.PrivateMethod");
        private static readonly BitmapSource PropertyMethod = GetImage("Icons.16x16.Property");
        private static readonly BitmapSource ProtectedPropertyMethod = GetImage("Icons.16x16.ProtectedProperty");
        private static readonly BitmapSource PrivatePropertyMethod = GetImage("Icons.16x16.PrivateProperty");
        
        private static readonly BitmapSource Field = GetImage("Icons.16x16.Field");
        private static readonly BitmapSource ProtectedField = GetImage("Icons.16x16.ProtectedField");
        private static readonly BitmapSource PrivateField = GetImage("Icons.16x16.PrivateField");
        private static readonly BitmapSource EventField = GetImage("Icons.16x16.Event");
        private static readonly BitmapSource ProtectedEventField = GetImage("Icons.16x16.ProtectedEvent");
        private static readonly BitmapSource PrivateEventField = GetImage("Icons.16x16.PrivateEvent");
        private static readonly BitmapSource ConstantField = GetImage("Icons.16x16.Literal");

        private static BitmapSource GetImage(string name)
        {
            try
            {
                return PresentationResourceService.GetBitmapSource(name);
            }
            catch (Exception)
            {
                return null; // image isnt needed neccessery
            }
            
        }

        public static BitmapSource GetIcon(Field field)
        {
            if (field.IsPublic)
                return Field;
            if (field.IsPrivate)
                return PrivateField;
            if (field.IsProtected)
                return ProtectedField;
            if (field.IsConstant)
                return ConstantField;

            if (field.IsEvent)
            {
                if (field.IsPublic)
                    return EventField;
                if (field.IsPrivate)
                    return PrivateEventField;
                if (field.IsProtected)
                    return ProtectedEventField;
            }

            return Field;
        }

        public static BitmapSource GetIcon(Method method)
        {
            if (method.IsPublic)
                return Method;
            if (method.IsPrivate)
                return PrivateMethod;
            if (method.IsProtected)
                return ProtectedMethod;

            if (method.IsGetter || method.IsSetter)
            {
                if (method.IsPublic)
                    return PropertyMethod;
                if (method.IsPrivate)
                    return PrivatePropertyMethod;
                if (method.IsProtected)
                    return ProtectedPropertyMethod; 
            }

            return Method;
        }

        public static BitmapSource GetIcon(Module module)
        {
            return Assembly;
        }

        public static BitmapSource GetIcon(Namespace ns)
        {
            return Namespace;
        }

        public static BitmapSource GetIcon(Type type)
        {
            if (type.IsEnum)
            {
                if (type.IsPublic)
                    return Enum;
                if (type.IsNestedPrivate)
                    return PrivateEnum;
                if (type.IsNestedProtected)
                    return ProtectedEnum;
                if (type.IsInternal)
                    return InternalEnum;
            }
            else if (type.IsStruct)
            {
                if (type.IsPublic)
                    return Struct;
                if (type.IsNestedPrivate)
                    return PrivateStruct;
                if (type.IsNestedProtected)
                    return ProtectedStruct;
                if (type.IsInternal)
                    return InternalStruct;
            }
            else if (type.IsInterface)
            {
                if (type.IsPublic)
                    return Interface;
                if (type.IsNestedPrivate)
                    return PrivateInterface;
                if (type.IsNestedProtected)
                    return ProtectedInterface;
                if (type.IsInternal)
                    return InternalInterface;
            }
            else if (type.IsDelegate)
            {
                if (type.IsPublic)
                    return Delegate;
                if (type.IsNestedPrivate)
                    return PrivateDelegate;
                if (type.IsNestedProtected)
                    return ProtectedDelegate;
                if (type.IsInternal)
                    return InternalDelegate;
            }

            if (type.IsPublic)
                return Class;
            if (type.IsNestedPrivate)
                return PrivateClass;
            if (type.IsNestedProtected)
                return ProtectedClass;
            if (type.IsInternal)
                return InternalClass;

            return Class;
        }
    }
}
