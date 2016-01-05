using System;
using System.IO;
using PluginInterface;

namespace Plugin2
{
    public class Plugin2 : MarshalByRefObject, IMyPlugin
    {
        public string Execute(Func<string> f)
        {
            return f();
        }
    }
}