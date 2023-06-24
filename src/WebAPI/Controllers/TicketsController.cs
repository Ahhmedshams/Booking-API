using AutoMapper;
using Domain.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
   
    public class TicketsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _Mapper;
        public TicketsController(UserManager<ApplicationUser> userManager , ApplicationDbContext context,IMapper Map)
        {
            this._userManager = userManager;
            this._context = context;
            this._Mapper = Map;
        }

        [HttpPost("Ticket")]
        public async Task<IActionResult> AddTicket(TicketDTO ticketDTO)
        {  
            var user = await _userManager.FindByIdAsync(ticketDTO.ApplicationUserId);
            if (user == null)
                return NotFound("User Not Found");
            var Data =  _Mapper.Map<Ticket>(ticketDTO);    
            await _context.Tickets.AddAsync(Data);
            await _context.SaveChangesAsync();

            return Ok(Data);
        }

        
        [HttpGet("TicketById")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
           
            if (ticket == null)
                return NotFound("Ticket Not Found");
            //var ticket = _Mapper.Map<Ticket>(ticketCheck);
            if(ticket.RecivedAdminId == null || ticket.AdminRecivedAt == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ticket.RecivedAdminId = await _userManager.GetUserIdAsync(user);
                }
                ticket.AdminRecivedAt = DateTime.Now;

                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
            }
            return Ok(ticket);
        }


        [HttpPost("TicketContactDone")]
        public async Task<IActionResult> TicketContactDone(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound("Ticket Not Found");
            //var ticket = _Mapper.Map<Ticket>(ticketCheck);
            ticket.ContactDoneAt = DateTime.Now;
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return Ok(ticket);
        }

        [HttpGet("NewTickets")]
        public async Task<IActionResult> GetNewTickets()
        {
            var tickets =  _context.Tickets.Where(x => x.RecivedAdminId == null || x.AdminRecivedAt == null).ToList();
            if (tickets == null)
                return NotFound("Ticket Not Found");
            return Ok(tickets);
        }

        [HttpGet("TicketsOfUser")]
        public async Task<IActionResult> GetTicketsOfUser(string id)
        {
            var tickets = _context.Tickets.Where(x => x.ApplicationUserId == id).ToList();
            if (tickets == null)
                return NotFound("Ticket Not Found");
            return Ok(tickets);
        }


    }
}
