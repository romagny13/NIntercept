using System.Reflection;

namespace NIntercept
{
    internal class StaticMemberCache : ReflectionCache
    {
        public override BindingFlags Flags
        {
            get { return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }
    }
}
