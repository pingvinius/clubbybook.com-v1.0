namespace ClubbyBook.Common.Utilities
{
    using System;
    using System.ComponentModel;

    public static class AttributeHelper
    {
        public static string GetEnumValueDescription(object obj)
        {
            DescriptionAttribute description = GetEnumValueAttribute<DescriptionAttribute>(obj);
            if (description != null)
                return description.Description;

            return string.Empty;
        }

        public static string GetEnumValueUrlRewrite(object obj)
        {
            UrlRewriteAttribute description = GetEnumValueAttribute<UrlRewriteAttribute>(obj);
            if (description != null)
                return description.UrlRewrite;

            return string.Empty;
        }

        public static object FindEnumValueByUrlRewrite<EnumType>(string urlRewrite)
        {
            if (string.IsNullOrEmpty(urlRewrite))
                throw new ArgumentNullException("urlRewrite");

            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                string valueUrlRewrite = GetEnumValueUrlRewrite(value);
                if (!string.IsNullOrEmpty(valueUrlRewrite) && valueUrlRewrite == urlRewrite)
                    return value;
            }

            return null;
        }

        private static AttributeType GetEnumValueAttribute<AttributeType>(object obj) where AttributeType : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            object[] attributes = obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(AttributeType), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0] as AttributeType;
            return null;
        }
    }
}