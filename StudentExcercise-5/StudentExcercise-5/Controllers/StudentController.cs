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

namespace StudentExcercise_5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
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

       

       /* [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName,s.SlackHandle,c.CohortName

                                        FROM Student s
                                        Left Join Cohort c on s.CohortId = c.id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle= reader.GetString(reader.GetOrdinal("SlackHandle")),
                             Cohort = new Cohort
                             {
                                 Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                 CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                             }
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }*/

        // GET: api/Students?q=joe&include=exercise

        [HttpGet]
        public IEnumerable<Student> Get(string include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "exercise")
                    {
                        cmd.CommandText = @"select s.id as StudentId,
                                               s.FirstName,
                                               s.LastName,
                                               s.SlackHandle,
                                               s.CohortId,
                                               c.CohortName,
                                               e.id as ExcerciseId,
                                               e.ExcerciseName,
                                               e.ExcerciseLanguage
                                          from student s
                                               left join Cohort c on s.CohortId = c.Id
                                               left join StudentExcercise se on s.Id = se.StudentId
                                               left join Excercise e on se.ExcerciseId = e.Id
                                         WHERE 1 = 1";
                    }
                    else
                    {
                        cmd.CommandText = @"select s.Id as StudentId,
                                               s.FirstName,
                                               s.LastName,
                                               s.SlackHandle,
                                               s.CohortId,
                                               c.CohortName
                                          from student s
                                               left join Cohort c on s.CohortId = c.Id
                                         WHERE 1 = 1";
                    }

                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.CommandText += @" AND 
                                             (s.FirstName LIKE @q OR
                                              s.LastName LIKE @q OR
                                              s.SlackHandle LIKE @q)";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Student> students = new Dictionary<int, Student>();
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                        if (!students.ContainsKey(studentId))
                        {
                            Student newStudent = new Student
                            {
                                Id = studentId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };

                            students.Add(studentId, newStudent);
                        }

                        if (include == "exercise")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                Student currentStudent = students[studentId];
                                currentStudent.Exercises.Add(
                                 new Excercise
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                        ExcerciseName = reader.GetString(reader.GetOrdinal("ExcerciseName")),
                                        ExcerciseLanguage= reader.GetString(reader.GetOrdinal("ExerciseLanguage")),
                                    }
                                );
                            }
                        }
                    }

                    reader.Close();

                    return students.Values.ToList();
                }
            }
        }



        /*[HttpGet("{Id}", Name = "GetExcercise")]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, ExcerciseName, ExcerciseLanguage
                        FROM 
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
        }*/
    }
}