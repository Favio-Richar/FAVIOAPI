using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("Reserva")]
public class ReservaController : ControllerBase
{
    private readonly string StringConector; // Cadena de conexión a la base de datos

    // Constructor que recibe la configuración para obtener la cadena de conexión
    public ReservaController(IConfiguration config)
    {
        StringConector = config.GetConnectionString("MySqlConnection");
    }

    // Endpoint para obtener todas las reservas
    [HttpGet]
    public async Task<IActionResult> ListarReservas()
    {
        try
        {
            // Establecer conexión con la base de datos
            using (MySqlConnection conecta = new MySqlConnection(StringConector))
            {
                await conecta.OpenAsync(); // Abrir conexión

                string sentencia = "SELECT * FROM RESERVA"; // Consulta SQL para obtener todas las reservas
                List<Reserva> reservas = new List<Reserva>(); // Lista para almacenar las reservas obtenidas

                // Ejecutar consulta para obtener datos
                using (MySqlCommand comandos = new MySqlCommand(sentencia, conecta))
                {
                    using (var lector = await comandos.ExecuteReaderAsync())
                    {
                        // Leer cada fila obtenida de la base de datos y crear objetos Reserva
                        while (await lector.ReadAsync())
                        {
                            reservas.Add(new Reserva
                            {
                                idReserva = lector.GetInt32(0),
                                Especialidad = lector.GetString(1),
                                DiaReserva = lector.GetDateTime(2)
                            });
                        }
                    }
                }

                return StatusCode(200, reservas); // Devolver la lista de reservas obtenidas
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener la lista de reservas: " + ex.Message); // Manejo de errores
        }
    }

    // Endpoint para obtener una reserva por su ID
    [HttpGet("{idReserva}")]
    public async Task<IActionResult> ObtenerReserva(int idReserva)
    {
        try
        {
            // Establecer conexión con la base de datos
            using (MySqlConnection conectar = new MySqlConnection(StringConector))
            {
                await conectar.OpenAsync(); // Abrir conexión

                string sentencia = "SELECT * FROM RESERVA WHERE idReserva = @idReserva"; // Consulta SQL para obtener una reserva específica
                Reserva reserva = new Reserva(); // Objeto Reserva para almacenar la reserva obtenida

                // Ejecutar consulta para obtener datos por ID
                using (MySqlCommand comandos = new MySqlCommand(sentencia, conectar))
                {
                    comandos.Parameters.AddWithValue("@idReserva", idReserva); // Parámetro para filtrar por ID

                    using (var lector = await comandos.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            // Llenar el objeto Reserva con los datos obtenidos
                            reserva.idReserva = lector.GetInt32(0);
                            reserva.Especialidad = lector.GetString(1);
                            reserva.DiaReserva = lector.GetDateTime(2);

                            return StatusCode(200, reserva); // Devolver la reserva encontrada
                        }
                        else
                        {
                            return StatusCode(404, "No se encontró la reserva con el ID especificado"); // Si no se encuentra, devuelve un mensaje de error
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener la reserva: " + ex.Message); // Manejo de errores
        }
    }

    // Otros métodos como CrearReserva, EditarReserva, EliminarReserva, pueden ser implementados de manera similar.
}
