using System;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class TypeUtils
    {
        public static Type GetTypeByName(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
