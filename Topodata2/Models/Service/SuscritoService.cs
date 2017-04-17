using Topodata2.Models.Entities;
using Topodata2.ViewModels;

namespace Topodata2.Models.Service
{
    public class SuscritoService
    {
        private readonly TopodataContext _db = new TopodataContext();

        public bool Insert(string email)
        {
            var result = false;
            var newSuscrito = new Suscrito
            {
                Email = email,
                Informed = true
            };
            _db.Suscritoes.Add(newSuscrito);
            if (_db.SaveChanges() > 0)
            {
                result = true;
            }
            return result;
        }
        public bool Insert(SubscribeViewModel viewModel)
        {
            return Insert(viewModel.Email);
        }
    }
}