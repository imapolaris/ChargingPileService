using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Cache
{
    public interface ICacheManager
    {
        Task LoadCache();
    }
}
