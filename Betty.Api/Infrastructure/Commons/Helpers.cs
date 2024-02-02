using System.Reflection;

namespace Betty.Api.Commons
{
    public static class Helpers
    {
        public static void SetProperty(object containingObject, string propertyName, object newValue)
        {
            containingObject.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, containingObject, new object[] { newValue });
        }
    }
}
