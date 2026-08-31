using Dapper;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.Data;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseTestController : ControllerBase
{
    private readonly DapperContext _context;

    public DatabaseTestController(DapperContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<int>(
                "SELECT 1");

            return Ok(new
            {
                message = "Database connection successful",
                result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Database connection failed",
                error = ex.Message
            });
        }
    }
}