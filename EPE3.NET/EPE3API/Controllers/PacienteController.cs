using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("Paciente")]
public class PacienteController : ControllerBase
{
    private readonly string StringConector;

    // Constructor que recibe la configuración para obtener la cadena de conexión
    public PacienteController(IConfiguration config)
    {
        StringConector = config.GetConnectionString("MySqlConnection");
    }

    // Endpoint para obtener todos los pacientes
    [HttpGet]
    public async Task<IActionResult> ListarPacientes()
    {
        try
        {
            using (MySqlConnection conecta = new MySqlConnection(StringConector))
            {
                await conecta.OpenAsync();

                string sentencia = "SELECT * FROM PACIENTE";
                List<Paciente> pacientes = new List<Paciente>();

                using (MySqlCommand comandos = new MySqlCommand(sentencia, conecta))
                {
                    using (var lector = await comandos.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            // Creación de objetos Paciente a partir de los datos obtenidos de la base de datos
                            pacientes.Add(new Paciente
                            {
                                idPaciente = lector.GetInt32(0),
                                NombrePac = lector.GetString(1),
                                ApellidoPac = lector.GetString(2),
                                RunPac = lector.GetString(3),
                                Nacionalidad = lector.GetString(4),
                                Visa = lector.GetString(5),
                                Genero = lector.GetString(6),
                                SintomasPac = lector.GetString(7)
                            });
                        }
                    }
                }

                return StatusCode(200, pacientes); // Retorna la lista de pacientes
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener la lista de pacientes: " + ex.Message); // Manejo de errores
        }
    }

    // Endpoint para obtener un paciente por su ID
    [HttpGet("{idPaciente}")]
    public async Task<IActionResult> ObtenerPaciente(int idPaciente)
    {
        try
        {
            using (MySqlConnection conectar = new MySqlConnection(StringConector))
            {
                await conectar.OpenAsync();
                string sentencia = "SELECT * FROM PACIENTE WHERE idPaciente = @idPaciente";
                Paciente paciente = new Paciente();

                using (MySqlCommand comandos = new MySqlCommand(sentencia, conectar))
                {
                    comandos.Parameters.AddWithValue("@idPaciente", idPaciente);

                    using (var lector = await comandos.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            // Creación de un objeto Paciente a partir de los datos obtenidos por ID
                            paciente.idPaciente = lector.GetInt32(0);
                            paciente.NombrePac = lector.GetString(1);
                            paciente.ApellidoPac = lector.GetString(2);
                            paciente.RunPac = lector.GetString(3);
                            paciente.Nacionalidad = lector.GetString(4);
                            paciente.Visa = lector.GetString(5);
                            paciente.Genero = lector.GetString(6);
                            paciente.SintomasPac = lector.GetString(7);

                            return StatusCode(200, paciente); // Retorna el paciente encontrado
                        }
                        else
                        {
                            return StatusCode(404, "No se encontró el paciente con el ID especificado"); // Si no se encuentra, devuelve un mensaje de error
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el paciente: " + ex.Message); // Manejo de errores
        }
    }

    // Endpoint para crear un nuevo paciente
    [HttpPost]
    public async Task<IActionResult> CrearPaciente([FromBody] Paciente paciente)
    {
        try
        {
            using (MySqlConnection conectar = new MySqlConnection(StringConector))
            {
                await conectar.OpenAsync();
                
                string sentencia = "INSERT INTO PACIENTE (NombrePac, ApellidoPac, RunPac, Nacionalidad, Visa, Genero, SintomasPac) VALUES (@NombrePac, @ApellidoPac, @RunPac, @Nacionalidad, @Visa, @Genero, @SintomasPac)";
                MySqlCommand comandos = new MySqlCommand(sentencia, conectar);

                // Asignación de parámetros para la inserción del nuevo paciente
                comandos.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                comandos.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                comandos.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                comandos.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                comandos.Parameters.AddWithValue("@Visa", paciente.Visa);
                comandos.Parameters.AddWithValue("@Genero", paciente.Genero);
                comandos.Parameters.AddWithValue("@SintomasPac", paciente.SintomasPac);

                await comandos.ExecuteNonQueryAsync(); // Ejecución de la inserción

                return StatusCode(201, "Paciente creado correctamente"); // Retorna un mensaje de éxito
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al crear el paciente: " + ex.Message); // Manejo de errores
        }
    }

    // Endpoint para editar un paciente existente
    [HttpPut("{idPaciente}")]
    public async Task<IActionResult> EditarPaciente(int idPaciente, [FromBody] Paciente paciente)
    {
        try
        {
            using (MySqlConnection conectar = new MySqlConnection(StringConector))
            {
                await conectar.OpenAsync();
                
                string sentencia = "UPDATE PACIENTE SET NombrePac = @NombrePac, ApellidoPac = @ApellidoPac, RunPac = @RunPac, Nacionalidad = @Nacionalidad, Visa = @Visa, Genero = @Genero, SintomasPac = @SintomasPac WHERE idPaciente = @idPaciente";
                MySqlCommand comandos = new MySqlCommand(sentencia, conectar);

                // Asignación de parámetros para la actualización del paciente
                comandos.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                comandos.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                comandos.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                comandos.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                comandos.Parameters.AddWithValue("@Visa", paciente.Visa);
                comandos.Parameters.AddWithValue("@Genero", paciente.Genero);
                comandos.Parameters.AddWithValue("@SintomasPac", paciente.SintomasPac);
                comandos.Parameters.AddWithValue("@idPaciente", idPaciente);

                int filasAfectadas = await comandos.ExecuteNonQueryAsync();

                if (filasAfectadas > 0)
                {
                    return StatusCode(200, "Registro editado correctamente"); // Si se edita con éxito, retorna un mensaje de éxito
                }
                else
                {
                    return StatusCode(404, "No se encontró el paciente con el ID especificado"); // Si no se encuentra, devuelve un mensaje de error
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al editar el paciente: " + ex.Message); // Manejo de errores
        }
    }

    // Endpoint para eliminar un paciente por su ID
    [HttpDelete("{idPaciente}")]
    public async Task<IActionResult> EliminarPaciente(int idPaciente)
    {
        try
        {
            using (MySqlConnection conectar = new MySqlConnection(StringConector))
            {
                await conectar.OpenAsync();
                
                string sentencia = "DELETE FROM PACIENTE WHERE idPaciente = @idPaciente";
                MySqlCommand comandos = new MySqlCommand(sentencia, conectar);

                comandos.Parameters.AddWithValue("@idPaciente", idPaciente);

                int filasAfectadas = await comandos.ExecuteNonQueryAsync();

                if (filasAfectadas > 0)
                {
                    return StatusCode(200, "Paciente eliminado correctamente"); // Si se elimina con éxito, retorna un mensaje de éxito
                }
                else
                {
                    return StatusCode(404, "No se encontró el paciente con el ID especificado"); // Si no se encuentra, devuelve un mensaje de error
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al eliminar el paciente: " + ex.Message); // Manejo de errores
        }
    }
}
