using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace DynamicFilterDemo.Helpers
{
    public class AppHelper
    {
        public static object ConvertValue(object value, Type destinationType)
        {
            return ConvertValue(value, destinationType, CultureInfo.CurrentCulture);
        }

        public static object ConvertValue(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                if (destinationType == null)
                {
                    throw new ArgumentNullException("destinationType");
                }
                var sourceType = value.GetType();

                if (destinationType == typeof(decimal) || destinationType == typeof(decimal?) && sourceType == typeof(string))
                {
                    value = value.ToString().Replace(",", ".");
                    culture = CultureInfo.InvariantCulture;
                }

                var destinationConverter = GetTypeConverter(destinationType);
                var sourceConverter = GetTypeConverter(sourceType);

                if (destinationConverter != null && destinationConverter.CanConvertFrom(sourceType))
                {
                    var item = destinationConverter.ConvertFrom(null, culture, value);
                    return item;
                }

                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                {
                    var item = sourceConverter.ConvertTo(null, culture, value, destinationType);
                    return item;
                }

                if (destinationType.IsEnum && value is int)
                {
                    var item = Enum.ToObject(destinationType, (int)value);
                    return item;
                }

                if (!destinationType.IsInstanceOfType(value))
                {
                    var item = Convert.ChangeType(value, destinationType, culture);
                    return item;
                }
            }
            return value;
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var converterType = typeof(ListTypeConverter<>);
                var elementType = type.GetGenericArguments()[0];

                converterType = converterType.MakeGenericType(elementType);
                var item = Activator.CreateInstance(converterType) as TypeConverter;
                return item;
            }

            var result = TypeDescriptor.GetConverter(type);
            return result;
        }
    }


    public class ListTypeConverter<T> : TypeConverter
    {
        private readonly TypeConverter _ElementConverter;

        public ListTypeConverter()
        {
            var type = typeof(T);
            _ElementConverter = TypeDescriptor.GetConverter(type);
            if (_ElementConverter == null)
            {
                throw new Exception(string.Format("Type converter for the type '{0}' could not found", type.FullName));
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                var items = new List<T>();
                var valueType = value.GetType();
                if (valueType == typeof(string))
                {
                    var strValue = value.ToString();
                    var strArr = strValue.Split(',');

                    items.AddRange(
                        from t in strArr
                        select _ElementConverter.ConvertFromInvariantString(t)
                        into item
                        where item != null
                        select (T)item
                    );
                }
                return items;
            }
            var result = base.ConvertFrom(context, culture, null);
            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var text = new StringBuilder();
                var elements = value as IList<T>;
                if (elements != null)
                {
                    for (var i = 0; i < elements.Count; i++)
                    {
                        if (i != 0)
                        {
                            text.Append(",");
                        }
                        text.Append(Convert.ToString(elements[i], CultureInfo.InvariantCulture));
                    }
                }
                return text.ToString();
            }
            var result = base.ConvertTo(context, culture, value, destinationType);
            return result;
        }
    }

    public class AppTypeInformation
    {

        internal const BindingFlags DEFAULT_FLAG = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private Dictionary<Type, TypeInformation> _Types = new Dictionary<Type, TypeInformation>();

        private object lockObj = new object();

        public TypeInformation this[Type type]
        {
            get
            {
                if (_Types.ContainsKey(type))
                    return _Types[type];

                lock (lockObj)
                {
                    if (!_Types.ContainsKey(type))
                    {
                        TypeInformation information = new TypeInformation(type);

                        _Types.Add(type, information);
                    }

                    return _Types[type];
                }
            }
        }

        public List<PropertyInformation> ResolveProperties(Type type)
        {
            return ResolveProperties(type, false);
        }

        public List<PropertyInformation> ResolveProperties(Type type, bool includeNonPublic)
        {
            TypeInformation information = this[type];

            return information.ResolveProperties();
        }

        public PropertyInformation GetProperty(Type type, string name)
        {
            TypeInformation information = this[type];

            return information.GetProperty(name);
        }

        public List<MethodInformation> ResolveMethods(Type type, bool nonPublic)
        {
            TypeInformation information = this[type];

            return information.ResolveMethods(nonPublic);
        }
        public List<MethodInformation> ResolveMethods(Type type)
        {
            return ResolveMethods(type, false);
        }

        public MethodInformation GetMethod(Type type, string name)
        {
            TypeInformation information = this[type];

            return information.GetMethod(name);
        }

        public IEnumerable<T> GetAttributes<T>(Type type) where T : Attribute
        {
            TypeInformation information = this[type];

            return information.GetCustomAttributes<T>();
        }
        public bool ContainsCache(Type type)
        {
            return _Types.ContainsKey(type);
        }

        internal void Clear()
        {
            _Types.Clear();
        }
    }


    public abstract class MemberInformation : IEnumerable<Attribute>
    {
        protected List<Attribute> Attributes { get; set; }

        protected abstract ICustomAttributeProvider Provider { get; }


        public List<Attribute> ResolveAttributes()
        {
            if (Attributes == null)
            {
                Attributes = Provider.GetCustomAttributes(true).ToList().ConvertAll<Attribute>(t => t as Attribute); //OnResolveAttributes();
            }

            return Attributes;
        }


        public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute
        {
            List<Attribute> attributes = ResolveAttributes();

            return attributes.OfType<T>();
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            Attributes = ResolveAttributes();

            return Attributes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class PropertyInformation : MemberInformation
    {
        public PropertyInfo Info { get; private set; }

        protected override ICustomAttributeProvider Provider
        {
            get { return Info; }
        }

        public PropertyInformation(PropertyInfo info)
        {
            if (info == null)
                throw new Exception("Property info");
            Info = info;
        }
    }

    public class TypeInformation : MemberInformation
    {
        private object lockObj = new object();

        public Type Type { get; private set; }

        private List<PropertyInformation> Properties { get; set; }

        private List<MethodInformation> Methods { get; set; }

        protected override ICustomAttributeProvider Provider
        {
            get { return Type; }
        }

        public TypeInformation(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Type");
            }
            Type = type;
        }
        public List<PropertyInformation> ResolveProperties()
        {
            if (Properties != null)
            {
                return Properties;
            }

            lock (lockObj)
            {
                if (Properties == null)
                {
                    Properties = new List<PropertyInformation>();

                    List<PropertyInfo> properties = Type.GetProperties(AppTypeInformation.DEFAULT_FLAG).ToList();

                    properties.ForEach(p =>
                    {
                        Properties.Add(new PropertyInformation(p));
                    });
                }
                return Properties;
            }
        }

        public PropertyInformation GetProperty(string name)
        {
            List<PropertyInformation> properties = ResolveProperties();

            return properties.Where(p => p.Info.Name == name).FirstOrDefault();
        }

        public List<MethodInformation> ResolveMethods()
        {
            return ResolveMethods(false);
        }

        public List<MethodInformation> ResolveMethods(bool includeNonPublic)
        {
            if (Methods != null)
            {
                if (includeNonPublic)
                {
                    return Methods;
                }

                return Methods.Where(t => t.IsPublic).ToList();
            }

            lock (lockObj)
            {
                if (Methods == null)
                {
                    Methods = new List<MethodInformation>();

                    List<MethodInfo> methods = Type.GetMethods(AppTypeInformation.DEFAULT_FLAG).ToList();

                    methods.ForEach(p =>
                    {
                        Methods.Add(new MethodInformation(p));
                    });
                }
            }

            if (includeNonPublic)
            {
                return Methods;
            }

            return Methods.Where(t => t.IsPublic).ToList();
        }

        public MethodInformation GetMethod(string name)
        {
            List<MethodInformation> methods = ResolveMethods(true);

            return methods.Where(p => p.Info.Name == name).FirstOrDefault();
        }



        internal int GetCachedPropertyCount()
        {
            return GetCachedPropertyCount(false);
        }

        internal int GetCachedPropertyCount(bool includeNonPubliic)
        {
            if (Properties == null)
                return 0;
            if (includeNonPubliic)
                return Properties.Count;
            return Properties.Count();
        }
    }

    public class MethodInformation : MemberInformation
    {
        public MethodInfo Info { get; private set; }

        protected override ICustomAttributeProvider Provider
        {
            get { return Info; }
        }

        public bool IsPublic
        {
            get
            {
                return Info.IsPublic;
            }
        }

        public MethodInformation(MethodInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("Method info");
            Info = info;
        }
    }
}
