using System;
using Topodata2.Classes;


namespace Topodata2.Models.Home
{
    public class HomeSlider : Model
    {
        public string PathImageSeason { get; set; }
        public string UrlVideo { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class HomeSliderImageSeason : Model
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class HomeSliderVideo : Model
    {
        public int Id { get; set; }
        public string UrlVideo { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class DeslinderModel : Model
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Ubicacion { get; set; }
        public string Telefono { get; set; }
        public string Movil { get; set; }
        public string Correo { get; set; }
        public string NoMatrical { get; set; }
        public string NoParsela { get; set; }
        public string NoDistrito { get; set; }
        public string Municipio { get; set; }
        public string Provincia { get; set; }
        public string Area { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class TextoHome : Model
    {
        public int Id { get; set; }
        public string Agrimensura { get; set; }
        public string EstudioSuelo { get; set; }
        public string Diseno { get; set; }
        public string Ingenieria { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class OurTeamModel : Model
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class Flipboard : Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class ContactUsModel : Model
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Mensaje { get; set; }
    }
}