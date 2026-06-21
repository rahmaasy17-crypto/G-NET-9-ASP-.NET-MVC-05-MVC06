using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Common
{
    public record Result(bool success,string? error=null,Resultkind kind =Resultkind.Ok)
    {
    }
}
