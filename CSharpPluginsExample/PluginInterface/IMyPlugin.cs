using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterface
{
    public interface IMyPlugin
    {
       string Execute(Func<string> f);
    }
}
