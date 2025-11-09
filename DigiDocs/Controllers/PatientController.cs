using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DigiDocs.Models;

namespace DigiDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(DigidocsContext _context) : ControllerBase
    { 
        
    }
}
