using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class EventBuilderExtensions
    {
        private static readonly FieldInfo nameField = typeof(EventBuilder).GetField("m_name", BindingFlags.NonPublic | BindingFlags.Instance);

        public static string GetName(this EventBuilder eventBuilder)
        {
            return (string)nameField.GetValue(eventBuilder);
        }
    }
}
