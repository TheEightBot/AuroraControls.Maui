using System;
using System.Reflection;

namespace AuroraControls;

/// <summary>
/// Embedded resource loader.
/// </summary>
public static class EmbeddedResourceLoader
{
    /// <summary>
    /// A list of key value pairs of assemblies and resource names.
    /// </summary>
    private static readonly List<KeyValuePair<Assembly, string[]>> _assemblies = new List<KeyValuePair<Assembly, string[]>>();

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    public static void LoadAssembly(Assembly assembly)
    {
        if (_assemblies.Any(kvp => kvp.Key == assembly))
        {
            return;
        }

        _assemblies.Add(new KeyValuePair<Assembly, string[]>(assembly, assembly.GetManifestResourceNames()));
    }

    /// <summary>
    /// Load the specified resource name.
    /// </summary>
    /// <returns>The resource as a stream.</returns>
    /// <param name="name">Takes a string representing the name of the embeded resource.</param>
    public static Stream Load(string name)
    {
        Stream stream = null;

        var formattedName = $".{name}";

        foreach (var kvp in _assemblies)
        {
            var foundResource = kvp.Value.FirstOrDefault(n => n.Equals(formattedName, StringComparison.OrdinalIgnoreCase) || n.EndsWith(formattedName, StringComparison.OrdinalIgnoreCase));
            if (foundResource is not null)
            {
                stream = kvp.Key.GetManifestResourceStream(foundResource);

                if (stream is not null)
                {
                    break;
                }
            }
        }

        return stream ?? Stream.Null;
    }
}
