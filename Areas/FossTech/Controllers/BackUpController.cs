using FossTech.Data;
using FossTech.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FossTech.Areas.FossTech.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("FossTech")]
    public class BackupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BackupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Now()
        {
            string backupFilePath = "C:\\backups\\vedaedtech.com.bak";
            string databaseName = "vedaedtech.com";


            try
            {
                var backupQuery = $"BACKUP DATABASE [{databaseName}] TO DISK = '{backupFilePath}' WITH INIT";

                // Execute the backup query
                _context.Database.ExecuteSqlRaw(backupQuery);

                return RedirectToAction("Index", "FossTech", new { area = "FossTech" }).WithSuccess("Database backup completed successfully.");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "FossTech", new { area = "FossTech" }).WithError($"An error occurred: {ex.Message}");
            }

            // return RedirectToAction("Index", "FossTech", new { area = "FossTech" }).WithSuccess("Backup done");
        }

    }
}
