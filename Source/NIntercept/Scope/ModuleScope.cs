using NIntercept.Builder;
using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

namespace NIntercept
{
    public class ModuleScope
    {
        private const string DefaultAssemblyName = "NIntercept.DynamicAssembly";
        private const string DefaultModuleName = "NIntercept.DynamicModule";
        private const string Snk = "NIntercept.NIntercept.snk";
        private IInvocationTypeBuilder invocationTypeBuilder;
        private List<Type> invocationTypes;
        private List<Type> proxyTypes;
        private string assemblyName;
        private string moduleName;
        private AssemblyBuilder assembly;
        private ModuleBuilder module;
#if NET45 || NET472
        private bool saveAssembly;
        private bool strongName;
        private bool isSaved;

        public ModuleScope(string assemblyName, string moduleName, bool saveAssembly, bool strongName)
        {
            if (assemblyName is null)
                throw new ArgumentNullException(nameof(assemblyName));
            if (moduleName is null)
                throw new ArgumentNullException(nameof(moduleName));

            this.invocationTypeBuilder = new InvocationTypeBuilder();
            this.invocationTypes = new List<Type>();
            this.proxyTypes = new List<Type>();

            this.assemblyName = assemblyName;
            this.moduleName = moduleName;
            this.saveAssembly = saveAssembly;
            this.strongName = strongName;
        }

        public ModuleScope(bool saveAssembly, bool strongName)
           : this(DefaultAssemblyName, DefaultModuleName, saveAssembly, strongName)
        { }

        public ModuleScope(bool saveAssembly)
         : this(DefaultAssemblyName, DefaultModuleName, saveAssembly, false)
        { }

        public ModuleScope(string assemblyName, string moduleName)
            : this(assemblyName, moduleName, false, false)
        { }

#else
        public ModuleScope(string assemblyName, string moduleName)
        {
            if (assemblyName is null)
                throw new ArgumentNullException(nameof(assemblyName));
            if (moduleName is null)
                throw new ArgumentNullException(nameof(moduleName));

            this.invocationTypeBuilder = new InvocationTypeBuilder();
            this.invocationTypes = new List<Type>();
            this.proxyTypes = new List<Type>();

            this.assemblyName = assemblyName;
            this.moduleName = moduleName;
        }
#endif
        public ModuleScope()
            : this(DefaultAssemblyName, DefaultModuleName)
        { }


        public string AssemblyName
        {
            get { return assemblyName; }
        }

        public string ModuleName
        {
            get { return moduleName; }
        }

        public AssemblyBuilder Assembly
        {
            get
            {
                if (assembly == null)
                    assembly = DefineAssembly(assemblyName);
                return assembly;
            }
        }

        public ModuleBuilder Module
        {
            get
            {
                if (module == null)
                    module = DefineModule(moduleName);
                return module;
            }
        }

        public IReadOnlyList<Type> ProxyTypes
        {
            get { return proxyTypes; }
        }

        public IReadOnlyList<Type> InvocationTypes
        {
            get { return invocationTypes; }
        }

#if NET45 || NET472

        public bool SaveAssembly
        {
            get { return saveAssembly; }
        }

        public bool StrongName
        {
            get { return strongName; }
        }

        public virtual string FileName
        {
            get { return $"{moduleName}.dll"; }
        }

        public bool IsSaved
        {
            get { return isSaved; }
        }

        protected AssemblyBuilder DefineAssembly(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            if (strongName)
                name.KeyPair = new StrongNameKeyPair(GetStrongNameKeyPair());

            AssemblyBuilderAccess access = saveAssembly ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            return AssemblyBuilder.DefineDynamicAssembly(name, access);
        }

        protected byte[] GetStrongNameKeyPair()
        {
            using (var stream = typeof(ModuleScope).Assembly.GetManifestResourceStream(Snk))
            {
                if (stream == null)
                    throw new MissingManifestResourceException($"{Snk} resource not found");

                var length = (int)stream.Length;
                var keyPair = new byte[length];
                stream.Read(keyPair, 0, length);
                return keyPair;
            }
        }

        protected ModuleBuilder DefineModule(string moduleName)
        {
            if (saveAssembly)
                return Assembly.DefineDynamicModule(moduleName, FileName);

            return Assembly.DefineDynamicModule(moduleName);
        }

        public virtual void Save()
        {
            if (!saveAssembly)
                return;

            Assembly.Save(FileName);
            isSaved = true;
        }
#else
        protected AssemblyBuilder DefineAssembly(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
            return AssemblyBuilder.DefineDynamicAssembly(name, access);
        }

        protected ModuleBuilder DefineModule(string moduleName)
        {
            return Assembly.DefineDynamicModule(moduleName);
        }
#endif

        public Type GetOrCreateProxyType(ProxyTypeDefinition typeDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));

            string name = typeDefinition.Name;
            Type type = proxyTypes.FirstOrDefault(p => p.Name == name);
            if (type == null)
            {
                TypeBuilder typeBuilder = Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);
                ProxyScope proxyScope = new ProxyScope(this, typeBuilder, typeDefinition);
                proxyScope.DefineTypeAndMembers();
                type = typeBuilder.BuildType();
                proxyTypes.Add(type);
            }
            return type;
        }

        public Type GetOrCreateInvocationType(InvocationTypeDefinition invocationTypeDefinition, MethodBuilder callbackMethodBuilder)
        {
            string name = invocationTypeDefinition.Name;
            Type type = invocationTypes.FirstOrDefault(p => p.Name == name);
            if (type == null)
            {
                type = invocationTypeBuilder.CreateType(this, invocationTypeDefinition, callbackMethodBuilder);
                invocationTypes.Add(type);
            }
            return type;
        }
    }

}
