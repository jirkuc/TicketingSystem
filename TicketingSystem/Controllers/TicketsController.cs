using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.DTO;
using TicketingSystem.Enums;
using TicketingSystem.Models;
using TicketingSystem.Services;

namespace TicketingSystem.Controllers {
    [Authorize]
    public class TicketsController : Controller {

        private TicketService _ticketService;
        private RoleManager<AppRole> _roleManager;
        private UserManager<AppUser> _userManager;

        public TicketsController(TicketService service, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager) {
            _ticketService = service;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index() {
            return View(await _ticketService.GetAllTicketsAsync());
        }

        // GET: Tickets/StaNew
        public async Task<IActionResult> StaNew() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.NEW));
        }
        // GET: Tickets/StaWorking
        public async Task<IActionResult> StaWorking() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.WORKING));
        }
        // GET: Tickets/StaCustomer
        public async Task<IActionResult> StaCustomer() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.CUSTOMERONHOLD));
        }
        // GET: Tickets/StaResolved
        public async Task<IActionResult> StaResolved() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.RESOLVED));
        }
        // GET: Tickets/StaClosed
        public async Task<IActionResult> StaClosed() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.CLOSED));
        }
        // GET: Tickets/StaVoid
        public async Task<IActionResult> StaVoid() {
            return View(await _ticketService.GetAllTicketsAsync(TicketState.VOID));
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync(id);
            if (ticket == null) {
                return View("NotFound");
            }
            return View(ticket);
        }
        // GET: Tickets/Create
        public async Task<IActionResult> CreateAsync() {
            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(TicketDTO ticket) {
            var authenticatedUserId = _userManager.GetUserId(User);
            if (ModelState.IsValid) {
                await _ticketService.CreateAsync(ticket, authenticatedUserId);
                return RedirectToAction(nameof(Index));
            }
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var authenticatedUserId = _userManager.GetUserId(User);
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketDTO ticketDTO, string statusComment) {
            if (id != ticketDTO.Id) {
                return View("NotFound");
            }

            var ticketToUpdate = await _ticketService.GetTicketAsync(id);
            var authenticatedUserId = _userManager.GetUserId(User);
            if (ticketToUpdate == null) {
                return View("NotFound");
            }
            else if (ticketToUpdate.CurrentActivityNumber != ticketDTO.CurrentActivityNumber) {
                ModelState.AddModelError("", "Ticket has been changed by a different instance, go back and try again.");
            }
            else if (ticketToUpdate.ClosedFlag) {
                ModelState.AddModelError("", "Ticket has been closed and cannot be modified. Please open a new ticket.");
            }
            
            else if (
                ticketToUpdate.CustomerName.Equals(ticketDTO.CustomerName, StringComparison.OrdinalIgnoreCase)
                && ticketToUpdate.ProblemDescription.Equals(ticketDTO.ProblemDescription, StringComparison.OrdinalIgnoreCase)
                && ticketToUpdate.Priority == ticketDTO.Priority
                && ticketToUpdate.ReportSource == ticketDTO.ReportSource
                ) {
                ModelState.AddModelError("", "Ticket values have not been modified, make changes or go back.");
            }
            else if (ModelState.IsValid) {
                await _ticketService.ModifyAsync(id, ticketDTO, authenticatedUserId, statusComment);
                return RedirectToAction(nameof(Details), new { id = ticketDTO.Id });
            }
            await FillSelectsAsync(authenticatedUserId);
            return View(ticketDTO);
        }

        // GET: Tickets/Comment/5
        [Authorize]
        public async Task<IActionResult> Comment(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/Comment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int id, TicketDTO ticketDTO, string statusComment, InternalFlag InternalComment) {
            if (id != ticketDTO.Id) {
                return View("NotFound");
            }
            var ticketToUpdate = await _ticketService.GetTicketAsync(id);
            var authenticatedUserId = _userManager.GetUserId(User);
            if (ticketToUpdate == null) {
                return View("NotFound");
            }
            else if (ModelState.IsValid) {
                await _ticketService.Comment(id, ticketDTO, authenticatedUserId, statusComment, InternalComment);
                return RedirectToAction(nameof(Details), new { id = ticketDTO.Id });
            }
            await FillSelectsAsync(authenticatedUserId);
            return View(ticketToUpdate);
        }

        // GET: Tickets/StartWork/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> StartWork(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/StartWork/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartWork(int id, TicketDTO ticketDTO, string statusComment, InternalFlag internalCommentFlagFromTicket) {
            InternalFlag internalCommentFlag = InternalFlag.CUSTOMER;
            TicketActivityEnum ticketActivityValue = TicketActivityEnum.STARTWORK;

            return await CallServiceToModifyTicketAndAddActivity(id, ticketDTO, statusComment, internalCommentFlag, ticketActivityValue);
        }

        // GET: Tickets/CustomerTime/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> CustomerTime(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/CustomerTime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerTime(
            int id,
            TicketDTO ticketDTO,
            string statusComment,
            InternalFlag internalCommentFlagFromTicket) {

            InternalFlag internalCommentFlag = InternalFlag.CUSTOMER;
            TicketActivityEnum ticketActivityValue = TicketActivityEnum.CUSTOMERONHOLD;

            return await CallServiceToModifyTicketAndAddActivity(id, ticketDTO, statusComment, internalCommentFlag, ticketActivityValue);
        }

        // GET: Tickets/Resolve/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> Resolve(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(
            int id,
            TicketDTO ticketDTO,
            string statusComment,
            InternalFlag internalCommentFlagFromTicket) {

            InternalFlag internalCommentFlag = InternalFlag.CUSTOMER;
            TicketActivityEnum ticketActivityValue = TicketActivityEnum.RESOLVE;

            return await CallServiceToModifyTicketAndAddActivity(id, ticketDTO, statusComment, internalCommentFlag, ticketActivityValue);
        }

        // GET: Tickets/Close/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> Close(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(
            int id,
            TicketDTO ticketDTO,
            string statusComment,
            InternalFlag internalCommentFlagFromTicket) {

            InternalFlag internalCommentFlag = InternalFlag.CUSTOMER;
            TicketActivityEnum ticketActivityValue = TicketActivityEnum.CLOSE;

            return await CallServiceToModifyTicketAndAddActivity(id, ticketDTO, statusComment, internalCommentFlag, ticketActivityValue);
        }

        // GET: Tickets/VoidTicket/5
        [Authorize(Roles = "TECHNICIAN, ADMIN")]
        public async Task<IActionResult> VoidTicket(int? id) {
            if (id == null) {
                return View("NotFound");
            }
            var ticket = await _ticketService.GetTicketAsync((int)id);
            if (ticket == null) {
                return View("NotFound");
            }

            var authenticatedUserId = _userManager.GetUserId(User);
            await FillSelectsAsync(authenticatedUserId);
            return View(ticket);
        }

        // POST: Tickets/VoidTicket/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoidTicket(
            int id,
            TicketDTO ticketDTO,
            string statusComment,
            InternalFlag internalCommentFlagFromTicket) {

            InternalFlag internalCommentFlag = InternalFlag.CUSTOMER;
            TicketActivityEnum ticketActivityValue = TicketActivityEnum.VOID;

            return await CallServiceToModifyTicketAndAddActivity(id, ticketDTO, statusComment, internalCommentFlag, ticketActivityValue);
        }

        private async Task<IActionResult> CallServiceToModifyTicketAndAddActivity(int id, TicketDTO ticketDTO, string statusComment, InternalFlag internalComment, TicketActivityEnum ticketActvityValue) {
            if (id != ticketDTO.Id) {
                return View("NotFound");
            }
            var ticketToUpdate = await _ticketService.GetTicketAsync(id);
            var authenticatedUserId = _userManager.GetUserId(User);
            if (ticketToUpdate == null) {
                return View("NotFound");
            }
            else if (ticketToUpdate.ClosedFlag) {
                ModelState.AddModelError("", "The ticket has been closed and the state cannot be modified. Please jump on another ticket.");
            }
            else if (ticketToUpdate.CurrentActivityNumber != ticketDTO.CurrentActivityNumber) {
                ModelState.AddModelError("", "The ticket has been changed by a different instance, go back and try again.");
            }
            else if (ticketActvityValue == TicketActivityEnum.CLOSE && !ticketToUpdate.TicketState.Equals(TicketState.RESOLVED)) {
                ModelState.AddModelError("", "The ticket has not been RESOLVED, resolve the ticket first and then it can be closed.");
            }
            else if (ModelState.IsValid) {
                await _ticketService.ModifyState(
                    id,
                    ticketDTO,
                    authenticatedUserId,
                    statusComment,
                    internalComment,
                    ticketActvityValue
                    );
                return RedirectToAction(nameof(Details), new { id = ticketDTO.Id });
            }
            await FillSelectsAsync(authenticatedUserId);
            return View(ticketToUpdate);
        }

        /*
        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var ticketDTO = await _ticketService.ITS_Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketDTO == null) {
                return NotFound();
            }

            return View(ticketDTO);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var ticketDTO = await _ticketService.ITS_Tickets.FindAsync(id);
            if (ticketDTO != null) {
                _ticketService.ITS_Tickets.Remove(ticketDTO);
            }

            await _ticketService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id) {
            return _ticketService.ITS_Tickets.Any(e => e.Id == id);
        }*/

        private async Task FillSelectsAsync(string userId) {
            AppUser authenticatedUser = await _ticketService.GetUser(userId);
            AppRole authenticatedUserRole = await _ticketService.GetRole(authenticatedUser.DefaultRoleId);

            string authenticatedUserRoleName = authenticatedUserRole.NormalizedName;
            if (authenticatedUserRoleName.Equals("CUSTOMER")) {
                List<ReportSource> reportSources = new() { ReportSource.CUSTOMER };
                ViewBag.ReportSource = new SelectList(reportSources);
            }
            else {
                ViewBag.ReportSource = new SelectList(Enum.GetNames(typeof(ReportSource)));
            }

            var rolesDropDownData = await _ticketService.GetRoleDropdownsData();
            var usersDropDownData = await _ticketService.GetUserDropdownsData();
            ViewBag.Roles = new SelectList(rolesDropDownData.Roles, "Id", "NormalizedName");
            ViewBag.Users = new SelectList(usersDropDownData.Users, "NormalizedUserName", "NormalizedUserName");
            ViewBag.CustomerName = usersDropDownData.Users;
            ViewBag.Priority = new SelectList(Enum.GetNames(typeof(Priority)));
            ViewBag.TicketState = new SelectList(Enum.GetNames(typeof(TicketState)));
        }
    }
}
