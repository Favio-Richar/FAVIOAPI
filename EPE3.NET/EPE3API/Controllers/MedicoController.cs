using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Controlador para gestionar operaciones relacionadas con los médicos
[ApiController]
[Route("Medico")]
public class MedicoController : ControllerBase
{
    private readonly string StringConector; // Cadena de conexión a la base de datos

    // Constructor que recibe la configuración para la cadena de conexión
    public MedicoController(IConfiguration config)
    {
        StringConector = config.GetConnectionString("MySqlConnection"); // Obtiene la cadena de conexión de la configuración
    }

    // Método para obtener la lista de todos los médicos
    [HttpGet]
    public async Task<IActionResult> ListarMedicos()
    {
        try
        {
            // Establece la conexión con la base de datos
            using (MySqlConnection conecta = new MySqlConnection(StringConector))
            {
                await conecta.OpenAsync(); // Abre la conexión

                string sentencia = "SELECT * FROM MEDICO"; // Sentencia SQL para seleccionar todos los médicos
                List<Medico> medicos = new List<Medico>(); // Lista para almacenar los médicos

                // Ejecuta la consulta para obtener los médicos
                using (MySqlCommand comandos = new MySqlCommand(sentencia, conecta))
                {
                    // Lee los resultados de la consulta
                    using (var lector = await comandos.ExecuteReaderAsync())
                    {
                        // Recorre los resultados y agrega cada médico a la lista
                        while (await lector.ReadAsync())
                        {
                            medicos.Add(new Medico
                            {
                                // Asigna los valores obtenidos de la consulta al objeto Medico
                                idMedico = lector.GetInt32(0),
                                NombreMed = lector.GetString(1),
                                ApellidoMed = lector.GetString(2),
                                RunMed = lector.GetString(3),
                                Eunacom = lector.GetString(4),
                                NacionalidadMed = lector.GetString(5),
                                Especialidad = lector.GetString(6),
                                Horarios = lector.GetString(7),
                                TarifaHr = lector.GetInt32(8)
                            });
                        }
                    }
                }

                // Retorna la lista de médicos obtenida de la base de datos
                return StatusCode(200, medicos);
            }
        }
        catch (Exception ex)
        {
            // En caso de error, devuelve un código de error con un mensaje descriptivo
            return StatusCode(500, "Error al obtener la lista de médicos: " + ex.Message);
        }
    }

    // Métodos para obtener, crear, editar y eliminar médicos con comentarios similares a los anteriores...
}
