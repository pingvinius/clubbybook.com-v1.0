namespace ClubbyBook.Common.Utilities
{
    using System.Text;

    public struct JSONKeyValuePair
    {
        private string key;
        private object value;

        public JSONKeyValuePair(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }
    }

    public static class JSONHelper
    {
        public static string FromArray(params JSONKeyValuePair[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            if (array != null)
            {
                string[] strArray = new string[array.Length];
                for (int i = 0; i < strArray.Length; i++)
                    strArray[i] = string.Format("'{0}':'{1}'", array[i].Key, array[i].Value);
                sb.Append(string.Join(",", strArray));
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}