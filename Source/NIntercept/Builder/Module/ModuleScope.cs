using System;
using System.Collections;
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
        private string assemblyName;
        private string moduleName;
        private AssemblyBuilder assembly;
        private ModuleBuilder module;
        private InvocationTypeRegistry invocationTypeRegistry;
        private ProxyTypeRegistry proxyTypeRegistry;


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

            this.invocationTypeRegistry = new InvocationTypeRegistry();
            this.proxyTypeRegistry = new ProxyTypeRegistry();
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

            this.invocationTypeRegistry = new InvocationTypeRegistry();
            this.proxyTypeRegistry = new ProxyTypeRegistry();
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

        internal InvocationTypeRegistry InvocationTypeRegistry
        {
            get { return invocationTypeRegistry; }
        }

        internal ProxyTypeRegistry ProxyTypeRegistry
        {
            get { return proxyTypeRegistry; }
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
        protected virtual AssemblyBuilder DefineAssembly(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
            return AssemblyBuilder.DefineDynamicAssembly(name, access);
        }

        protected virtual ModuleBuilder DefineModule(string moduleName)
        {
            return Assembly.DefineDynamicModule(moduleName);
        }
#endif       

    }

}
