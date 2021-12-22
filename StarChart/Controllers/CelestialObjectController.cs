using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                CelestialObject CelObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
                if (CelObject == null) return NotFound();

                CelObject.Satellites = _context.CelestialObjects
                                        .Where(i => i.OrbitedObjectId == CelObject.Id)
                                        .ToList<CelestialObject>();
                return Ok(CelObject);
            }
            catch (Exception)
            {
                return BadRequest("Unable to process request");
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                List<CelestialObject> CelObject = _context.CelestialObjects
                        .Where(i => i.Name == name)
                        .ToList<CelestialObject>();
                 if (CelObject.Count() == 0) return NotFound();

                foreach (var CelestialObject in CelObject)
                {
                    CelestialObject.Satellites = _context.CelestialObjects
                        .Where(i => i.OrbitedObjectId == CelestialObject.Id)
                        .ToList<CelestialObject>();
                }

                return Ok(CelObject);
            }
            catch (Exception)
            {
                return BadRequest("Unable to process request");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<CelestialObject> objectList = _context.CelestialObjects.ToList<CelestialObject>();

                foreach (var CelestialObject in objectList)
                {
                    CelestialObject.Satellites = _context.CelestialObjects
                        .Where(i => i.OrbitedObjectId == CelestialObject.Id)
                        .ToList<CelestialObject>();
                }

                return Ok(objectList);
            }
            catch (Exception)
            {
                return BadRequest("Unable to process request");
            }
        }

        [HttpPost]        
        public IActionResult Create([FromBody]CelestialObject cObject)
        {
            try
            {
                _context.CelestialObjects.Add(cObject);
                _context.SaveChanges();

                CelestialObject returnObject = new CelestialObject
                {
                    Id = cObject.Id,
                    Name = cObject.Name,
                    Satellites = cObject.Satellites,
                    OrbitalPeriod = cObject.OrbitalPeriod,
                    OrbitedObjectId = cObject.OrbitedObjectId,

                };
                return CreatedAtRoute($"GetById/{cObject.Id}", returnObject);
            }
            catch
            {
                return BadRequest("Unable to Add Object");
            }
        }
    }
}
