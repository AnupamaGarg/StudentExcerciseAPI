﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace StudentExcercise_5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        // GET: api/Instructor
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Instructor/5
        [HttpGet("{id}", Name = "GetInstructor")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Instructor
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
