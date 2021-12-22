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
                CelestialObject celestialObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
                if (celestialObject == null) return NotFound();

                celestialObject.Satellites = _context.CelestialObjects
                                        .Where(i => i.OrbitedObjectId == celestialObject.Id)
                                        .ToList<CelestialObject>();
                return Ok(celestialObject);
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
                List<CelestialObject> celestialObject = _context.CelestialObjects
                        .Where(i => i.Name == name)
                        .ToList<CelestialObject>();
                 if (celestialObject.Count() == 0) return NotFound();

                foreach (var CelestialObject in celestialObject)
                {
                    CelestialObject.Satellites = _context.CelestialObjects
                        .Where(i => i.OrbitedObjectId == CelestialObject.Id)
                        .ToList<CelestialObject>();
                }

                return Ok(celestialObject);
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
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            try
            {
                _context.CelestialObjects.Add(celestialObject);
                _context.SaveChanges();

                CelestialObject returnObject = new CelestialObject
                {
                    Id = celestialObject.Id,
                    Name = celestialObject.Name,
                    Satellites = celestialObject.Satellites,
                    OrbitalPeriod = celestialObject.OrbitalPeriod,
                    OrbitedObjectId = celestialObject.OrbitedObjectId,

                };
                return CreatedAtRoute($"GetById",new { id = returnObject.Id }, returnObject);
            }
            catch
            {
                return BadRequest("Unable to Add Object");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject celestialObject)
        {
            try
            {
                CelestialObject oldObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
                if (oldObject == null) return NotFound();

                oldObject.Name = celestialObject.Name;
                oldObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                _context.CelestialObjects.Update(oldObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("Unable to update Record.");
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            try
            {
                CelestialObject celestialObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
                if (celestialObject == null) return NotFound();

                celestialObject.Name = name;
                _context.CelestialObjects.Update(celestialObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("Unable to rename Record.");
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                List<CelestialObject> celestialObject = _context.CelestialObjects
                        .Where(i => i.Id == id)
                        .Where(i => i.OrbitedObjectId == id)
                        .ToList<CelestialObject>();
                if (celestialObject.Count() == 0) return NotFound();
                _context.CelestialObjects.RemoveRange(celestialObject);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return BadRequest("Unable to Delete Record");
            }
            return NoContent();
        }
    }
}
