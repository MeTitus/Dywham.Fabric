using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dywham.Fabric.Utils
{
    public static class AssemblyUtils
    {
        private const string DllSearchPattern = "*.dll";


        public static object ConvertDictionaryToObject(Dictionary<string, object> dictionary)
        {
            var expandoObject = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)expandoObject;

            foreach (var kvp in dictionary)
            {
                eoColl.Add(kvp);
            }

            dynamic eoDynamic = expandoObject;

            return eoDynamic;
        }

        public static Assembly[] GetAssembliesMatching(string path, Func<Assembly, bool> condition)
        {
            return GetAssembliesMatching(path, DllSearchPattern, condition);
        }

        public static Assembly[] GetAssembliesMatching(string path, string searchPattern, Func<Assembly, bool> condition)
        {
            var files = Directory.GetFiles(Path.GetFullPath(path), searchPattern, SearchOption.AllDirectories);

            var result = new HashSet<Assembly>();

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);

                    if (string.IsNullOrEmpty(fileName)) continue;

                    var assembly = Assembly.LoadFile(file);

                    if (condition(assembly))
                    {
                        result.Add(assembly);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return result.ToArray();
        }

        public static Assembly[] GetAssemblies(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var files = Directory.GetFiles(path, DllSearchPattern, SearchOption.AllDirectories).ToList();
            var assembliesTemp = new HashSet<Assembly>();

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);

                    if (string.IsNullOrEmpty(fileName)) continue;

                    assembliesTemp.Add(Assembly.LoadFrom(file));
                }
                catch (FileLoadException)
                { } // The Assembly has already been loaded.
                catch (BadImageFormatException)
                { } // If a BadImageFormatException exception is thrown, the file is not an assembly.
            }

            assembliesTemp.Add(Assembly.GetEntryAssembly());

            return assembliesTemp.ToArray();
        }

        public static IList<Type> GetTypesMatching(string defaultExecutionPath, Func<Type, bool> condition)
        {
            var types = new List<Type>();

            foreach (var assembly in GetAssemblies(defaultExecutionPath))
            {
                try
                {
                    var typesToAdd = assembly.GetTypes().Where(condition).ToList();

                    if (!typesToAdd.Any()) continue;

                    types.AddRange(typesToAdd);
                }
                catch
                {
                    // ignored
                }
            }

            return types;
        }

        public static IList<Type> GetTypesMatching(Assembly assembly, Func<Type, bool> condition)
        {
            return assembly.GetTypes().Where(condition).ToList();
        }

        public static Type[] GetAllTypesExceptSubtypesOf(this Assembly[] assemblies, Type typeToExclude)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes().Where(type => !typeToExclude.IsAssignableFrom(type)));
                }
                catch
                {
                    // ignored
                }
            }

            return types.ToArray();
        }

        public static IEnumerable<T> CreateInstancesOf<T>()
        {
            return CreateInstancesOf(typeof(T)).Select(x => (T)x);
        }

        public static IEnumerable<object> CreateInstancesOf(Type typeToFind)
        {
            var types = GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeToFind.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

            return types.Select(Activator.CreateInstance).ToList();
        }
    }
}