using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigiDocs.Models;
using DigiDocs.dtos;
using DigiDocs.Enums;
using System.Collections.Generic;
using Mapster;

namespace DigiDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly DigidocsContext _context;

        public PatientController(DigidocsContext context)
        {
            _context = context;
        }

        // POST: api/patient
        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto.userId is null)
                return Unauthorized(new { message = "userId is required" });

            var patient = new PatientDatum
            {
                Pname = dto.Pname,
                Page = dto.Page,
                Pjob = dto.Pjob,
                Pgender = dto.Pgender,
                Paddress = dto.Paddress,
                Pcomplain = dto.Pcomplain,
                PchronicDisease = dto.PchronicDisease,
                PserviceType = dto.PserviceType,
                Ppriority = dto.Ppriority,
                PamountToPay = dto.PamountToPay,
                PphoneNumber = dto.PphoneNumber,


                #region Audit Fields

                CreatedById = dto.userId,
                LastModifiedById = dto.userId,
                // CreatedDateTime and LastModifiedDateTime have DB defaults, but set them to UTC now for immediate availability
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow

                #endregion
            };

            _context.PatientData.Add(patient);
            _context.SaveChanges();

            _context.PatientQueues.Add(new PatientQueue { PatientId = patient.PatientId, Status = QueueStatus.Waiting });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, new { message = "Patient created successfully", patientId = patient.PatientId });
        }

        // GET: api/patient/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _context.PatientData.FindAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpGet("GetQueue")]
        public async Task<IActionResult> GetQueue()
        {
            List<PatientQueue> queue = await _context.PatientQueues
                .Include(pq => pq.Patient)
                .ToListAsync();

            List<PatientQueueResponseDto> response = queue.Adapt<List<PatientQueueResponseDto>>();
            
            return Ok(response);
        }

        [HttpPost("StartExamination")]
        public async Task<IActionResult> StartExamination([FromQuery]int id)
        {
            PatientQueue? queue = await _context.PatientQueues.FindAsync(id); 
            if (queue == null) return NotFound();

            queue.Status = QueueStatus.InProgress;

            await _context.SaveChangesAsync(); 

            return Ok(new { message = "Examination started successfully" });

        }
    }
}
