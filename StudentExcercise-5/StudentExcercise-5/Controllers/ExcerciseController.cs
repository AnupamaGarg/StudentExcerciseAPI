using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExcercise_5.Models;
using Microsoft.AspNetCore.Http;

namespace Stu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcerciseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExcerciseController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        //Create ExercisesController
        //Code for getting a list of exercises
        //Code for getting a single exercise
        //Code for creating an exercise
        //Code for editing an exercise
        //Code for deleting an exercise

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ExcerciseName, ExcerciseLanguage FROM Excercise";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Excercise> excercises = new List<Excercise>();

                    while (reader.Read())
                    {
                        Excercise excercise = new Excercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExcerciseName = reader.GetString(reader.GetOrdinal("ExcerciseName")),
                            ExcerciseLanguage = reader.GetString(reader.GetOrdinal("ExcerciseLanguage"))
                        };

                        excercises.Add(excercise);
                    }
                    reader.Close();

                    return Ok(excercises);
                }
            }
        }


        [HttpGet("{Id}", Name = "GetExcercise")]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExcerciseName, ExcerciseLanguage
                        FROM Excercise
                        WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", Id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Excercise excercise = null;

                    if (reader.Read())
                    {
                        excercise = new Excercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExcerciseName = reader.GetString(reader.GetOrdinal("ExcerciseName")),
                            ExcerciseLanguage = reader.GetString(reader.GetOrdinal("ExcerciseLanguage"))
                        };
                    }
                    reader.Close();

                    return Ok(excercise);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Excercise excercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Excercise (ExcerciseName, ExcerciseLanguage)
                                        OUTPUT INSERTED.Id
                                        VALUES (@ExcerciseName, @ExcerciseLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@ExcerciseName", excercise.ExcerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ExcerciseLanguage", excercise.ExcerciseLanguage));

                    int newId = (int)cmd.ExecuteScalar();
                    excercise.Id = newId;
                    return CreatedAtRoute("GetExcercise", new { Id = newId }, excercise);
                }
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int Id, [FromBody] Excercise excercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Excercise
                                            SET ExcerciseName = @ExcerciseName,
                                                ExcerciseLanguage = @ExcerciseLanguage
                                            WHERE Id = @Id";
                        cmd.Parameters.Add(new SqlParameter("@ExcerciseName", excercise.ExcerciseName));
                        cmd.Parameters.Add(new SqlParameter("@ExcerciseLanguage", excercise.ExcerciseLanguage));
                        cmd.Parameters.Add(new SqlParameter("@Id", Id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExcerciseExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Excercise WHERE Id = @Id";
                        cmd.Parameters.Add(new SqlParameter("@id", Id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExcerciseExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }



        }
        private bool ExcerciseExists(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ExcerciseName, ExcerciseLanguage
                        FROM Excercise
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}