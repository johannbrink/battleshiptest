using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Battleship.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        public GameController()
        {
        }
    }
}