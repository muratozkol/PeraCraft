using Microsoft.AspNetCore.Mvc;
using Peracraft.Data;
using Microsoft.EntityFrameworkCore;

namespace Peracraft.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public UserProfileViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return View("Anonymous");
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Id == kullaniciId);

            if (kullanici == null)
            {
                return View("Anonymous");
            }

            return View(kullanici);
        }
    }
} 