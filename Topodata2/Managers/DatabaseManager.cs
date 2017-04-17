using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Topodata2.Classes;
using Topodata2.Models.Home;
using Topodata2.Models.UserFolder;

namespace Topodata2.Managers
{
    public static class DatabaseManager
    {
        private static readonly string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
        public static List<Model> ExecuteQuery(CommandType commandType, ModelType modelType, string commandText, params string[] values)
        {
            var result = new List<Model>();
            using (var sqlConnection = new SqlConnection(Connection))
            {
                using (var sqlCommand = new SqlCommand
                {
                    CommandType = commandType,
                    CommandText = commandText,
                    Connection = sqlConnection
                })
                {
                    sqlConnection.Open();
                    switch (commandType)
                    {
                        case CommandType.Text:
                            result = ExecuteDataReader(sqlCommand,modelType,values);
                            break;
                        case CommandType.StoredProcedure:
                            if (ExecuteStoreProcedure(sqlCommand, values))
                            {
                                result.Add(new StoreProcedure
                                {
                                    Success = true
                                });
                            }
                            break;
                        case CommandType.TableDirect:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
                    }
                }
            }
            return result;
        }

        private static List<Model> ExecuteDataReader(SqlCommand sqlCommand, ModelType modelType, params string[] values)
        {
            if (values.Length > 0)
            {
                for(var i = 0; i < values.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue("value" + i, values[i]);
                }
            }
            var result = new List<Model>();
            using (var sqlDataReader = sqlCommand.ExecuteReader())
            {
                if (!sqlDataReader.HasRows) return result;
                switch (modelType)
                {
                    case ModelType.Default:
                        break;
                    case ModelType.HomeSlider:
                        result.AddRange(GetHomeSlider(sqlDataReader));
                        break;
                    case ModelType.HomeSliderImageSeason:                        
                        //GetHomeSliderImageSeason(sqlDataReader);
                        break;
                    case ModelType.HomeSliderVideo:
                        result.AddRange(GetHomeSliderVideo(sqlDataReader));
                        break;
                    case ModelType.DeslindeModel:
                        //GetDeslindeModel(sqlDataReader);
                        break;
                    case ModelType.TextoHome:
                        result.AddRange(GetTextoHome(sqlDataReader));
                        break;
                    case ModelType.OurTeam:
                        result.AddRange(GetOurTeam(sqlDataReader));
                        break;
                    case ModelType.Flipboard:
                        result.AddRange(GetFlipboard(sqlDataReader));
                        break;
                    case ModelType.User:
                        result.AddRange(GetUser(sqlDataReader));
                        break;
                    case ModelType.Subscribed:
                        result.AddRange(GetSubscribed(sqlDataReader));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(modelType), modelType, null);
                }
            }
            return result;
        }

        private static bool ExecuteStoreProcedure(SqlCommand sqlCommand, IReadOnlyList<string> values)
        {
            var result = false;
            for (var i = 0; i < values.Count; i++)
            {
                sqlCommand.Parameters.AddWithValue("value" + i, values[i]);
            }
            var returnValue = sqlCommand.Parameters.Add("returnVal", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            sqlCommand.ExecuteNonQuery();
            if (Convert.ToInt32(returnValue.Value) == 1)
            {
                result = true;
            }
            return result;
        }

        private static IEnumerable<HomeSlider> GetHomeSlider(IDataReader reader)
        {
            var result = new List<HomeSlider>();
            while (reader.Read())
            {
                result.Add(new HomeSlider
                {
                    UrlVideo = reader.GetString(reader.GetOrdinal("UrlVideo")),
                    PathImageSeason = reader.GetString(reader.GetOrdinal("PathImageSeason")),
                    RegDate = reader.GetDateTime(reader.GetOrdinal("RegDate"))
                });
            }
            return result;
        }

        private static IEnumerable<HomeSliderImageSeason> GetHomeSliderImageSeason(IDataReader reader)
        {
            return null;
        }

        private static IEnumerable<HomeSliderVideo> GetHomeSliderVideo(IDataReader reader)
        {
            var result = new List<HomeSliderVideo>();
            while (reader.Read())
            {
                result.Add(new HomeSliderVideo
                {
                    UrlVideo = reader.GetString(reader.GetOrdinal("UrlVideo")),
                    RegDate = reader.GetDateTime(reader.GetOrdinal("RegDate")),
                    Id = reader.GetInt32(reader.GetOrdinal("Id"))
                });
            }
            return result;
        }

        private static IEnumerable<DeslinderModel> GetDeslindeModel(IDataReader reader)
        {
            var result = new List<DeslinderModel>();
            while (reader.Read())
            {
                result.Add(new DeslinderModel());
            }
            return result;
        }

        private static IEnumerable<TextoHome> GetTextoHome(IDataReader reader)
        {
            var result = new List<TextoHome>();
            while (reader.Read())
            {
                result.Add(new TextoHome
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Agrimensura = reader.GetString(reader.GetOrdinal("Agrimensura")),
                    EstudioSuelo = reader.GetString(reader.GetOrdinal("EstudioSuelo")),
                    Diseno = reader.GetString(reader.GetOrdinal("Diseno")),
                    Ingenieria = reader.GetString(reader.GetOrdinal("Ingenieria")),
                    RegDate = reader.GetDateTime(reader.GetOrdinal("RegDate"))
                });
            }
            return result;
        }

        private static IEnumerable<OurTeamModel> GetOurTeam(IDataReader reader)
        {
            var result = new List<OurTeamModel>();
            while (reader.Read())
            {
                result.Add(new OurTeamModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Cargo = reader.GetString(reader.GetOrdinal("Cargo")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                });
            }
            return result;
        }

        private static IEnumerable<Flipboard> GetFlipboard(IDataReader reader)
        {
            var result = new List<Flipboard>();
            while (reader.Read())
            {
                result.Insert(0,new Flipboard
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Url = reader.GetString(reader.GetOrdinal("Url")),
                    RegDate = reader.GetDateTime(reader.GetOrdinal("RegDate"))
                });
            }
            return result;
        }

        private static IEnumerable<UserModel> GetUser(IDataReader reader)
        {
            var result = new List<UserModel>();
            while (reader.Read())
            {
                result.Insert(0, new UserModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    RegDate = reader.GetDateTime(reader.GetOrdinal("RegDate")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Informed = reader.GetBoolean(reader.GetOrdinal("Informed")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Rol = reader.GetString(reader.GetOrdinal("Descripcion")),
                    Password = reader.GetString(reader.GetOrdinal("Password"))
                });
            }
            return result;
        }

        private static IEnumerable<UserModel> GetSubscribed(IDataReader reader)
        {
            var result = new List<UserModel>();
            while (reader.Read())
            {
                result.Insert(0, new UserModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Email = reader.GetString(reader.GetOrdinal("Email"))
                });
            }
            return result;
        }
    }

    public class StoreProcedure : Model
    {
        public bool Success { get; set; }
    }
}