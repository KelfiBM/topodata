﻿using System.Collections.Generic;
using System.Data;
using Topodata2.Classes;
using Topodata2.Managers;
using Topodata2.resources.Strings;

namespace Topodata2.Models.Home
{
    public static class HomeManager
    {
        public static HomeSlider GetLastHomeSliderData()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.HomeSlider,
                DatabaseParameters.GetLastHomeSliderData);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (HomeSlider) i)[0];
            return result;
        }

        public static TextoHome GetLastHomeText()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.TextoHome,
                DatabaseParameters.GetLastHomeText);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (TextoHome) i)[0];
            return result;
        }

        public static TextoHomeViewModel GetLastHomeTextViewModel()
        {
            var value = GetLastHomeText();
            if (value == null) return null;
            var result = new TextoHomeViewModel
            {
                EstudioSuelo = value.EstudioSuelo,
                Diseno = value.Diseno,
                Agrimensura = value.Agrimensura,
                Ingenieria = value.Ingenieria
            };
            return result;
        }

        public static bool AddHomeText(TextoHome model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.InsertTextoHome,
                model.Agrimensura, model.EstudioSuelo, model.Diseno, model.Ingenieria);
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool AddHomeText(TextoHomeViewModel viewModel)
        {
            return AddHomeText(new TextoHome
            {
                Agrimensura = viewModel.Agrimensura,
                Diseno = viewModel.Diseno,
                EstudioSuelo = viewModel.EstudioSuelo,
                Ingenieria = viewModel.Ingenieria
            });
        }

        public static List<OurTeam> GetAllOurTeam()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.OurTeam,
                DatabaseParameters.GetAllOurTeam);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (OurTeam) i);
            return result;
        }

        public static bool AddOurTeam(OurTeam model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.InsertOurTeam,
                model.Nombre, model.Cargo, model.Email, model.ImagePath);
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool AddOurTeam(OurTeamViewModel viewModel)
        {
            return AddOurTeam(new OurTeam
            {
                Nombre = viewModel.Nombre,
                Cargo = viewModel.Cargo,
                Email = viewModel.Email,
                ImagePath = viewModel.ImagePath
            });
        }

        public static bool DeleteOurTeam(OurTeam model)
        {
            var file = GetImagePathOurTeam(model);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.DeleteOurTeam,
                model.Id.ToString());
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        private static string GetImagePathOurTeam(OurTeam model)
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.OurTeam,
                DatabaseParameters.GetImagePathOurTeam, model.Id.ToString());
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (OurTeam) i)[0].ImagePath;
            return result;
        }

        public static HomeSliderVideo GetCurrentHomeSliderVideo()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.HomeSliderVideo,
                DatabaseParameters.GetCurrentHomeSliderVideo);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (HomeSliderVideo) i)[0];
            return result;
        }

        public static HomeSliderVideoViewModel GetCurrentHomeSliderVideoViewModel()
        {
            var value = GetCurrentHomeSliderVideo();
            if (value == null) return null;
            var result = new HomeSliderVideoViewModel
            {
                UrlVideo = value.UrlVideo
            };
            return result;
        }

        public static bool AddHomeSliderVideo(HomeSliderVideo model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.InsertHomeSlideVideo,
                Youtube.GetVideoId(model.UrlVideo));
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool AddHomeSlideVideo(HomeSliderVideoViewModel viewModel)
        {
            return AddHomeSliderVideo(new HomeSliderVideo
            {
                UrlVideo = viewModel.UrlVideo
            });
        }

        public static List<Flipboard> GetAllFlipboard()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.Flipboard,
                DatabaseParameters.GetAllFlipboard);
            if (value.Count == 0) return null;
            var result = value.ConvertAll(i => (Flipboard) i);
            return result;
        }

        public static bool AddFlipboard(Flipboard model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.InsertFlipboard,
                model.Name, model.Url);
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool AddFlipboard(FlipboardViewModel model)
        {
            return AddFlipboard(new Flipboard
            {
                Name = model.Name,
                Url = model.Url
            });
        }

        public static bool DeleteFlipboard(Flipboard model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.DeleteFlipboard,
                model.Id.ToString());
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool FlipboardExists()
        {
            var value = DatabaseManager.ExecuteQuery(CommandType.Text, ModelType.Flipboard,
                DatabaseParameters.GetFlipboardExists);
            return value.Count != 0;
        }

        public static bool AddHomeSlideImageSeason(HomeSliderImageSeason model)
        {
            var result = false;
            var value = DatabaseManager.ExecuteQuery(CommandType.StoredProcedure, ModelType.Default,
                DatabaseParameters.InsertHomeImageSeason,
                model.ImagePath);
            if (value.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool AddHomeSlideImageSeason(HomeSliderImageSeasonViewModel viewModel)
        {
            var result = AddHomeSlideImageSeason(new HomeSliderImageSeason
            {
                ImagePath = viewModel.ImagePath
            });
            return result;
        }
    }
}