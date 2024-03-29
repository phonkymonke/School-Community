﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

namespace Lab4.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public StudentsController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(int? ID)
        {
            var viewModel = new CommunityViewModel();
            viewModel.Students = await _context.Students
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (ID != null)
            {
                viewModel.CommunityMemberships = from m in _context.CommunityMemberships
                                                              .Include(i => i.Community)
                                                              .OrderBy(i => i.Community.Title)
                                                              .Where(x => x.StudentID == ID)
                                                 select m;
            }

            return View(viewModel);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> EditMemberships(int? ID)
        {
            var viewModel = new StudentMembershipViewModel();
            if (ID == null)
            {
                return NotFound();
            }

            viewModel.Student = await _context.Students.FindAsync(ID);
            if (viewModel.Student == null)
            {
                return NotFound();
            }

            var registered = _context.Communities.Where(i => (from m in _context.CommunityMemberships
                                        .Include(i => i.Community)
                                        .OrderBy(i => i.Community.Title)
                                        .Where(x => x.StudentID == ID)
                                                              select m).Any(m => i == m.Community)).OrderBy(i => i.Title);

            var unregistered = _context.Communities.Except(registered).OrderBy(i => i.Title);

            List<CommunityMembershipViewModel> communities = new List<CommunityMembershipViewModel>();

            foreach (var community in unregistered)
            {
                var studentModel = new CommunityMembershipViewModel();
                studentModel.IsMember = false;
                studentModel.Title = community.Title;
                studentModel.CommunityId = community.ID;
                communities.Add(studentModel);
            }

            foreach (var community in registered)
            {
                var studentModel = new CommunityMembershipViewModel();
                studentModel.IsMember = true;
                studentModel.Title = community.Title;
                studentModel.CommunityId = community.ID;
                communities.Add(studentModel);
            }

            viewModel.CommunityMemberships = communities;

            return View(viewModel);
        }

        public IActionResult AddMemberships(int studentID, string communityID)
        {
            _context.CommunityMemberships.Add(new CommunityMembership { StudentID = studentID, CommunityID = communityID });
            _context.SaveChanges();
            return RedirectToAction("EditMemberships", new { ID = studentID });
        }


        public IActionResult RemoveMemberships(int studentID, string communityID)
        {
            foreach (var membership in _context.CommunityMemberships.Where(i => i.CommunityID == communityID && i.StudentID == studentID))
            {
                _context.CommunityMemberships.Remove(membership);
            }
            _context.SaveChanges();
            return RedirectToAction("EditMemberships", new { ID = studentID });
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
