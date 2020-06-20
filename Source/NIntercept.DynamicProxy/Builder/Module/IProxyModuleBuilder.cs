using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyModuleBuilder
    {
        AssemblyBuilder Assembly { get; }
        string AssemblyName { get; }
        ModuleBuilder Module { get; }
        string ModuleName { get; }
#if NET45 || NET472
        string FileName { get; }
        bool IsSaved { get; }
        bool SaveAssembly { get; }
        bool StrongName { get; }

        void Save();
#endif
    }
}
