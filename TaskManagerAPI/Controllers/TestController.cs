using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // toma la URL base
        [HttpGet]
        public string Get() 
        {
            return "Api funcionando"; 
        }
        /* POST - METODO SIMPLE
        [HttpPost]
        public string Post()
        {
            return "Post funcionando";
        }
        */

        // POST - SUMATORIA TIPO INT
        [HttpPost]
        public int Post(int num1, int num2)
        {
            int suma = num1 + num2;
            return suma;
        }

        /* POST - SUMATORIA TIPO STRING
        [HttpPost]
        public string Post(int num1, int num2)
        {
            int suma = num1 + num2;
            return $"el resultado es {suma}";
        }
        */

    }
}
