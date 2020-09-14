# Patterns
GameWorkstore basic pattern structures and project helpers
Use it your own risk!

# How to install

At package.json, add these line of code:
> "com.gameworkstore.patterns": "https://github.com/GameWorkstore/patterns.git"

and wait unity download and compile the package.

Is interesting to write a editor script to update them when necessary!

```csharp
using UnityEditor;
using UnityEditor.PackageManager;

public class PackageUpdater
{
    [MenuItem("Help/UpdateGitPackages")]
    public static void TrackPackages()
    {
        Client.Add("https://github.com/GameWorkstore/patterns.git");
    }
}
```