using System;

namespace PluginInterface
{
    public interface IMyPlugin
    {
       string Execute(Func<string> f);
    }
}
