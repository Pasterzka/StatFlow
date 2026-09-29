using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.User;
using User_service.Data;
using User_service.Models;
using Microsoft.EntityFrameworkCore;
using Shared.User.Response;

namespace User_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UserController> _logger;


        public UserController(UserDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // api/User/add -> dodanie uzytkownika do bazy danych
        [HttpPost("add")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // walidacja

            // posty request
            if (request == null) {
                _logger.LogInformation("[ERROR] Pusty request zwracany BadRequest!");
                return BadRequest($"[ERROR] Pusty request {request}");
            }

            // TODO: Walidacja MAIL i TELEFON

            // czy mail istnieje w bazie danych (email lub telefon)
            var badUser = _context.Users
                                .Where(u => (u.email == request.email) || (u.phoneNumber == request.phoneNumber))
                                .FirstOrDefaultAsync();

            if(badUser != null)
            {
                _logger.LogInformation("[ERROR] Uzytkownik o podanym emailu istnieje w bazie danych. Zwracam BadRequest!");
                return BadRequest($"[ERROR] Uzytkownik o mailu {request.email} istnieje w bazie danych");
            }


            // dodanie uzytkownika do bazy danych
            var user = new User
            {
                UserId = Guid.NewGuid(),
                firstName = request.firstName,
                lastName = request.lastName,
                email = request.email,
                password = request.password,
                address = request.addres,
                phoneNumber = request.phoneNumber,
                country = request.country
            };

            _logger.LogInformation($"[SUCCESS] Dodano uzytkownika o Nawisku {user.lastName} z ID {user.UserId}!");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // api/User/{userId} -> zwracanie informacji o danym uzytkowniku
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            // znalezienie uzytkownika o podanym id
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new GetUserResponse(
                    u.UserId,
                    u.firstName,
                    u.lastName,
                    u.email
                ))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogInformation("[ERROR] Brak uzytkownika o podnanym ID!");
                return BadRequest("[ERROR] Brak uzytkownika o podnanym ID!");
            }
            

            _logger.LogInformation($"[SUCCES] Znaleziono uzytkownika o id: {userId}!");
            return Ok(user);
        }

        // api/User/all -> pobranie wszystkich uzytkowników z bazy danych
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _context.Users;

            _logger.LogInformation("[SUCCESS] Pobrano wszystkich uzytkowników z bazy danych!");
            return Ok(users);
        }

        // api/User/pagination - zwraca posortowanych według dowolnego argumentu daną liczbę uytkowników (zestronnicowaną)
        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaginationUsers(
            [FromQuery] string sortBy,
            [FromQuery] bool isDescending,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            // stworzenie zapytania
            var query = _context.Users.AsNoTracking();

            // dodanie sortowania na podstawie wybranych parametrów
            query = sortBy switch
            {
                "userId" => isDescending ? query.OrderByDescending(u => u.UserId) : query.OrderBy(u => u.UserId),
                "firstName" => isDescending ?query.OrderByDescending(u => u.firstName) : query.OrderBy(u => u.firstName),
                "lastName" => isDescending ?query.OrderByDescending(u => u.lastName) : query.OrderBy(u => u.lastName),
                "email" => isDescending ?query.OrderByDescending(u => u.email) : query.OrderBy(u => u.email),
                "phoneNumber" => isDescending ?query.OrderByDescending(u => u.phoneNumber) : query.OrderBy(u => u.phoneNumber),
                "country" => isDescending ?query.OrderByDescending(u => u.country) : query.OrderBy(u => u.country),
                _ => query.OrderBy(u => u.firstName),
            };

            // pobranie wybranych uytkowników
            var users = await query
                .Skip((pageNumber-1) * pageSize)
                .Take(pageSize)
                .Select(u => new GetUserResponse(
                    u.UserId, 
                    u.firstName,
                    u.lastName,
                    u.email
                ))
                .ToListAsync();

            _logger.LogInformation("[SUCCESS] Pobrano uzytkownikow!");
            return Ok(users);
        }
    }
}
