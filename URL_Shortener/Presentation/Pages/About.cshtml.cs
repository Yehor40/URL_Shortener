using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Domain.Entities;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Pages;

public class AboutModel : PageModel
{
    private readonly AppDbContext _context;

    public AboutModel(AppDbContext context)
    {
        _context = context;
    }

    public List<AboutInfo> Infos { get; set; } = new();
    public bool IsAdmin { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Infos = await _context.AboutInfos.ToListAsync(cancellationToken);
        IsAdmin = User.IsInRole("Admin");
    }

    public async Task<IActionResult> OnPostAsync(int id, string content, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var info = await _context.AboutInfos.FindAsync(new object[] { id }, cancellationToken);
        if (info != null)
        {
            info.Content = content;
            info.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            TempData["Message"] = "Content updated successfully!";
        }

        return RedirectToPage();
    }
}
