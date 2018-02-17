using System;
using System.IO;
using System.Reflection;

namespace TypeIsolator
{
    public class Isolator : IDisposable
    {
        private readonly AppDomain _appDomain;
        private readonly IsolatorRunner _runner;

        public Isolator(Type type, string methodName)
        {
            var runnerType = typeof(IsolatorRunner);
            var runnerAssemblyPath = runnerType.Assembly.Location;
            var runnerAssemblyContents = File.ReadAllBytes(runnerAssemblyPath);
            var runnerAssemblyFolder = Path.GetDirectoryName(runnerAssemblyPath);

            _appDomain = AppDomain.CreateDomain($"Foo_Isolated_AppDomain_{Guid.NewGuid()}", null, new AppDomainSetup()
            {
                ApplicationBase = runnerAssemblyFolder
            });

            var cTorArgs = new object[] { runnerAssemblyContents, type.FullName, methodName };
            _runner = (IsolatorRunner)_appDomain.CreateInstanceFromAndUnwrap(runnerAssemblyPath, runnerType.FullName, false
                , BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance
                , null, cTorArgs, null, null);
        }

        public void Run()
        {
            _runner.Run();
        }

        public void Dispose()
        {
            if(_appDomain != null)
                AppDomain.Unload(_appDomain);
        }
    }

    internal class IsolatorRunner : MarshalByRefObject
    {
        private readonly object _instance;
        private readonly MethodInfo _method;

        public IsolatorRunner(byte[] assemblyContents, string typeName, string methodName)
        {
            var assembly = AppDomain.CurrentDomain.Load(assemblyContents);
            var type = assembly.GetType(typeName);
            var cTor = type.GetConstructor(Type.EmptyTypes);
            _instance = cTor.Invoke(new object[] { });
            _method = type.GetMethod(methodName);
        }

        public void Run()
        {
            _method.Invoke(_instance, null);
        }
    }
}