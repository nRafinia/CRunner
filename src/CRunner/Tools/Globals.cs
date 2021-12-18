using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace CRunner.Tools;

public static class Globals
{
    public static IEnumerable<Type> GetImplementedInterfaceOf<T>()
    {
        return GetImplementedInterfaceOf(typeof(T));
    }

    public static IEnumerable<Type> GetImplementedInterfaceOf(Type type)
    {
        var platform = Environment.OSVersion.Platform.ToString();
        var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

        var res = new List<Type>();
        foreach (var assembly in runtimeAssemblyNames)
        {
            try
            {
                var items = Assembly.Load(assembly.FullName)
                    .GetTypes()
                    .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);

                if (items.Any())
                {
                    res.AddRange(items);
                }
            }
            catch
            {
                //
            }
        }

        return !res.Any()
            ? new List<Type>()
            : res
                .GroupBy(a => a.Assembly)
                .First();
    }

}

