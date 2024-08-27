using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
//using Aspose.Cells;
//using Aspose.Cells.Charts;
using Spire.Xls;
using Newtonsoft.Json.Linq;
using System.Web;

namespace anketa
{
    public partial class SurveyForm : Page
    {
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            /*
            string currentLogin = Context.User.Identity.Name;
            if (string.IsNullOrEmpty(currentLogin))
            {
                // Обработка случая, когда пользователь не авторизован
                Response.Redirect("Login.aspx");
                return;
            }

            var survey = new
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                Comments = CommentsTextBox.Text
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsonData = js.Serialize(survey);

            string fileName = Path.GetFileName(FileUpload.PostedFile.FileName);
            string filePath = Server.MapPath("~/excelfiles/") + fileName;

            // Сохранение файла на сервере
            FileUpload.SaveAs(filePath);

            // Загрузка данных из Excel в таблицу AnketFiles
            LoadExcelDataToDatabase(filePath);

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string storedProcedure = "InsertSurveyData";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SurveyData", jsonData);
                command.Parameters.AddWithValue("@FileName", fileName);
                command.Parameters.AddWithValue("@UserName", currentLogin);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            */
            string username = Context.User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                Response.Write("надо зарегистрироваться от имени все зависит");
                Uri uri = HttpContext.Current.Request.Url;
                string host = uri.Host;
                // в режиме отладки продолжается выполнение
                if (!host.Contains("localhost"))
                {
                    return;
                } else
                {
                    username = "sam";
                }
            }
            string jsonString = QueryArea.Text;
            if (jsonString=="нет данных")
            {
                Response.Write("у вас нет доступа на эту операцию");
                return;
            }
            string filename;
            Workbook workbook = new Workbook();
            string template= ListBoxConfigs.Text;
            // Response.Write($" Выбран {template}");
            // в зависимости от выбора должно быть
            if (string.IsNullOrEmpty(template))
            {
                Response.Write("Ничего не выбрано");
                return;
            }
            string templatename = TemplateName.Text;
            if (!string.IsNullOrEmpty(templatename))
            {
                filename = Server.MapPath(templatename);
                if (!File.Exists(filename))
                {
                    Response.Write("Ошибка шаблон не найден");
                    return;

                }
                else
                {
                    workbook.LoadFromFile(filename);
                }

            }
            JArray json;
            try
            {
                json = JArray.Parse(jsonString);
            }
            catch (Exception ex)
            {
                Response.Write("Ошибка парсинга JSON: " + ex.Message + "Пример правильного содержания:" +
                    "[{\"procedure\":\"publicbase.dbo.GetStudentsbyDate\"," +
                    "\"params\":[{\"param\":\"date\", \"value\":\"20240401\"," +
                    "\"type\":\"string\"},{\"param\":\"trouble\", \"value\":\"0\",\"type\":\"int\"}],\"sheetnum\":2}," +
                    "{\"procedure\":\"publicbase.dbo.GetStudentsbyDate\",\"param\":\"date\", \"value\":\"20240101\"," +
                    "\"type\":\"string\",\"sheetnum\":3}]");
                return;
            }
            string connectionString=ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    Response.Write("ошибка учетных данных");
                    return;
                }
                string paramName;
                string paramValue;
                string paramType;
                string errors;
                // Обработка каждой хранимой процедуры в JSON
                foreach (var query in json)
                {
                    string procedureName = query["procedure"].ToString();
                    int sheetNum = (int)query["sheetnum"];
                    // Создание команды для вызова хранимой процедуры
                    using (SqlCommand command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        paramName = "";
                        paramValue = "";
                        paramValue = "";
                        errors = "";
                        foreach (var param in query["params"])
                        {
                            paramName = param["param"].ToString();
                            paramValue = param["value"].ToString();
                            paramType = param["type"].ToString();

                            // Проверка наличия обязательных параметров
                            if (string.IsNullOrEmpty(procedureName) || string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(paramValue) || string.IsNullOrEmpty(paramType) || sheetNum <= 0)
                            {
                                if (string.IsNullOrEmpty(procedureName))
                                {
                                    errors += "Отсутствует обязательный параметр имя Процедуры в процедуре. ";
                                }
                                if (string.IsNullOrEmpty(paramName))
                                {
                                    errors += "Отсутствует обязательный параметр имя параметра - param.  ";
                                }
                                if (string.IsNullOrEmpty(paramValue))
                                {
                                    errors += "Отсутствует обязательный параметр значение параметра - value. ";
                                }
                                if (string.IsNullOrEmpty(paramType))
                                {
                                    errors += "Отсутствует обязательный параметр тип параметра - type, должен быть либо string либо int ";
                                }
                                if (sheetNum <= 0)
                                {
                                    errors += "Отсутствует обязательный параметр номер листа ";
                                }

                                errors += "Пример правильного содержания ячейки A1:" +
                                "[{\"procedure\":\"publicbase.dbo.GetStudentsbyDate\"," +
                                "\"params\":[{\"param\":\"date\", \"value\":\"20240401\"," +
                                "\"type\":\"string\"},{\"param\":\"trouble\", \"value\":\"0\",\"type\":\"int\"}],\"sheetnum\":2}," +
                                "{\"procedure\":\"publicbase.dbo.GetStudentsbyDate\",\"param\":\"date\", \"value\":\"20240101\"," +
                                "\"type\":\"string\",\"sheetnum\":3}]";

                                Response.Write("есть пустые значения" + errors);
                                return;
                                // код поставлю из другого проекта
                            }
                            SqlParameter sqlParam = new SqlParameter(paramName, GetSqlDbType(paramType))
                            {
                                Value = paramValue
                            };
                            command.Parameters.Add(sqlParam);
                        }
                        paramName = "username";
                        paramType = "string";
                        SqlParameter sqlUserName = new SqlParameter(paramName, GetSqlDbType(paramType))
                        {
                            Value = username
                        };
                        command.Parameters.Add(sqlUserName);

                        // Проверка существования листа и создание, если его нет
                        Worksheet resultSheet;
                        if (sheetNum > workbook.Worksheets.Count)
                        {
                            resultSheet = workbook.Worksheets.Add($"Sheet{sheetNum}");
                        }
                        else
                        {
                            resultSheet = workbook.Worksheets[sheetNum - 1];
                        }

                        try
                        {
                            // Выполнение команды и получение результатов
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                int row = 1;

                                // Запись наименований полей в первую строку
                                for (int col = 0; col < reader.FieldCount; col++)
                                {
                                    resultSheet.Range[row, col + 1].Text = reader.GetName(col);
                                }
                                row++;
                                int currentRow = 0;
                                while (reader.Read())
                                {
                                    for (int col = 0; col < reader.FieldCount; col++)
                                    {
                                        resultSheet.Range[row, col + 1].Text = reader[col].ToString();
                                    }
                                    row++;
                                    currentRow++;
                
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            Response.Write($"Извините, хранимой процедуры {procedureName} нет или у вас нет доступа: {ex.Message}");
                            return;
                        }
                    }
                }

            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveToStream(stream, FileFormat.Version2013);
                stream.Position = 0;

                // Отправка файла клиенту
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=result.xlsx");
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }
        private JObject configJson;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string filePath = Server.MapPath("~/www/rasp/config.json");
                string jsonContent = File.ReadAllText(filePath);
                configJson = JObject.Parse(jsonContent);

                if (configJson["version"] == null)
                {
                    Response.Write("Неверный формат JSON файла: отсутствует ключ 'version'.");
                    return;
                }
                ListBoxConfigs.Items.Clear();
                foreach (var config in configJson["configs"])
                {
                    ListBoxConfigs.Items.Add(config["name"].ToString());
                }
            }

        }
        protected void ListBoxConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ListBoxConfigs.SelectedItem != null)
            {
                string filePath = Server.MapPath("~/www/rasp/config.json");
                string jsonContent = File.ReadAllText(filePath);
                configJson = JObject.Parse(jsonContent);

                string selectedConfigName = ListBoxConfigs.SelectedItem.ToString();
                var selectedConfig = configJson["configs"].First(config => config["name"].ToString() == selectedConfigName);
                string username = Context.User.Identity.Name;
                Response.Write(username);
                TemplateName.Text= selectedConfig["template"].ToString();
                desc.Text= selectedConfig["desc"].ToString();
                int facValue = GetFacValue(username);
                if (facValue > 0)
                {
                    string configData = selectedConfig["data"].ToString();
                    configData = configData.Replace("%fac%", facValue.ToString());
                    QueryArea.Text = configData;
                } else
                {
                    QueryArea.Text = "нет данных";
                }
            }
        }

        private int GetFacValue(string username)
        {
            int facValue = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("publicbase.dbo.getfac", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@username", username));

                    connection.Open();
                    facValue = (int)command.ExecuteScalar();
                }
            }

            return facValue;
        }
        static SqlDbType GetSqlDbType(string type)
        {
            switch (type.ToLower())
            {
                case "int":
                    return SqlDbType.Int;
                case "string":
                    return SqlDbType.NVarChar;
                default:
                    throw new ArgumentException("Unsupported type");
            }
        }

    }
}
