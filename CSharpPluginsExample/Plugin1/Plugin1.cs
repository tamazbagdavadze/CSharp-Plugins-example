using System;
using PluginInterface;

namespace Plugin1
{
    public class Plugin1 : MarshalByRefObject, IMyPlugin
    {
        public string Execute(Func<string> f)
        {
            return f();
        }
    }
}
